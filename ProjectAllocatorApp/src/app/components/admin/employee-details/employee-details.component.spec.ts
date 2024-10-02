import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeDetailsComponent } from './employee-details.component';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Employee } from 'src/app/models/Employee';
import { AdminServce } from 'src/app/services/admin.service';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeListComponent } from '../employee-list/employee-list.component';
import { DatePipe } from '@angular/common';

describe('EmployeeDetailsComponent', () => {
  let component: EmployeeDetailsComponent;
  let fixture: ComponentFixture<EmployeeDetailsComponent>;
  let adminSpy: jasmine.SpyObj<AdminServce>;
  let router: Router;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockEmp: Employee = {
    employeeId: 1,
    employeeName: 'Name',
    emailId: 'name@gmail.com',
    benchStartDate: '2025-01-01',
    benchEndDate: '',
    jobRoleId: 1,
    jobRole: {
      jobRoleId: 1,
      jobRoleName: 'Developer'
    },
    typeId: 1,
    allocation: {
      typeId: 1,
      type: 'Bench',
      allocations: null,
      employees: null
    },
    skills: [".net", "mvc", "angular"]
  }


  beforeEach(() => {
    adminSpy = jasmine.createSpyObj('AdminServce', ['getEmployeeById','deleteEmployee']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    const mockResponse: ApiResponse<Employee> = { success: true, data: mockEmp, message: 'success' };
    adminSpy.getEmployeeById.and.returnValue(of(mockResponse));
    TestBed.configureTestingModule({
      imports:[RouterTestingModule,HttpClientTestingModule],
      declarations: [EmployeeDetailsComponent],
      providers: [
        { provide: AdminServce, useValue: adminSpy },
        { provide:Router,use:mockRouter},
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => '1' 
              }
            }
          }
        }
      ]
    });
    fixture = TestBed.createComponent(EmployeeDetailsComponent);
    component = fixture.componentInstance;
    adminSpy = TestBed.inject(AdminServce) as jasmine.SpyObj<AdminServce>;
    router = TestBed.inject(Router);
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should fetch employee details on init', () => {
    const mockResponse: ApiResponse<Employee> = { success: true, data: mockEmp, message: 'Success' };
    adminSpy.getEmployeeById.and.returnValue(of(mockResponse));

    fixture.detectChanges(); 
    expect(adminSpy.getEmployeeById).toHaveBeenCalled();
    expect(component.employee).toEqual(mockEmp);
  });   
  it('should handle error when fetching employee details', () => {
    const mockError = new Error('Failed to fetch employee details');
    adminSpy.getEmployeeById.and.returnValue(throwError(mockError));

    fixture.detectChanges(); 

    expect(adminSpy.getEmployeeById).toHaveBeenCalled();
    expect(component.employee).toEqual({
      employeeId: 0,
      employeeName: '',
      emailId: '',
      benchStartDate: '',
      benchEndDate: '',
      jobRoleId: 0,
      jobRole: { jobRoleId: 0, jobRoleName: '' },
      typeId: 0,
      allocation: { typeId: 0, type: '', allocations: null, employees: null },
      skills: []
    }); 
  });
  it('should handle error when fetching employee details', () => {
  const mockResponse: ApiResponse<Employee> = { success: false, data: mockEmp, message: 'Employee not found' };
  spyOn(console, 'error');
  adminSpy.getEmployeeById.and.returnValue(of(mockResponse));

    fixture.detectChanges();

    expect(adminSpy.getEmployeeById).toHaveBeenCalled();
    expect(component.employee).toEqual({
      employeeId: 0,
      employeeName: '',
      emailId: '',
      benchStartDate: '',
      benchEndDate: '',
      jobRoleId: 0,
      jobRole: { jobRoleId: 0, jobRoleName: '' },
      typeId: 0,
      allocation: { typeId: 0, type: '', allocations: null, employees: null },
      skills: []
    }); 
    expect(console.error).toHaveBeenCalledWith('Failed to fetch employee.', 'Employee not found');
  });
  it('should delete employee', () => {
    const employeeId = 1;
    spyOn(router, 'navigate');
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Deleted' };

    adminSpy.deleteEmployee.and.returnValue(of(mockResponse));

    spyOn(window, 'confirm').and.returnValue(true);

    component.deleteBook(employeeId);

    expect(adminSpy.deleteEmployee).toHaveBeenCalledWith(employeeId);
    expect(router.navigate).toHaveBeenCalledWith(['/employeeList']);
  });
});
