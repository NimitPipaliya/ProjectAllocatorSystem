import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { BenchAllocation } from 'src/app/models/BenchAllocation';
import { Employee } from 'src/app/models/Employee';
import { Employees } from 'src/app/models/Employees';
import { AuthService } from 'src/app/services/auth.service';
import { ManagerService } from 'src/app/services/manager.service';

@Component({
  selector: 'app-bench-list',
  templateUrl: './bench-list.component.html',
  styleUrls: ['./bench-list.component.css']
})
export class BenchListComponent implements OnInit {
  employees: Employees[] = [];
  sortedemployees: Employees[] = [];
  totalemployees!: number;
  pageSize = 4;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'asc';
  search: string = '';
  sortBy = '';
  constructor(private authService: AuthService, private managerService: ManagerService, private cdr: ChangeDetectorRef, private route: Router) { }

  ngOnInit(): void {
    this.searchEmployee();
    this.loadAllEmployees();

  }
  
  totalEmployeesCount(search?: string) {
    this.managerService.getEmployeesCount(search)
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
          console.error('Failed to fetch employees', error);
          this.loading = false;
        })
      });
  }
 
  loadAllEmployees(): void {
    // this.loading = true;
    this.managerService.getAllEmployees().subscribe({
      next: (response: ApiResponse<Employees[]>) => {
        if (response.success) {
          console.log(response.data);
          this.employees = response.data;//change
          this.fetchTrainingIdsForEmployees();
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

  fetchTrainingIdsForEmployees(): void {
    // Assuming this.sortedemployees is populated correctly
    this.employees.forEach(employee => {
      this.managerService.getEmployeeById(employee.employeeId).subscribe({
        next: (response: ApiResponse<BenchAllocation>) => {
          if (response.success) {
            employee.trainingId = response.data.trainingId;
            employee.internalProjectId = response.data.internalProjectId;
            console.log(`Fetched trainingId for employee ${employee.employeeId}: ${employee.trainingId} ${employee.internalProjectId}`);
          } else {
            console.error(`Failed to fetch employee ${employee.employeeId}: ${response.message}`);
          }
          this.loading = false; // Ensure loading flag is appropriately managed
        },
        error: (error) => {
          console.error(`Error fetching trainingId for employee ${employee.employeeId}:`, error);
          this.loading = false; // Ensure loading flag is appropriately managed
        }
      });
    });
  }
 
  loadPaginatedEmployees(search?: string, sortBy?: string) {
    // this.loading = true;
    this.managerService.getAllEmployeesByPagination(this.currentPage, this.pageSize, this.sortOrder, search, sortBy)
      .subscribe({
        next: (response: ApiResponse<Employees[]>) => {
          if (response.success) {
            this.employees = response.data;
            this.fetchTrainingIdsForEmployees();
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
