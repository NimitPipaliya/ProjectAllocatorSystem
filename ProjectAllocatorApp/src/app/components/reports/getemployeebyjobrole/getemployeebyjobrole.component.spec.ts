import { ComponentFixture, TestBed } from '@angular/core/testing';
 
import { GetemployeebyjobroleComponent } from './getemployeebyjobrole.component';
import { AdminServce } from 'src/app/services/admin.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { JobRole } from 'src/app/models/JobRole';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { Employee } from 'src/app/models/Employee';
 
describe('GetemployeebyjobroleComponent', () => {
  let component: GetemployeebyjobroleComponent;
  let fixture: ComponentFixture<GetemployeebyjobroleComponent>;
  let adminService: AdminServce;
  let adminServiceSpy: jasmine.SpyObj<AdminServce>;
  let router: Router;
 
  beforeEach(() => {
    const spy = jasmine.createSpyObj('AdminServce', ['getJobRoles','getEmployeeBasedOnJobRole']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,RouterTestingModule,FormsModule],
      declarations: [GetemployeebyjobroleComponent],
      providers: [AdminServce]
    });
    fixture = TestBed.createComponent(GetemployeebyjobroleComponent);
    component = fixture.componentInstance;
    adminService = TestBed.inject(AdminServce);
    adminServiceSpy = TestBed.inject(AdminServce) as jasmine.SpyObj<AdminServce>;
    fixture.detectChanges();
    router = TestBed.inject(Router);
  });
 
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should handle error when fetching job roles fails', () => {
    const errorMessage = 'Failed to fetch job roles';
    spyOn(adminService,'getJobRoles').and.returnValue(of({ success:false,data:[],message: errorMessage }));
    spyOn(console,'error');
    component.getJobRoles();

    expect(component.loading).toBe(false);

    expect(console.error).toHaveBeenCalledWith('Failed to fetch jobroles ', errorMessage);

    expect(component.jobRoles).toEqual([]);
  });
  it('should handle error when fetching employees fails', () => {
    const errorMessage = 'Failed to fetch employees';
    const selectedJobRoleId = 1;
    spyOn(adminService,'getEmployeeBasedOnJobRole').and.returnValue(of({ success:false,data:[],message: errorMessage }));
    spyOn(console,'error');
    component.selectedJobRoleId = selectedJobRoleId;

    component.getEmployeeBasedOnJobRole();

    expect(component.loading).toBe(false);

    expect(console.error).toHaveBeenCalledWith('Failed to fetch employees ', errorMessage);

    expect(component.error).toBe('Failed to fetch employees');
    expect(component.errorMessage).toBe(errorMessage);
  });
  it('should handle error when fetchingg employees fails', () => {
    const errorResponse = {error: { message: 'Not Found' } };

    spyOn(adminService,'getEmployeeBasedOnJobRole').and.returnValue(throwError(errorResponse));
    spyOn(console,'error');

    component.selectedJobRoleId = 1; // Example selected job role ID

    component.getEmployeeBasedOnJobRole();

    expect(component.loading).toBe(false);

    expect(console.error).toHaveBeenCalledWith('Error fetching employees : ', errorResponse);

    expect(component.error).toBe('Error fetching employees');
    expect(component.errorMessage).toBe(errorResponse.error.message);
  });
 
  it('should fetch job roles successfully', () => {
    const mockJobRoles: JobRole[] = [
      { jobRoleId: 1, jobRoleName: 'Role 1' },
      { jobRoleId: 2, jobRoleName: 'Role 2' }
    ];
    const mockApiResponse: ApiResponse<JobRole[]> = {
      success: true,
      data: mockJobRoles,
      message: 'Job roles fetched successfully'
    };
 
    spyOn(adminService, 'getJobRoles').and.returnValue(of(mockApiResponse));
 
    component.getJobRoles();
 
    expect(component.loading).toBeFalsy(); // Loading should be false after response
    expect(component.jobRoles).toEqual(mockJobRoles); // Verify jobRoles array
    expect(component.errorMessage).toBe(''); // Error message should be empty
  });
 
  it('should handle error when fetching job roles', () => {
    const errorMessage = 'Error fetching job roles';
    spyOn(adminService, 'getJobRoles').and.returnValue(throwError({ message: errorMessage }));
    spyOn(console,'error')
 
    component.getJobRoles();
 
    expect(component.loading).toBeFalsy(); // Loading should be false after error
    expect(component.jobRoles.length).toBe(0); // jobRoles array should be empty
    expect(console.error).toHaveBeenCalledOnceWith('Error fetching jobroles : ',errorMessage)
  });
  it('should handle error when fetching employees fails', () => {
    const selectedJobRoleId = 1;
    const employees: Employee[] = [
      {
        employeeId: 1, employeeName: 'Employee 1',
        emailId: '',
        benchStartDate: '',
        benchEndDate: '',
        jobRoleId: 0,
        jobRole: {
          jobRoleId:1,
          jobRoleName:"",
 
        },
        typeId: 0,
        allocation: {
          allocations:null,
          employees:null,
          type:"",
          typeId:1
        },
        skills: []
      },
      {
        employeeId: 2, employeeName: 'Employee 2',
        emailId: '',
        benchStartDate: '',
        benchEndDate: '',
        jobRoleId: 0,
        jobRole: {
          jobRoleId:1,
          jobRoleName:"",
 
        },
        typeId: 0,
        allocation: {
          allocations:null,
          employees:null,
          type:"",
          typeId:1
        },
        skills: []
      },
    ];
 
    spyOn(adminService,'getEmployeeBasedOnJobRole').and.returnValue(of({ success: true, data: employees ,message:""}));
    // Set selectedJobRoleId and call the method
    component.selectedJobRoleId = selectedJobRoleId;
    component.getEmployeeBasedOnJobRole();
 
    expect(component.loading).toBe(false);
 
    // Simulate asynchronous behavior with fixture.whenStable()
    fixture.whenStable().then(() => {
      expect(component.employees).toEqual(employees);
      expect(component.loading).toBe(false);
      expect(component.errorMessage).toBe('');
    });
 
    // Trigger change detection
    fixture.detectChanges();
  });
 
  it('should fetch employees based on selected job role ID', () => {
    const selectedJobRoleId = 1;
    const errorMessage = 'Failed to fetch employees';
 
    spyOn(adminService,'getEmployeeBasedOnJobRole').and.returnValue(of({ success: true, data: [],message:""}));
 
    // Set selectedJobRoleId and call the method
    component.selectedJobRoleId = selectedJobRoleId;
    component.getEmployeeBasedOnJobRole();
 
    expect(component.loading).toBe(false);
 
    // Simulate asynchronous behavior with fixture.whenStable()
    fixture.whenStable().then(() => {
      expect(console.error).toHaveBeenCalledWith('Error fetching employees : ', jasmine.any(Error));
      expect(component.employees).toEqual([]);
      expect(component.loading).toBe(true);
      expect(component.errorMessage).toBe(errorMessage);
    });
 
    // Trigger change detection
    fixture.detectChanges();
  });
 
  it('should set errorMessage when no job role is selected', () => {
    spyOn(adminService, 'getEmployeeBasedOnJobRole').and.callThrough(); // Spy on the method and allow it to be called
 
    component.selectedJobRoleId = ''; // Simulate no job role selected
 
    component.getEmployeeBasedOnJobRole();
 
    // Verify that getEmployeeBasedOnJobRole was not called
    expect(adminService.getEmployeeBasedOnJobRole).not.toHaveBeenCalled();
 
    // Assert errorMessage
    expect(component.errorMessage).toBe('*Please Select job role');
  });
});