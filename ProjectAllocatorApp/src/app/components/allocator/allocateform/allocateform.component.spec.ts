import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllocateformComponent } from './allocateform.component';
import { AllocatorService } from 'src/app/services/allocator.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Allocation } from 'src/app/models/Allocation';
import { JobRole } from 'src/app/models/JobRole';
import { Employee } from 'src/app/models/Employee';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AllocatorListComponent } from '../allocator-list/allocator-list.component';
import { of, throwError } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';

describe('AllocateformComponent', () => {
  let component: AllocateformComponent;
  let fixture: ComponentFixture<AllocateformComponent>;
  let allocationSpy : jasmine.SpyObj<AllocatorService>;
  let router : Router;
  const mockAllocation : Allocation = {
    typeId: 1,
    type: 'string',
    allocations: null,
    employees: null
  }
  const mockJobRole : JobRole = {
    jobRoleId : 1,
    jobRoleName : 'string'
  } 
  const mockEmployee : Employee = {
    employeeId: 1,
    employeeName: 'string',
    emailId: 'string',
    benchStartDate: Date.toString(),
    benchEndDate: Date.toString(),
    jobRoleId: 1,
    jobRole: mockJobRole,
    typeId: 1,
    allocation: mockAllocation,
    skills: ['abc']
  }
  beforeEach(() => {
    let employeeSpyObj = jasmine.createSpyObj('AllocatorService',['getEmployeeById','updateEmployee','addAllocation']);
    TestBed.configureTestingModule({
      declarations: [AllocateformComponent],
      imports : [HttpClientTestingModule,RouterTestingModule.withRoutes([{path : 'allocatorlist',component:AllocatorListComponent}]),FormsModule,ReactiveFormsModule],
      providers : [
        {provide : AllocatorService,useValue : employeeSpyObj},
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(AllocateformComponent);
    component = fixture.componentInstance;
    allocationSpy = TestBed.inject(AllocatorService) as jasmine.SpyObj<AllocatorService>;;
    router = TestBed.inject(Router) as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize employeeId from route params and load employee details', () => {
    // Arrange
    
    const mockResponse: ApiResponse<Employee> = { success: true, data: mockEmployee, message: '' };
    allocationSpy.getEmployeeById.and.returnValue(of(mockResponse));

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.employeeId).toBe(1);
    expect(allocationSpy.getEmployeeById).toHaveBeenCalledWith(1);
    expect(component.setBenchForm).toBeDefined();
    expect(component.setBenchForm.get('employeeId')).toBeTruthy();
    expect(component.setBenchForm.get('typeId')).toBeTruthy();
  });

  it('should handle error on fething data', () => {
    // Arrange
    var mockData !: Employee ;
    const mockResponse: ApiResponse<Employee> = { success: false, data: mockData, message: 'Failed to fetch employees' };
    allocationSpy.getEmployeeById.and.returnValue(of(mockResponse));
    spyOn(console,"error")

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.employeeId).toBe(1);
    expect(allocationSpy.getEmployeeById).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch employee',mockResponse.message);
  });
  it('should handle Http error for fetching data', () => {
    // Arrange
    var mockData !: Employee ;
    const mockError = { error: { message: 'Failed to update employee' } };
    allocationSpy.getEmployeeById.and.returnValue(throwError(mockError));
    spyOn(console,"error");
    spyOn(window,"alert");

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.employeeId).toBe(1);
    expect(allocationSpy.getEmployeeById).toHaveBeenCalledWith(1);
  });

  it('should update employee suessccfully and nevigate to allocator list',()=>{
    //Arrange
    spyOn(router,"navigate")
    const employee = {
      employeeId : 1,
      typeId : 1,
      details : 'string',
      startDate : Date.toString(),
      endDate : Date.toString(),
      trainingId :1,
      internalProjectId : 1
    };
    const updateEmployee = {
      employeeId : 1,
      typeId : 1,
      startDate : Date.toString(),
      endDate : Date.toString()
    }

    component.setBenchForm.setValue(employee)
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Employee updated successfully' };

    allocationSpy.updateEmployee.and.returnValue(of(mockResponse));

    //Act
    
    component.updateEmployee();

    //Assert
   // expect(allocationSpy.updateEmployee).toHaveBeenCalledWith(updateEmployee);
    expect(router.navigate).toHaveBeenCalledWith(['/allocatorlist']);


  })

  it('should handle error when update employees fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const employee = {
      employeeId : 1,
      typeId : 1,
      details : 'string',
      startDate : Date.toString(),
      endDate : Date.toString(),
      trainingId :1,
      internalProjectId : 1
    };
    const updateEmployee = {
      employeeId : 1,
      typeId : 1,
      startDate : Date.toString(),
      endDate : Date.toString()
    }
    component.setBenchForm.setValue(employee)
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    allocationSpy.updateEmployee.and.returnValue(of(mockResponse));

    // Act
    component.updateEmployee();

    // Assert
  // expect(allocationSpy.updateEmployee).toHaveBeenCalledWith(updateEmployee);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
  //  expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
  })


  it('should handle error when http error on update employee', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const employee = {
      employeeId : 1,
      typeId : 1,
      details : 'string',
      startDate : Date.toString(),
      endDate : Date.toString(),
      trainingId :1,
      internalProjectId : 1
    };
    const updateEmployee = {
      employeeId : 1,
      typeId : 1,
      startDate : Date.toString(),
      endDate : Date.toString()
    }

    component.setBenchForm.setValue(employee)
    //const mockError = { error: { message: 'HTTP error' } };
    const mockError = { error: { message: 'Failed to update employee' } };
    allocationSpy.updateEmployee.and.returnValue(throwError(mockError));

    // Act
    component.updateEmployee();

    // Assert
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })

  it("should handle formcontrol method",()=>{
    component.formControls;
  });

});
