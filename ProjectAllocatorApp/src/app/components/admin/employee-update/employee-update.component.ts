import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminServce } from 'src/app/services/admin.service';

@Component({
  selector: 'app-employee-update',
  templateUrl: './employee-update.component.html',
  styleUrls: ['./employee-update.component.css']
})
export class EmployeeUpdateComponent implements OnInit{
  
  loading: boolean = false;
  empForm!: FormGroup;
  skillsSuggestion: string[] = ['Web Forms', 'MVC', '.Net Core', 'Blazor', 'Angular', 'PHP', 'Java', 'React', 'TypeScript', 'Power BI', 'MS SQL Server', 'Oracle'];
  selectedSkills: string[] = [];
  skills: string[] = [];
  showSuggestions: boolean = false;
  employeeId: number = 0; 

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private adminService: AdminServce,
      private datePipe: 
DatePipe
  ) {}

  ngOnInit(): void {
    this.employeeId = Number(this.route.snapshot.paramMap.get('employeeId'));

    this.empForm = this.fb.group({
      employeeName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      emailId: ['', [Validators.required, Validators.email]],
      benchStartDate: ['', [Validators.required]],
      benchEndDate: [null],
      jobRoleId: [0, [Validators.required]],
      typeId:0,
      skills: [[], [Validators.required]]
    });

    // Fetch existing employee data by ID and populate the form
    this.adminService.getEmployeeById(this.employeeId).subscribe({
      next: (employee) => {
        const formattedStartDate = this.datePipe.transform(employee.data.benchStartDate, 'yyyy-MM-dd');
        const formattedEndDate = this.datePipe.transform(employee.data.benchEndDate, 'yyyy-MM-dd');

        this.empForm.patchValue({
          employeeName: employee.data.employeeName,
          emailId: employee.data.emailId,
          benchStartDate: formattedStartDate,
          benchEndDate: formattedEndDate,
          jobRoleId: employee.data.jobRoleId,
          typeId:employee.data.typeId,
          skills: employee.data.skills
        });
        this.skills = employee.data.skills; 
      },
      error: (err) => {
        console.error('Failed to fetch employee details', err);
      }
    });

    this.empForm.get('skills')?.valueChanges.subscribe(() => {
      this.empForm.valid ? this.enableSubmit() : this.disableSubmit();
    });
  }

  get formControl() {
    return this.empForm.controls;
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
        employeeId: this.employeeId,
        employeeName: this.empForm.value.employeeName,
        emailId: this.empForm.value.emailId,
        benchStartDate: this.empForm.value.benchStartDate,
        benchEndDate: this.empForm.value.benchEndDate,
        jobRoleId: this.empForm.value.jobRoleId,
        skills: this.skills,
        typeId:this.empForm.value.typeId
      };
console.log(this.empForm);
      this.adminService.modifyEmployee(formData).subscribe({
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