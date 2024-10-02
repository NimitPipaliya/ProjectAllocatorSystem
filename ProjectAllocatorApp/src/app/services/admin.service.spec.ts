/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AdminServce } from './admin.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SP } from '../models/sp.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Employee } from '../models/Employee';
import { AddEmployee } from '../models/AddEmployee';
import { JobRole } from '../models/JobRole';
import { EditEmployee } from '../models/EditEmployee';

describe('Service: Admin', () => {
  let service: AdminServce;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5031/api/Admin/';
  const mockApiResponse: ApiResponse<SP[]> = {
    success: true,
    data: [
      {
        employeeId: 1,
        employeeName: 'Employee 1',
        typeId: 1,
        benchStartDate:'2003-01-01',
        benchEndDate:'2024-01-01',
        
        trainingId:1,
        trainingName:'MVC',
        trainingDescription:'asdfg',
        projectName:'qwe',
        projectDescription:'sdfg',
       

      },
      {
        employeeId: 1,
        employeeName: 'Employee 1',
        typeId: 1,
        benchStartDate:'2003-01-01',
        benchEndDate:'2024-01-01',
        
        trainingId:1,
        trainingName:'MVC',
        trainingDescription:'asdfg',
        projectName:'qwe',
        projectDescription:'sdfg',
      }
    ],
    message: ''
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AdminServce]
    });
    service = TestBed.inject(AdminServce);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });


  it('should ...', inject([AdminServce], (service: AdminServce) => {
    expect(service).toBeTruthy();
  }));
  it('should retrieve all employees from the API via GET', () => {
    const mockEmployees: ApiResponse<Employee[]> = {
      data: [
        {
          employeeId: 1,
          employeeName: 'Employee 1',
          typeId: 1,
          benchStartDate: '2003-01-01',
          benchEndDate: '2024-01-01',
          emailId: '',
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          },
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          },
          skills: [],
          jobRoleId: 0
        },
        {
          employeeId: 2,
          employeeName: 'Employee 2',
          typeId: 1,
          benchStartDate: '2003-01-02',
          benchEndDate: '2024-01-02',
          emailId: '',
          jobRole: {
            jobRoleId: 1,
            jobRoleName: "",
          },
          allocation: {
            allocations: null,
            type: "",
            typeId: 1,
            employees: null,
          },
          skills: [],
          jobRoleId: 0
        }
      ],
      success: true,
      message: 'Employees retrieved successfully'
    };

    service.getAllEmployees().subscribe(response => {
      expect(response).toEqual(mockEmployees);
    });

    const req = httpMock.expectOne(apiUrl + 'GetAllEmployees');
    expect(req.request.method).toBe('GET');
    req.flush(mockEmployees);
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
 
    // Make the HTTP GET request without search and sortBy parameters
    service.getAllEmployeesByPagination(page, pageSize, sortOrder).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should fetch employees without search and sortBy');
      },
      fail
    );
 
    // Expect a single request to a particular API URL
    const req = httpMock.expectOne(`${apiUrl}GetAllEmployeesByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
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
 
    // Make the HTTP GET request with search and sortBy parameters
    service.getAllEmployeesByPagination(page, pageSize, sortOrder, search, sortBy).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should fetch employees with search and sortBy');
      },
      fail
    );
 
    // Expect a single request to a particular API URL with query parameters
    const req = httpMock.expectOne(`${apiUrl}GetAllEmployeesByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toEqual('GET');
 
    // Respond with mock data
    req.flush(mockResponse);
  });
  it('should return employee count when search parameter is provided', () => {
    const search = 'John';
    const mockResponse: ApiResponse<number> = {
      data: 10, success: true,
      message: ''
    }; // Adjust based on your ApiResponse structure

    service.getEmployeesCount(search).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetEmployeesCount?search=${search}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should return employee count when search parameter is not provided', () => {
    const mockResponse: ApiResponse<number> = {
      data: 20, success: true,
      message: ''
    }; // Adjust based on your ApiResponse structure

    service.getEmployeesCount().subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetEmployeesCount`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should return employee data based on date range', () => {
    const startDate = '2024-01-01';
    const endDate = '2024-01-31';
    const mockResponse: ApiResponse<SP[]> = {
        data: [
          {
            employeeId: 3, employeeName: 'Developer One',
            typeId: 1,
            benchStartDate: '',
            benchEndDate: '',
            trainingId: 0,
            trainingName: '',
            trainingDescription: '',
            projectName: '',
            projectDescription: ''
          },
          {
            employeeId: 4, employeeName: 'Developer Two',
            benchStartDate: '',
            benchEndDate: '',
            typeId: 0,
            trainingId: 0,
            trainingName: '',
            trainingDescription: '',
            projectName: '',
            projectDescription: ''
          }
        ],
      success: false,
      message: ''
    };

    service.getEmployeeBasedOnDateRange(startDate, endDate).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetEmployeeData?startDate=${startDate}&enddate=${endDate}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should return employees based on job role', () => {
    const jobroleId = 1;
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
      success: true,
      message: ''
    };

    service.getEmployeeBasedOnJobRole(jobroleId).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetEmployeesByJobRoleAndType?jobRoleId=${jobroleId}&typeId=1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should add an employee and return success message', () => {
    const addEmployee: AddEmployee = {
      employeeName: 'John Doe',
      emailId: '',
      benchStartDate: '',
      benchEndDate: '',
      jobRoleId: 0,
      skills: []
    };

    const mockResponse: ApiResponse<string> = {
      data: 'Employee added successfully',
      success: true,
      message: ''
    };

    service.addEmployee(addEmployee).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}AddEmployee`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(addEmployee);
    req.flush(mockResponse);
  });
  it('should return job roles', () => {
    const mockResponse: ApiResponse<JobRole[]> = {
      data: [
        { jobRoleId: 1, jobRoleName: 'Developer' }, // Adjust based on your JobRole structure
        { jobRoleId: 2, jobRoleName: 'Tester' }
      ],
      success: true,
      message: ''
    };

    service.getJobRoles().subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetAllJobRoles`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should return employee data by id', () => {
    const employeeId = 1;
    const mockEmployee: ApiResponse<Employee> = {
      data: {
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
      success: true,
      message: ''
    };

    service.getEmployeeById(employeeId).subscribe(response => {
      expect(response).toEqual(mockEmployee);
    });

    const req = httpMock.expectOne(`${apiUrl}GetEmployeeById?id=${employeeId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockEmployee);
  });
  it('should modify an employee and return a success message', () => {
    const editEmployee: EditEmployee = {
      employeeId: 1,
      employeeName: '',
      emailId: '',
      benchStartDate: '',
      benchEndDate: '',
      jobRoleId: 0,
      typeId: 0,
      skills: []
    };

    const mockResponse: ApiResponse<string> = {
      data: 'Employee updated successfully',
      success: true,
      message: ''
    };

    service.modifyEmployee(editEmployee).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}UpdateEmployee`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(editEmployee);
    req.flush(mockResponse);
  });
  it('should delete an employee and return a success message', () => {
    const employeeId = 1;
    const mockResponse: ApiResponse<string> = {
      data: 'Employee removed successfully',
      success: true,
      message: ''
    };

    service.deleteEmployee(employeeId).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}RemoveEmployee?id=${employeeId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });
  it('should retrieve employees with pagination and sorting when search is null and sortBy is provided', () => {
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const sortBy = 'name';
    const search = undefined; // search is null in this test case

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
      success: true,
      message: ''
    };

    service.getAllEmployeesByPagination(page, pageSize, sortOrder, search, sortBy).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetAllEmployeesByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should retrievee employees with pagination and sorting when search is null and sortBy is provided', () => {
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const sortBy = undefined;
    const search = "dev"; // search is null in this test case

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
      success: true,
      message: ''
    };

    service.getAllEmployeesByPagination(page, pageSize, sortOrder, search, sortBy).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${apiUrl}GetAllEmployeesByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
