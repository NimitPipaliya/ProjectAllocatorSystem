import { Component } from '@angular/core';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SP } from 'src/app/models/sp.model';
import { AdminServce } from 'src/app/services/admin.service';

@Component({
  selector: 'app-getemployeebydaterange',
  templateUrl: './getemployeebydaterange.component.html',
  styleUrls: ['./getemployeebydaterange.component.css']
})
export class GetemployeebydaterangeComponent {
  
  employees: SP[] = [];
  startDate: string = '';
  endDate: string = '';
  loading:boolean=false;  
  errorMessage : string ='';
  noDataMessage: string = '';
  

  constructor(private adminService: AdminServce) { } 

  ngOnInit(): void {
    this.setDefaultDates();
    this.getEmployeesByDateRange();
  }

  setDefaultDates(): void {
    const today = new Date();
    const year = today.getFullYear();
    const month = today.getMonth() + 1; // Month is zero-indexed, so we add 1
    const day = today.getDate();
    this.startDate = `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;
    
  }

  getEmployeesByDateRange():void{
    this.loading = true;
    this.errorMessage = ''; 
    if (!this.startDate) {
      this.errorMessage = '*Please Select start date';
      return;
    }
   
    this.adminService.getEmployeeBasedOnDateRange(this.startDate, this.endDate).subscribe({
      next:(response:ApiResponse<SP[]>)=>{
        if(response.success){
          this.noDataMessage = this.employees.length === 0 ? 'No employees found for the selected date range.' : '';
          
          this.employees = response.data;
          
         
          console.log(this.employees);
        
        }else{
          console.error('Failed to fetch employee',response.message);
          this.errorMessage = response.message;

        }
        this.loading = false;
      },
      error :(error)=>{
        console.error('Error fetching employee.',error);
        this.errorMessage = error.error.message;
        this.loading = false;
      }
    });
  }

}
