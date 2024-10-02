import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminServce } from 'src/app/services/admin.service';
import { AllocatorService } from 'src/app/services/allocator.service';

@Component({
  selector: 'app-employee-add',
  templateUrl: './employee-add.component.html',
  styleUrls: ['./employee-add.component.css']
})

export class EmployeeAddComponent implements OnInit {
  loading: boolean = false;
  empForm!: FormGroup;
  skillsSuggestion: string[] = ['Web Forms', 'MVC', '.Net Core', 'Blazor', 'Angular', 'PHP', 'Java', 'React', 'TypeScript', 'Power BI', 'MS SQL Server', 'Oracle'];
  selectedSkills: string[] = [];
  skills: string[] = [];
  showSuggestions: boolean = false;
empId:number = 0;
  constructor(private fb: FormBuilder, private router: Router, private adminService: AdminServce,public allocatorService:AllocatorService) {}

  ngOnInit(): void {
    this.empForm = this.fb.group({
      employeeName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      emailId: ['', [Validators.required, Validators.email]],
      benchStartDate: ['', [Validators.required]],
      benchEndDate: [null],
      jobRoleId: [, [Validators.required]],
      skills: [[],[Validators.required]]
    });

    this.empForm.get('skills')?.valueChanges.subscribe(() => {
      this.empForm.valid ? this.enableSubmit() : this.disableSubmit();
    });
  }
  get formControl(){
    return this.empForm.controls;
   }
   minDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = (today.getMonth() + 1).toString().padStart(2, '0'); // January is 0!
    const day = today.getDate().toString().padStart(2, '0');
 
    return `${year}-${month}-${day}`;
  }

onSkillInput(event: Event): void {
  const inputElement = event.target as HTMLInputElement;
  const value = inputElement.value.trim(); 

  const segments = value.split(',');
  const lastSegment = segments[segments.length - 1].trim();

  if (lastSegment.length > 2) {
    this.selectedSkills = this.skillsSuggestion.filter(skill =>
      skill.toLowerCase().includes(lastSegment.toLowerCase())
      && !this.skills.includes(skill)
    );
  if (!this.selectedSkills.includes(lastSegment) && !this.skills.includes(lastSegment)) {
      this.selectedSkills.push(lastSegment);
    }
    this.showSuggestions = this.selectedSkills.length > 0;
  } else {
    this.selectedSkills = [];
    this.showSuggestions = false;
  }
}
  addSkill(skill: string): void {
    if (skill && !this.skills.includes(skill)) {
      this.skills.push(skill);
      this.empForm.get('skills')?.setValue(this.skills); 
      this.selectedSkills = [];
      this.showSuggestions = false;
    }
  }

  removeSkill(index: number): void {
    if (index >= 0 && index < this.skills.length) {
      this.skills.splice(index, 1);
      this.empForm.get('skills')?.setValue(this.skills); 
    }
  }

  enableSubmit(): void {
    const submitBtn = document.getElementById('submitButton') as HTMLButtonElement;
    if (submitBtn) {
      submitBtn.disabled = false;
    }
  }

  disableSubmit(): void {
    const submitBtn = document.getElementById('submitButton') as HTMLButtonElement;
    if (submitBtn) {
      submitBtn.disabled = true;
    }
  }
 
  OnSubmit(): void {
    this.loading = true;

    if (this.empForm.valid) {
      const formData = {
        employeeName: this.empForm.value.employeeName,
        emailId: this.empForm.value.emailId,
        benchStartDate: this.empForm.value.benchStartDate,
        benchEndDate: this.empForm.value.benchEndDate,
        jobRoleId: this.empForm.value.jobRoleId,
        skills: this.skills
      };

      this.adminService.addEmployee(formData).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/employeeList']);
          } else {
            window.alert(response.message);
          }
        },
        error: (err) => {
          window.alert(err.error.message);
          this.loading = false;
        },
        complete: () => {
          console.log("Completed");
          this.loading = false;
        }
      });
    }
  }
}