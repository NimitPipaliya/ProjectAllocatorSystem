import { Component } from '@angular/core';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Employee } from 'src/app/models/Employee';
import { JobRole } from 'src/app/models/JobRole.model';
import { AdminServce } from 'src/app/services/admin.service';

@Component({
  selector: 'app-getemployeebyjobrole',
  templateUrl: './getemployeebyjobrole.component.html',
  styleUrls: ['./getemployeebyjobrole.component.css']
})
export class GetemployeebyjobroleComponent {
  loading:boolean=false;
  error: string = ''; 
  employees: Employee[] = [];
  jobRoles: JobRole[] = [];
  errorMessage : string ='';
  noDataMessage: string = '';
  selectedJobRoleId: any = ''; // Initialize with a default value, or null if your API supports it

  constructor(private adminService: AdminServce) { }

  ngOnInit() {
    this.getJobRoles();
    this.selectedJobRoleId = '';
    
  }

 
    getJobRoles():void{
      this.loading = true;
      this.adminService.getJobRoles().subscribe({
        next:(response: ApiResponse<JobRole[]>) =>{
          if(response.success){
            this.jobRoles = response.data;
          }
          else{
            console.error('Failed to fetch jobroles ', response.message);
          }
          this.loading = false;
        },error:(error)=>{
          console.error('Error fetching jobroles : ',error.message);
          this.loading = false;
        }
      });
    }

    getEmployeeBasedOnJobRole():void{
      this.loading = true;
      this.errorMessage = ''; 
      if (!this.selectedJobRoleId) {
        this.errorMessage = '*Please Select job role';
        return;
      }
      this.adminService.getEmployeeBasedOnJobRole(this.selectedJobRoleId).subscribe({
        next:(response: ApiResponse<Employee[]>) =>{
          if(response.success){
            this.employees = response.data;
          }
          else{
            console.error('Failed to fetch employees ', response.message);
            this.error = "Failed to fetch employees";
            this.errorMessage = response.message;
          }
          this.loading = false;
        },
        error:(error)=>{
          console.error('Error fetching employees : ',error);
          this.error = "Error fetching employees";
          this.errorMessage = error.error.message;
          this.loading = false;
        }
      });
    }






  }
  
  
  


