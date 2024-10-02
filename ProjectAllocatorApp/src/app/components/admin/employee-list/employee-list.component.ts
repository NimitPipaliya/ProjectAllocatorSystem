import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Employee } from 'src/app/models/Employee';
import { AdminServce } from 'src/app/services/admin.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit{

  employees: Employee[] = [];
  sortedemployees: Employee[] = [];
  totalemployees!: number;
  pageSize = 4;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'asc';
  search: string = '';
  sortBy = '';
  

  constructor(private authService: AuthService, private adminService: AdminServce, private cdr: ChangeDetectorRef, private route: Router) { }

  ngOnInit(): void {
    this.searchEmployee();
    //this.loadAllEmployees();
  }
  totalEmployeesCount(search?: string) {
    this.adminService.getEmployeesCount(search)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalemployees = response.data;
            console.log(this.totalemployees);
            this.calculateTotalPages();

          }
          else {
            console.error('Failed to fetch employees', response.message);
          }
        },
        error: (error => {
          console.error('Failed to fetch employees', error.message);
          this.loading = false;
        })
      });
  }

  loadAllEmployees(): void {
    // this.loading = true;
    this.adminService.getAllEmployees().subscribe({
      next: (response: ApiResponse<Employee[]>) => {
        if (response.success) {
          console.log(response.data);
          this.sortedemployees = response.data;
        }
        else {
          console.error('Failed to fetch employees.', response.message);
          alert('Failed to fetch employees.');
        }
        // this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching employees.', error);
        // this.loading = false;
      }
    });
  }

  loadPaginatedEmployees(search?: string, sortBy?: string) {
    // this.loading = true;
    this.adminService.getAllEmployeesByPagination(this.currentPage, this.pageSize, this.sortOrder, search, sortBy)
      .subscribe({
        next: (response: ApiResponse<Employee[]>) => {
          if (response.success) {
            this.employees = response.data;
            console.log(response.data);
          }
          else {
            console.error('Failed to fetch employees', response.message);
          }
          // this.loading = false;

        },
        error: (error => {
          console.error('Failed to fetch employees', error);
          // this.loading = false;
        })
      });
  }
 
  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalemployees / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  sortColumn(columnName: string) {
    if (this.sortBy === columnName ) {
      
      if(this.sortOrder=='desc')
        {
          this.currentPage = this.currentPage;
          this.totalEmployeesCount(this.search);
          this.loadPaginatedEmployees(this.search,columnName);
          this.sortOrder='asc';
        }
        else{
          this.currentPage = this.currentPage;
          this.totalEmployeesCount(this.search);
          this.loadPaginatedEmployees(this.search,columnName);
          this.sortOrder='desc';
        }
    } 
    else {
      this.sortBy = columnName;
      this.sortOrder = 'default';
      this.totalEmployeesCount(this.search);
      this.loadPaginatedEmployees(this.search,columnName);
      this.sortOrder = 'desc'
    }
  }

  searchEmployee() {
    // this.currentPage = 1;
    // this.loadPaginatedBooks(this.search,this.sortBy);
    // this.totalBooksCount(this.search);

    if (this.search != ''  && this.search.length > 2) {
      this.currentPage = 1;
      this.totalEmployeesCount(this.search);
      this.loadPaginatedEmployees(this.search);
    }
    else {
      this.currentPage = 1;
      this.totalEmployeesCount();
      this.loadPaginatedEmployees('');
    }
}
onPageChange(page: number,search?:string) {
  this.currentPage = page;
  this.loadPaginatedEmployees(search,this.sortBy);
}
onPageSizeChange(search?: string) {
  this.currentPage = 1; 
  this.loadPaginatedEmployees(search, this.sortBy);
  this.totalEmployeesCount(search);
}
calculateSrNo(index: number): number {
  return (this.currentPage - 1) * this.pageSize + index + 1;
}
isActive(pageNumber: number): boolean {
  return this.currentPage === pageNumber;
}
deleteEmployee(employeeId: number) {
  if (confirm('Are you sure you want to delete this employee?')) {
    this.adminService.deleteEmployee(employeeId).subscribe(() => {
      this.totalemployees --;
      const pages = Math.ceil(this.totalemployees / this.pageSize);
      if(this.currentPage>pages){
        this.currentPage=pages;
      }
      this.loadPaginatedEmployees(this.search,this.sortBy); 
      this.totalEmployeesCount(this.search);     
    });
  }
}

  onPrevPage(search?: string) {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadPaginatedEmployees(search, this.sortBy);
    }
  }

  onNextPage(search?: string) {
    if (this.currentPage < this.totalPages.length) {
      this.currentPage++;
      this.loadPaginatedEmployees(search, this.sortBy);
    }
  }
}

