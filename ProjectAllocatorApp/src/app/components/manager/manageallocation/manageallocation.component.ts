import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AllocatorService } from 'src/app/services/allocator.service';
import { ManagerService } from 'src/app/services/manager.service';

@Component({
  selector: 'app-manageallocation',
  templateUrl: './manageallocation.component.html',
  styleUrls: ['./manageallocation.component.css']
})
export class ManageallocationComponent implements OnInit {

  loading: boolean = false;
  benchAllocation!: FormGroup;
  employeeId !: number;
  constructor(
    private managerService: ManagerService, 
    private fb: FormBuilder,
    private router : Router,
    private route : ActivatedRoute,
    private allocatorService : AllocatorService
  ){ }

  ngOnInit(): void {
    this.route.params.subscribe((params) =>{
      this.employeeId = params['id'];
      this.loadEmployeeId(this.employeeId);
  });
  this.benchAllocation = this.fb.group({
    employeeId : [this.employeeId],
    type : [,[Validators.required]],
    details: ['',[Validators.required]],
    typeId:[1],
    trainingId: [1,[Validators.required]],
    internalProjectId :[1,[Validators.required]]
  });
  };
  get formControls() {
    return this.benchAllocation.controls;
  }

  
  loadEmployeeId(id : number)
  {
   // this.loading = true;

    this.managerService.getEmployeeById(id).subscribe({
      next:(response) =>{
        if(response.success){
          this.benchAllocation.patchValue({
            employeeId : response.data.employeeId,
            startDate : response.data.startDate,
            endDate : response.data.endDate,
            typeId : response.data.typeId,
          });

        }
        else{
          console.error('Failed to fetch employee',response.message);
        }
        this.loading = false;

      },
      error:(err) =>{
        //alert(err.error.message);
        this.loading = false;

      },
      complete:()=>{
        console.log("completed");
      }
    })

  }
  onSubmit(){
    //this.loading = true;

    console.log(this.benchAllocation.value);
    if(this.benchAllocation.valid){
      this.allocatorService.addAllocation(this.benchAllocation.value).subscribe({
        next:(response) =>{
          if(response.success){
            if(this.benchAllocation.value.trainingId != 1){
              alert('Successfully allocated to training!')
            }
            else{
              alert('Successfully allocated to internal project!')
            }
            this.router.navigate(['/benchlist']);
            
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
