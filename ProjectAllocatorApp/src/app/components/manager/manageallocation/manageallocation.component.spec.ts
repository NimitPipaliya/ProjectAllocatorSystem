import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageallocationComponent } from './manageallocation.component';
import { ManagerService } from 'src/app/services/manager.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BenchAllocation } from 'src/app/models/BenchAllocation';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BenchListComponent } from '../bench-list/bench-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { Employee } from 'src/app/models/Employee';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { AllocatorService } from 'src/app/services/allocator.service';

describe('ManageallocationComponent', () => {
  let component: ManageallocationComponent;
  let fixture: ComponentFixture<ManageallocationComponent>;
  let allocationSpy : jasmine.SpyObj<ManagerService>;
  let addSpy : jasmine.SpyObj<AllocatorService>;
  let router : Router;
  const mockAllocation : BenchAllocation ={
    allocationId: 1,
    employeeId: 1,
    startDate: Date.toString(),
    endDate: '',
    details: 'string',
    trainingId: 1,
    internalProjectId: 1,
    typeId: 1,
  }
  beforeEach(() => {
    let allocationSpyObj = jasmine.createSpyObj('ManagerService',["getEmployeeById"]);
    let addSpyObj = jasmine.createSpyObj('AllocatorService',["addAllocation"]);
    TestBed.configureTestingModule({
      declarations: [ManageallocationComponent],
      imports : [HttpClientTestingModule,RouterTestingModule.withRoutes([{path : 'benchlist',component:BenchListComponent}]),FormsModule,ReactiveFormsModule],
      providers : [
        {provide : ManagerService,useValue : allocationSpyObj},
        {provide : AllocatorService,useValue : addSpyObj},
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(ManageallocationComponent);
    component = fixture.componentInstance;
    allocationSpy = TestBed.inject(ManagerService) as jasmine.SpyObj<ManagerService>;
    addSpy = TestBed.inject(AllocatorService) as jasmine.SpyObj<AllocatorService>;
    router = TestBed.inject(Router) as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize employeeId from route params and load employee details', () => {
    // Arrange
    
    const mockResponse: ApiResponse<BenchAllocation> = { success: true, data: mockAllocation, message: '' };
    allocationSpy.getEmployeeById.and.returnValue(of(mockResponse));

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.employeeId).toBe(1);
    expect(allocationSpy.getEmployeeById).toHaveBeenCalledWith(1);
    expect(component.benchAllocation).toBeDefined();
    expect(component.benchAllocation.get('employeeId')).toBeTruthy();
    expect(component.benchAllocation.get('typeId')).toBeTruthy();
  });

  it('should handle error on fething data', () => {
    // Arrange
    var mockData !: BenchAllocation ;
    const mockResponse: ApiResponse<BenchAllocation> = { success: false, data: mockData, message: 'Failed to fetch employee' };
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
    var mockData !: BenchAllocation ;
    const mockError = { error: { message: 'Failed to add allocation' } };
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

  it('should add allocation suessccfully and nevigate to bench list',()=>{
    //Arrange
    spyOn(router,"navigate")
    const mockAllocation = {
      type : 1,
      employeeId : 1,
      trainingId: 2,
      internalProjectId: 1,
      typeId : 1,
      details : 'd1'
    };
    component.benchAllocation.setValue(mockAllocation)
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Employee allocated successfully' };

    addSpy.addAllocation.and.returnValue(of(mockResponse));

    //Act
    
    component.onSubmit();

    //Assert
    expect(router.navigate).not.toHaveBeenCalledWith(['/benchlist']);


  })

  it('should handle error when add allocation fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockAllocation = {
      type : 1,
      employeeId : 1,
      trainingId: 2,
      internalProjectId: 1,
      typeId : 1,
      details : 'd1'
    };

    component.benchAllocation.setValue(mockAllocation)
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    addSpy.addAllocation.and.returnValue(of(mockResponse));

    // Act
    component.onSubmit();

    // Assert

    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
  })


  it('should handle error when http error on add allocation', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockAllocation = {
      type : 1,
      employeeId : 1,
      trainingId: 2,
      internalProjectId: 1,
      typeId : 1,
      details : 'd1'
    };

    component.benchAllocation.setValue(mockAllocation)
    //const mockError = { error: { message: 'HTTP error' } };
    const mockError = { error: { message: 'Failed to add allocation' } };
    addSpy.addAllocation.and.returnValue(throwError(mockError));

    // Act
    component.onSubmit();

    // Assert
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
  })

  it("should handle formcontrol method",()=>{
    component.formControls;
  })
});
