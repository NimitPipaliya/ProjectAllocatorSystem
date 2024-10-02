import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AllocatorService } from 'src/app/services/allocator.service';

@Component({
  selector: 'app-allocateform',
  templateUrl: './allocateform.component.html',
  styleUrls: ['./allocateform.component.css']
})
export class AllocateformComponent implements OnInit {
  loading: boolean = false;
  setBenchForm!: FormGroup;
  employeeId !: number;
  constructor(
    private allocatorService: AllocatorService, 
    private fb: FormBuilder,
    private router : Router,
    private route : ActivatedRoute,
 
  ){ }

  ngOnInit(): void {
    this.route.params.subscribe((params) =>{
      this.employeeId = params['id'];
      this.loadEmployeeId(this.employeeId);
  });
  this.setBenchForm = this.fb.group({
    employeeId : [0,Validators.required],
    startDate: ['',[Validators.required]],
    endDate: [null,],
    details: ['',[Validators.required,Validators.minLength(10)]],
    typeId: [2,[Validators.required]],
    trainingId: [1,[Validators.required]],
    internalProjectId :[1,[Validators.required]]
  });
  };
  get formControls() {
    return this.setBenchForm.controls;
  }
  
  loadEmployeeId(id : number)
  {
    this.loading = true;

    this.allocatorService.getEmployeeById(id).subscribe({
      next:(response) =>{
        if(response.success){
          this.setBenchForm.patchValue({
            employeeId : response.data.employeeId,
            
          });

        }
        else{
          console.error('Failed to fetch employee',response.message);
        }
        this.loading = false;

      },
      error:(err) =>{
        alert(err.error.message);
        this.loading = false;

      },
      complete:()=>{
        console.log("completed");
      }
    })

  }

  minDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = (today.getMonth() + 1).toString().padStart(2, '0'); // January is 0!
    const day = today.getDate().toString().padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
  updateEmployee()
  {
    //this.loading = true;

    this.allocatorService.updateEmployee(this.setBenchForm.value).subscribe({
      next:(response) =>{
        if(response.success){
         console.log('success');
         this.router.navigate(['/allocatorlist']);
        }
        else{
          console.error('Failed to fetch employee',response.message);
        }
        this.loading = false;

      },
      error:(err) =>{
        alert(err.error.message);
        this.loading = false;

      },
      complete:()=>{
        console.log("completed");
      }
    })

  }

  onSubmit(){
    //this.loading = true;

    console.log(this.setBenchForm.value);
    if(this.setBenchForm.valid){
      this.allocatorService.addAllocation(this.setBenchForm.value).subscribe({
        next:(response) =>{
          if(response.success){
            this.updateEmployee();
            
          }
          else{
            alert(response.message);
          }
          this.loading = false;

        },
        error:(err) =>{
          console.error('Failed to add allocation: ',err.error.message);
          alert(err.error.message);
          this.loading = false;

        },
        complete:() =>{
          console.log('completed');
        }
      });
    }
  }
}