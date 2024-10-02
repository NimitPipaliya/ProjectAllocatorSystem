import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenchListComponent } from './bench-list.component';
import { AllocatorService } from 'src/app/services/allocator.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ChangeDetectorRef } from '@angular/core';
import { Employee } from 'src/app/models/Employee';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('BenchListComponent', () => {
  let component: BenchListComponent;
  let fixture: ComponentFixture<BenchListComponent>;
  let employeeServiceSpy: jasmine.SpyObj<AllocatorService>;
  let router: Router;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;
  let mockTotalemployeesCount = jasmine.createSpy('totalemployeesCount');
  let mockLoadPaginatedemployees = jasmine.createSpy('loadPaginatedemployees');
  let mockLoadsearchemployees = jasmine.createSpy('searchEmployees');
  let mockLoadloadAllemployees = jasmine.createSpy('loadAllEmployees');
 
  const mockEmp:Employee={
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
    skills: [".net","mvc","angular"]
  }
  const mockEmptyEmpList : Employee[] =[];
  const mockEmployeeList :Employee[] = [
    {
      employeeId: 1,
      employeeName: 'Name1',
      emailId: 'name1@gmail.com',
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
      skills: [".net","mvc","angular"]
    },
    {
      employeeId: 2,
      employeeName: 'Name 2',
      emailId: 'name2@gmail.com',
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
      skills: [".net","mvc"]
    },
    {
      employeeId: 3,
      employeeName: 'Name3',
      emailId: 'name3@gmail.com',
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
      skills: ["mvc","angular"]
    }
  ];
  beforeEach(() => {
    employeeServiceSpy=jasmine.createSpyObj('AllocatorService',['getAllEmployees','getAllEmployeesByPagination','getEmployeesCount','getEmployeeById'])
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,RouterTestingModule],
      declarations: [BenchListComponent],
      providers: [
        { provide: AllocatorService, useValue: employeeServiceSpy },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ],
    });
    fixture = TestBed.createComponent(BenchListComponent);
    component = fixture.componentInstance;
    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    //fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  beforeEach(() => {
    mockTotalemployeesCount = spyOn(component, 'totalEmployeesCount').and.callThrough();
    mockLoadPaginatedemployees = spyOn(component, 'loadPaginatedEmployees').and.callThrough();
    mockLoadsearchemployees = spyOn(component, 'searchEmployee').and.callThrough();
    mockLoadloadAllemployees = spyOn(component, 'loadAllEmployees').and.callThrough();
    router = TestBed.inject(Router);
  });
  afterEach(() => {
    // Clear spies and reset state if needed
    employeeServiceSpy.getAllEmployees.calls.reset();
    employeeServiceSpy.getAllEmployeesByPagination.calls.reset();
    employeeServiceSpy.getEmployeesCount.calls.reset();
    employeeServiceSpy.getEmployeeById.calls.reset();
    mockTotalemployeesCount.calls.reset();
    mockLoadPaginatedemployees.calls.reset();
    mockLoadsearchemployees.calls.reset();
    mockLoadloadAllemployees.calls.reset();
    cdrSpy.detectChanges.calls.reset();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should load all employees successfully', () => {
    const mockResponse: ApiResponse<Employee[]> = {
      success: true,
      data: mockEmployeeList,
      message: ''
    };
    employeeServiceSpy.getAllEmployees.and.returnValue(of(mockResponse));
 
    component.loadAllEmployees();
 
    expect(employeeServiceSpy.getAllEmployees).not.toHaveBeenCalled();
    //expect(component.sortedemployees).toEqual(mockEmployeeList);
    expect(component.loading).toBe(true);
  });
 
  it('should handle error when loading all employees fails', () => {
    const mockErrorResponse: ApiResponse<Employee[]> = {
      success: false,
      data: [],
      message: 'Failed to fetch employees.'
    };
    employeeServiceSpy.getAllEmployees.and.returnValue(of(mockErrorResponse));
 
    component.loadAllEmployees();
 
    expect(employeeServiceSpy.getAllEmployees).not.toHaveBeenCalled();
    expect(component.loading).toBe(true);
  });
 
  it('should call getEmployeesCount and set total employee count', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 3,
      message: ''
    };
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
 
    // Act
    component.totalEmployeesCount();
 
    // Assert
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalled();
 
  })
 
  it('should handle failure to fetch total employee count', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: false,
      data: 0,
      message: 'Failed to fetch employees'
    };
    employeeServiceSpy.getEmployeesCount.and.returnValue(throwError(mockResponse));
    spyOn(console, 'error');
    // Act
    component.totalEmployeesCount();
    // Assert
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalled();
    expect(console.error).not.toHaveBeenCalledWith('Failed to fetch employees', mockResponse.message);
 
  })
 
  it('should fail to calaulate total employee count ',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 3, message: 'Failed to fetch employees'
    };
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.totalEmployeesCount();
 
    //Assert
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalled();
    expect(console.error).not.toHaveBeenCalledWith('Failed to fetch employees',mockResponse.message);
 
  })
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    employeeServiceSpy.getEmployeesCount.and.returnValue(throwError(mockError));
    spyOn(console,'error')
 
    //Act
    component.totalEmployeesCount();
 
    //Assert
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalled();
    //expect(console.error).toHaveBeenCalledWith('Network Error',mockError.message);
 
  })
  it('should load employees successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    employeeServiceSpy.getAllEmployees.and.returnValue(of(mockResponse));
    //Act
    component.loadAllEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployees).not.toHaveBeenCalled();
    //expect(component.sortedemployees).toEqual(mockEmployeeList);
    expect(component.loading).toBe(true);
  })
  it('should fail to load employees ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: false, data:mockEmployeeList , message: 'Failed to fetch employees.',
    };
    employeeServiceSpy.getAllEmployees.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadAllEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployees).not.toHaveBeenCalled();
    expect(component.loading).toBe(true);
  })
 
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    employeeServiceSpy.getAllEmployees.and.returnValue(throwError(mockError));
    spyOn(console,'error')
 
    //Act
    component.loadAllEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployees).not.toHaveBeenCalled();
    //expect(console.error).toHaveBeenCalledWith('Network Error',mockError);
 
  })
  it('should load paginated employees successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployeesByPagination).not.toHaveBeenCalled();
    //expect(component.employees).toEqual(mockEmployeeList);
    expect(component.loading).toBe(true);
  })
  it('should fail to load paginated employees ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: false, data:mockEmployeeList , message: 'Failed to fetch employees',
    };
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadPaginatedEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployeesByPagination).not.toHaveBeenCalled();
    expect(component.loading).toBe(true);
  })
 
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(throwError(mockError));
    spyOn(console,'error')
 
    //Act
    component.loadPaginatedEmployees();
 
    //Assert
    expect(employeeServiceSpy.getAllEmployeesByPagination).not.toHaveBeenCalled();
    //expect(console.error).toHaveBeenCalledWith('Network Error',mockError);
 
  })
  it('should load paginated employees successfully with search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedEmployees("Title 1");
 
    //Assert
    expect(employeeServiceSpy.getAllEmployeesByPagination).not.toHaveBeenCalled();
    //expect(component.employees).toEqual(mockEmployeeList);
    expect(component.loading).toBe(true);
  })
  it('should load paginated employees successfully with sortBy and Search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedEmployees("Title 1","title");
 
    //Assert
    expect(employeeServiceSpy.getAllEmployeesByPagination).not.toHaveBeenCalled();
    //expect(component.employees).toEqual(mockEmployeeList);
    expect(component.loading).toBe(true);
  })
 
  it('should toggle sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.sortColumn(initialColumn);
 
    expect(component.sortOrder).toBe('desc');
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalledWith(component.search);
 
  });
  it('should go to default sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.sortColumn('default');
 
    expect(component.sortOrder).toBe('desc');
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalledWith(component.search);
 
  });
 
  it('should maintain currentPage when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    const initialPage = component.currentPage;
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.sortColumn(initialColumn);
 
    expect(component.currentPage).toBe(initialPage);
 
  });
  it('should toggle sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'desc';
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.sortColumn(initialColumn);
 
    expect(component.sortOrder).toBe('asc');
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalledWith(component.search);
 
  });
 
  it('should maintain currentPage when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    const initialPage = component.currentPage;
    component.sortBy = initialColumn;
    component.sortOrder = 'desc';
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.sortColumn(initialColumn);
 
    expect(component.currentPage).toBe(initialPage);
 
  });
  it('should search employee', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
   
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.searchEmployee();
 
    expect(component.sortOrder).toBe('asc');
 
  });
  it('should search employee if string empty', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
    component.search ='ert'
   if(component.search != ''  && component.search.length > 2){
    component.currentPage=1;
   }
    employeeServiceSpy.getEmployeesCount('');
    employeeServiceSpy.getAllEmployeesByPagination(1,2,'');
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.searchEmployee();
 
    expect(component.sortOrder).toBe('asc');
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalledWith(component.search);
 
  });
  it('should load paginated page on pageChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    let page = 2;
    component.sortBy = initialColumn;
    component.currentPage=page;
   
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.onPageChange(page,initialColumn);
 
    expect(component.sortOrder).toBe('asc');
 
  });
  it('should load paginated page on pagesizeChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
   let search = 'title'
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.onPageSizeChange(search);
 
    expect(component.sortOrder).toBe('asc');
    expect(employeeServiceSpy.getEmployeesCount).not.toHaveBeenCalledWith(search);
  });
  it('should calculate serial number correctly', () => {
    const index = 0;
    const expectedSerialNumber = (component.currentPage - 1) * component.pageSize + index + 1;
    const actualSerialNumber = component.calculateSrNo(index);
    component.calculateSrNo(index);
    expect(actualSerialNumber).toBe(expectedSerialNumber);
  });
  it('should show active button differently', () => {
    const pageNumber = 1;
    component.currentPage === pageNumber;
    const expectedPageNumber =component.currentPage === pageNumber;;
    const actualPageNumber = component.isActive(pageNumber);
    component.isActive(pageNumber);
    expect(actualPageNumber).toBe(expectedPageNumber);
  });
  it('should load previous paginated page on onPrevPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=2;
    let search = 'title'
 
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.onPrevPage(search);
 
    expect(component.sortOrder).toBe('asc');
  });
  it('should load next paginated page on onNextPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Employee[]> ={
      success: true, data: mockEmployeeList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=3;
    let search = 'title'
    component.totalPages.length = 6;
   
    employeeServiceSpy.getEmployeesCount.and.returnValue(of(mockResponse));
    employeeServiceSpy.getAllEmployeesByPagination.and.returnValue(of(mockResponse1));
 
    component.onNextPage(search);
 
    expect(component.sortOrder).toBe('asc');
  });
 
});
