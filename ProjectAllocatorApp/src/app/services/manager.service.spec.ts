import { TestBed } from '@angular/core/testing';

import { ManagerService } from './manager.service';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AllocatorService } from './allocator.service';
import { Employees } from '../models/Employees';
import { ApiResponse } from '../models/ApiResponse{T}';
import { BenchAllocation } from '../models/BenchAllocation';

describe('ManagerService', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let service: ManagerService;
  const apiUrl = 'http://localhost:5031/api/Admin/';
  const apiUrl1 = 'http://localhost:5031/api/Manager/';


  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule],
      providers: [ AllocatorService ]
    });
    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(ManagerService);
  });
  afterEach(() => {
    httpTestingController.verify(); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return expected employees from getAllEmployees', () => {
    const mockEmployees: Employees[] = [
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
        skills: null,
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
        skills: null,
        jobRole: {
          jobRoleId:1,
          jobRoleName:"",
        } }
    ];

    const mockApiResponse: ApiResponse<Employees[]> = {
      data: mockEmployees,
      message: 'Employees fetched successfully',
      success: false
    };

    service.getAllEmployees().subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should return expected employees');
      },
      fail
    );

    const req = httpTestingController.expectOne(apiUrl + 'GetAllEmployees');
    expect(req.request.method).toEqual('GET');

    req.flush(mockApiResponse);
  });

  it('should return employee count without search parameter', () => {
    const mockResponse: ApiResponse<number> = {
      data: 100, 
      message: 'Employee count fetched successfully',
      success: false
    };

    service.getEmployeesCount().subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should return employee count');
      },
      fail
    );

    const req = httpTestingController.expectOne(apiUrl + 'GetEmployeesCount');
    expect(req.request.method).toEqual('GET');

    req.flush(mockResponse);
  });

  it('should return employee count with search parameter', () => {
    const searchQuery = 'engineer';
    const mockResponse: ApiResponse<number> = {
      data: 50, 
      message: 'Employee count fetched successfully',
      success: true
    };

    service.getEmployeesCount(searchQuery).subscribe(
      response => {
        expect(response).toEqual(mockResponse, 'should return employee count');
      },
      fail
    );

    const req = httpTestingController.expectOne(apiUrl + `GetEmployeesCount?search=${searchQuery}`);
    expect(req.request.method).toEqual('GET');

    // Respond with mock data
    req.flush(mockResponse);
  });
  it('should return employee details by id', () => {
    const employeeId = 1;
   const benchAllocation : BenchAllocation ={
    allocationId: 1,
    employeeId: 1,
    startDate: Date.toString(),
    endDate: Date.toString(),
    details: 'string',
    trainingId: 1,
    internalProjectId: 1,
    typeId: 1,
   }

    const mockApiResponse: ApiResponse<BenchAllocation> = {
      data: benchAllocation,
      message: 'Employee details fetched successfully',
      success: false
    };

    service.getEmployeeById(employeeId).subscribe(
      response => {
        expect(response).toEqual(mockApiResponse, 'should return employee details');
      },
      fail
    );

    const req = httpTestingController.expectOne(apiUrl1 + `GetEmployeeById/${employeeId}`);
    expect(req.request.method).toEqual('GET');

    req.flush(mockApiResponse);
  });
});
