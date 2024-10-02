import { TestBed } from '@angular/core/testing';

import { AllocatorService } from './allocator.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Employee } from '../models/Employee';
import { AddAllocation } from '../models/Addallocation';
import { UpdateEmpAfterAllocation } from '../models/UpdateEmployeeAfterAllocation';

describe('AllocatorService', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let service: AllocatorService;
  const apiUrl = 'http://localhost:5031/api/Admin/';
  const apiUrl1 = 'http://localhost:5031/api/Allocator/';
  beforeEach(() => {
    
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule],
      providers: [ AllocatorService ]
    });
    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(AllocatorService);
  });
  afterEach(() => {
    httpTestingController.verify(); // Verify that no requests are outstanding
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
  it('should return expected employees from getAllEmployees', () => {
    const mockEmployees: Employee[] = [
      {
        employeeId: 1, employeeName: 'John Doe',
        emailId: '',
        benchStartDate: '',
        benchEndDate: '',
        jobRoleId: 0,
        typeId: 0,
        allocation: {
          allocations: null,
          type: "",
          typeId: 1,
          employees: null,
        },
        skills: [],
        jobRole: {
          jobRoleId:1,
          jobRoleName:"",
        }
      },
      { employeeId: 2, employeeName: 'Jon Doe',
        emailId: '',
        benchStartDate: '',
        benchEndDate: '',
        jobRoleId: 0,
        typeId: 0,
        allocation: {
          allocations: null,
          type: "",
          typeId: 1,
          employees: null,
        },
        skills: [],
        jobRole: {
          jobRoleId:1,
          jobRoleName:"",
        } }
    ];

    const mockApiResponse: ApiResponse<Employee[]> = {
      data: mockEmployees,
      message: 'Employees fetched successfully',
      success: false
    };

    // Make the HTTP GET request
    service.getAllEmployees().subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should return expected employees');
      },
      fail
    );

    // Expect a single request to a particular API URL
    const req = httpTestingController.expectOne(apiUrl + 'GetAllEmployees');
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockApiResponse);
  });

  it('should return employee count without search parameter', () => {
    const mockResponse: ApiResponse<number> = {
      data: 100, // Mocking a count of 100 employees
      message: 'Employee count fetched successfully',
      success: false
    };

    // Make the HTTP GET request without search parameter
    service.getEmployeesCount().subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should return employee count');
      },
      fail
    );

    // Expect a single request to a particular API URL
    const req = httpTestingController.expectOne(apiUrl + 'GetEmployeesCount');
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockResponse);
  });

  it('should return employee count with search parameter', () => {
    const searchQuery = 'engineer';
    const mockResponse: ApiResponse<number> = {
      data: 50, // Mocking a count of 50 employees matching the search query
      message: 'Employee count fetched successfully',
      success: true
    };

    // Make the HTTP GET request with search parameter
    service.getEmployeesCount(searchQuery).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should return employee count');
      },
      fail
    );

    // Expect a single request to a particular API URL with query parameters
    const req = httpTestingController.expectOne(apiUrl + `GetEmployeesCount?search=${searchQuery}`);
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockResponse);
  });
  it('should return employee details by id', () => {
    const employeeId = 1;
    const mockEmployee: Employee = {
      employeeId: employeeId,
      employeeName: 'John Doe'
      // Add other properties as needed
      ,
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
        allocations: null,
        type: "",
        typeId: 1,
        employees: null,
      },
      skills: []
    };

    const mockApiResponse: ApiResponse<Employee> = {
      data: mockEmployee,
      message: 'Employee details fetched successfully',
      success: false
    };

    // Make the HTTP GET request with employeeId parameter
    service.getEmployeeById(employeeId).subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should return employee details');
      },
      fail
    );

    // Expect a single request to a particular API URL with query parameters
    const req = httpTestingController.expectOne(apiUrl + `GetEmployeeById?id=${employeeId}`);
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockApiResponse);
  });
  it('should add allocation successfully', () => {
    const mockAddAllocation: AddAllocation = {
      employeeId: 1,
      startDate: '2024-07-17',
      endDate: '2024-07-31'
      // Add other properties as needed
      ,
      typeId: 0,
      details: '',
      trainingId: 0,
      internalProjectId: 0
    };

    const mockApiResponse: ApiResponse<string> = {
      data: 'Allocation added successfully',
      message: 'Allocation created',
      success: false
    };

    // Make the HTTP POST request with mockAddAllocation as request body
    service.addAllocation(mockAddAllocation).subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should add allocation successfully');
      },
      fail
    );

    // Expect a single request to a particular API URL
    const req = httpTestingController.expectOne(apiUrl1 +`Create`);
    expect(req.request.method).toEqual('POST');
    expect(req.request.body).toEqual(mockAddAllocation);

    // Respond with mock data
    req.flush(mockApiResponse);
  });
  it('should update employee successfully', () => {
    const mockUpdateEmployee: UpdateEmpAfterAllocation = {
      employeeId: 1,
      startDate: '2024-07-17',
      endDate: '2024-07-31'
      // Add other properties as needed
      ,
      typeId: 0
    };

    const mockApiResponse: ApiResponse<string> = {
      data: 'Employee updated successfully',
      message: 'Employee updated',
      success: false
    };

    // Make the HTTP PUT request with mockUpdateEmployee as request body
    service.updateEmployee(mockUpdateEmployee).subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should update employee successfully');
      },
      fail
    );

    // Expect a single request to a particular API URL
    const req = httpTestingController.expectOne(`${apiUrl}UpdateEmployees`);
    expect(req.request.method).toEqual('PUT');
    expect(req.request.body).toEqual(mockUpdateEmployee);

    // Respond with mock data
    req.flush(mockApiResponse);
  });
  it('should fetch employees by pagination without search and sortBy parameters', () => {
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';

    const mockResponse: ApiResponse<Employee[]> = {
      data: [
        {
          employeeId: 3, employeeName: 'Developer One',
          emailId: '',
          benchStartDate: '',
          benchEndDate: '',
          jobRoleId: 0,
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          }, typeId: 0,
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          },
          skills: []
        },
        {
          employeeId: 4, employeeName: 'Developer Two',
          emailId: '',
          benchStartDate: '',
          benchEndDate: '',
          jobRoleId: 0,
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          }, typeId: 0,
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          }, skills: []
        }
      ],
      message: 'Employees fetched successfully',
      success: false
    };

    service.getAllEmployeesByPagination(page, pageSize, sortOrder).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should fetch employees without search and sortBy');
      },
      fail
    );

    const req = httpTestingController.expectOne(`${apiUrl}GetAllEmployeesByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockResponse);
  });

  it('should fetch employees by pagination with search and sortBy parameters', () => {
    const page = 2;
    const pageSize = 20;
    const sortOrder = 'desc';
    const search = 'developer';
    const sortBy = 'name';

    const mockResponse: ApiResponse<Employee[]> = {
      data: [
        {
          employeeId: 3, employeeName: 'Developer One',
          emailId: '',
          benchStartDate: '',
          benchEndDate: '',
          jobRoleId: 0,
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          }, typeId: 0,
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          },
          skills: []
        },
        {
          employeeId: 4, employeeName: 'Developer Two',
          emailId: '',
          benchStartDate: '',
          benchEndDate: '',
          jobRoleId: 0,
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          }, typeId: 0,
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          }, skills: []
        }
      ],
      message: 'Employees fetched successfully',
      success: false
    };

    service.getAllEmployeesByPagination(page, pageSize, sortOrder, search, sortBy).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should fetch employees with search and sortBy');
      },
      fail
    );
    const req = httpTestingController.expectOne(`${apiUrl}GetAllEmployeesByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toEqual('GET');
    req.flush(mockResponse);
  });
});
