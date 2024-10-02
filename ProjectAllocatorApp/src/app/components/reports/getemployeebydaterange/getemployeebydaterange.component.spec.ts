import { ComponentFixture, TestBed } from '@angular/core/testing';
 
import { GetemployeebydaterangeComponent } from './getemployeebydaterange.component';
import { AdminServce } from 'src/app/services/admin.service';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SP } from 'src/app/models/sp.model';
import { of, throwError } from 'rxjs';
 
describe('GetemployeebydaterangeComponent', () => {
  let component: GetemployeebydaterangeComponent;
  let fixture: ComponentFixture<GetemployeebydaterangeComponent>;
  let adminService: jasmine.SpyObj<AdminServce>; // Mocked AdminService
 
  beforeEach(() => {
    const spy = jasmine.createSpyObj('AdminServce', ['getEmployeeBasedOnDateRange']);
    TestBed.configureTestingModule({
      declarations: [GetemployeebydaterangeComponent],
      providers: [
        { provide: AdminServce, useValue: spy }
      ]
    });
    fixture = TestBed.createComponent(GetemployeebydaterangeComponent);
    component = fixture.componentInstance;
    component = new GetemployeebydaterangeComponent(adminService);
    adminService = TestBed.inject(AdminServce) as jasmine.SpyObj<AdminServce>;
  });
 
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should call setDefaultDates and getEmployeesByDateRange on ngOnInit', () => {
    spyOn(component, 'setDefaultDates');
    spyOn(component, 'getEmployeesByDateRange');
    const mockResponse: ApiResponse<SP[]> = {
      success: true,
      data: [/* mock data */],
      message: ''
    };
    adminService.getEmployeeBasedOnDateRange.and.returnValue(of(mockResponse));
    component.ngOnInit();
   
    expect(component.setDefaultDates).toHaveBeenCalled();
    expect(component.getEmployeesByDateRange).toHaveBeenCalled();
  });
  it('should set startDate to today\'s date in YYYY-MM-DD format', () => {
    // Call the method to set default dates
    component.setDefaultDates();
 
    // Get today's date
    const today = new Date();
    const year = today.getFullYear();
    const month = (today.getMonth() + 1).toString().padStart(2, '0');
    const day = today.getDate().toString().padStart(2, '0');
    const expectedStartDate = `${year}-${month}-${day}`;
 
    // Assert that startDate is set correctly
    expect(component.startDate).toEqual(expectedStartDate);
  });
  it('should handle leap year correctly', () => {
    const date = new Date('2020-02-29'); // Leap year example
    jasmine.clock().mockDate(date);
    component.setDefaultDates();
    expect(component.startDate).toEqual('2020-02-29');
  });
  it('should handle the last day of the month correctly', () => {
    const date = new Date('2023-01-31');
    jasmine.clock().mockDate(date);
    component.setDefaultDates();
    expect(component.startDate).toEqual('2023-01-31');
  });
  it('should handle the first day of the year correctly', () => {
    const date = new Date('2022-01-01');
    jasmine.clock().mockDate(date);
    component.setDefaultDates();
    expect(component.startDate).toEqual('2022-01-01');
  });
  it('should handle single-digit day and month values correctly', () => {
    const date = new Date('2023-03-05'); // March 5th, 2023
    jasmine.clock().mockDate(date);
    component.setDefaultDates();
    expect(component.startDate).toEqual('2023-03-05');
  });
  it('should handle successful response', () => {
    const mockResponse: ApiResponse<SP[]> = {
      success: true,
      data: [{
        employeeId: 1, employeeName: 'John Doe',
        typeId: 0,
        benchStartDate: '',
        benchEndDate: '',
        trainingId: 0,
        trainingName: '',
        trainingDescription: '',
        projectName: '',
        projectDescription: ''
      }],
      message: ''
    };
    adminService.getEmployeeBasedOnDateRange.and.returnValue(of(mockResponse));
 
    component.getEmployeesByDateRange();
 
    expect(component.loading).toBe(true); // Ensure loading is set to false
    expect(component.errorMessage).toEqual('*Please Select start date'); // Ensure error message is cleared
    expect(component.noDataMessage).toEqual(''); // Ensure no data message is cleared
  });
  it('should set errorMessage when startDate is not selected', () => {
    component.startDate = ''; // Set startDate to empty string
   
    component.getEmployeesByDateRange();
 
    expect(component.loading).toBe(true); // Ensure loading is set to false
    expect(component.errorMessage).toEqual('*Please Select start date'); // Verify error message is set
    expect(component.noDataMessage).toEqual(''); // Ensure no data message is cleared
    expect(component.employees).toEqual([]); // Verify employees are empty
  });
  it('should handle no data response', () => {
    const mockResponse: ApiResponse<SP[]> = {
      success: true,
      data: [],
      message: ''
    };
    adminService.getEmployeeBasedOnDateRange.and.returnValue(of(mockResponse));
 
    component.getEmployeesByDateRange();
 
    expect(component.loading).toBe(true); // Ensure loading is set to false
    expect(component.errorMessage).toEqual('*Please Select start date'); // Ensure error message is cleared
    expect(component.noDataMessage).toEqual(''); // Verify no data message
    expect(component.employees).toEqual([]); // Verify employees are empty
  });
  it('should fetch employees for valid date range and handle success', () => {
    // Arrange
    const mockStartDate = '2024-01-01';
    const mockEndDate = '2024-01-31';
    const mockApiResponse = {
      success: true,
      data: [ /* Mock employee data */ ],
      message:"",
    };
    adminService.getEmployeeBasedOnDateRange.and.returnValue(of(mockApiResponse));
  
    // Act
    component.startDate = mockStartDate;
    component.endDate = mockEndDate;
    component.getEmployeesByDateRange();
  
    // Assert
    expect(component.loading).toBe(false); // loading should be false after response
    expect(component.errorMessage).toBe(''); // no error message on success
    expect(component.employees).toEqual(mockApiResponse.data); // employees should be set correctly
  });
  it('should set error message when start date is not selected', () => {
    // Arrange
    const mockStartDate = "";
    component.startDate = mockStartDate;
  
    // Act
    component.getEmployeesByDateRange();
  
    // Assert
    expect(component.errorMessage).toBe('*Please Select start date');
    expect(adminService.getEmployeeBasedOnDateRange).not.toHaveBeenCalled(); // service should not be called
    expect(component.loading).toBe(true); // loading should be false
  });
  it('should handle error from service', () => {
    // Arrange
    const mockStartDate = '2024-01-01';
    const mockEndDate = '2024-01-31';
    const mockErrorMessage = 'Error fetching employees';
    adminService. getEmployeeBasedOnDateRange.and.returnValue(throwError({ error: { message: mockErrorMessage } }));
  
    // Act
    component.startDate = mockStartDate;
    component.endDate = mockEndDate;
    component.getEmployeesByDateRange();
  
    // Assert
    expect(component.loading).toBe(false); // loading should be false after error
    expect(component.errorMessage).toBe(mockErrorMessage); // error message should be set
    expect(component.employees.length).toBe(0); // employees should be empty
  });
  it('should handle success response correctly', () => {
    // Arrange
    const mockStartDate = '2024-01-01';
    const mockEndDate = '2024-01-31';
    const mockApiResponse = {
      success: true,
      data: [],
      message:""
    };
    adminService.getEmployeeBasedOnDateRange.and.returnValue(of(mockApiResponse));

    // Act
    component.startDate = mockStartDate;
    component.endDate = mockEndDate;
    component.getEmployeesByDateRange();

    // Assert
    expect(component.loading).toBe(false); // loading should be false after successful response
    expect(component.errorMessage).toBe(''); // no error message on success
    expect(component.noDataMessage).toBe('No employees found for the selected date range.'); // noDataMessage should be set correctly
  });
  it('should handle failure response correctly', () => {
    // Arrange
    const mockStartDate = '2024-01-01';
    const mockEndDate = '2024-01-31';
    const mockErrorMessage = 'Failed to fetch employees';
    adminService.getEmployeeBasedOnDateRange.and.returnValue(throwError({ error: { message: mockErrorMessage } }));

    // Act
    component.startDate = mockStartDate;
    component.endDate = mockEndDate;
    component.getEmployeesByDateRange();

    // Assert
    expect(component.loading).toBe(false); // loading should be false after encountering an error
    expect(component.errorMessage).toBe(mockErrorMessage); // error message should be set correctly
    
    expect(component.employees.length).toBe(0); // employees should be empty
  });
});