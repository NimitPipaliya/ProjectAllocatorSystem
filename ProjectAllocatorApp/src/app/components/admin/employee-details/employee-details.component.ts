import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Employee } from 'src/app/models/Employee';
import { AdminServce } from 'src/app/services/admin.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-employee-details',
  templateUrl: './employee-details.component.html',
  styleUrls: ['./employee-details.component.css']
})
export class EmployeeDetailsComponent implements OnInit {
  employeeId:number|undefined;
  isAuthenticated: boolean = false;
  username:string |null|undefined;

  employee:Employee={
    employeeId: 0,
    employeeName: '',
    emailId: '',
    benchStartDate: '',
    benchEndDate: '',
    jobRoleId: 0,
    jobRole: {
      jobRoleId: 0,
      jobRoleName: ''
    },
    typeId: 0,
    allocation:{
      typeId: 0,
      type: '',
      allocations: null,
      employees: null
    },
    skills: []
  }
  constructor(private adminService:AdminServce,private route:ActivatedRoute,private router:Router,private authService:AuthService,private cdr:ChangeDetectorRef) { }

  ngOnInit():void{
     const employeeId = Number(this.route.snapshot.paramMap.get('employeeId'));
     this.adminService.getEmployeeById(employeeId).subscribe({
       next: (response) => {
         if (response.success) {
           this.employee = response.data;
         } else {
           console.error('Failed to fetch employee.', response.message);
         }
       },
       error: (error) => {
         console.error('Failed to fetch employee.', error);
       },
     });
  }
  deleteBook(bookId: number) {
    if (confirm('Are you sure you want to delete this book?')) {
      this.adminService.deleteEmployee(bookId).subscribe(() => {
        this.router.navigate(['/employeeList']);
      });
    }
  }

}
