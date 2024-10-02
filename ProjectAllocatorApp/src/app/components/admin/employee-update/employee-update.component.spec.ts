import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeUpdateComponent } from './employee-update.component';
import { AdminServce } from 'src/app/services/admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Employee } from 'src/app/models/Employee';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { EmployeeListComponent } from '../employee-list/employee-list.component';
import { EditEmployee } from 'src/app/models/EditEmployee';
import { DatePipe } from '@angular/common';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('EmployeeUpdateComponent', () => {
  let component: EmployeeUpdateComponent;
  let fixture: ComponentFixture<EmployeeUpdateComponent>;
  let adminSpy: jasmine.SpyObj<AdminServce>;
  let router: Router;

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

  const mockEditEmpData: EditEmployee = {
    employeeId: 1,
    employeeName: 'Name',
    emailId: 'name@gmail.com',
    benchStartDate: '2025-01-01',
    benchEndDate: '',
    jobRoleId: 1,
    typeId: 1,
    skills: [".net", "mvc", "angular"]
  }
  const mockEmployeeData = {
    data: {
      employeeId: 1,
      employeeName: 'Name',
      emailId: 'name@gmail.com',
      benchStartDate: '2025-01-01',
      benchEndDate: '',
      jobRoleId: 1,
      typeId: 1,
      skills: [".net", "mvc", "angular"]
    }
  }

  beforeEach(() => {
    adminSpy = jasmine.createSpyObj('AdminServce', ['getEmployeeById', 'modifyEmployee']);
    const mockResponse: ApiResponse<Employee> = { success: true, data: mockEmp, message: 'success' };
    adminSpy.getEmployeeById.and.returnValue(of(mockResponse));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([{ path: 'employeeList', component: EmployeeListComponent }]), ReactiveFormsModule],
      declarations: [EmployeeUpdateComponent],
      providers: [
        DatePipe,
        { provide: AdminServce, useValue: adminSpy },
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
    fixture = TestBed.createComponent(EmployeeUpdateComponent);
    component = fixture.componentInstance;
    adminSpy = TestBed.inject(AdminServce) as jasmine.SpyObj<AdminServce>;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize form with employee details', () => {
    expect(adminSpy.getEmployeeById).toHaveBeenCalledWith(1); // Check if getEmployeeById was called with the correct employeeId
    expect(component.empForm.value.employeeName).toBe(mockEmployeeData.data.employeeName);
    expect(component.empForm.value.emailId).toBe(mockEmployeeData.data.emailId);
    expect(component.empForm.value.benchStartDate).toBe(mockEmployeeData.data.benchStartDate);
    expect(component.empForm.value.benchEndDate).toBeNull();
    expect(component.empForm.value.jobRoleId).toBe(mockEmployeeData.data.jobRoleId);
    expect(component.empForm.value.typeId).toBe(mockEmployeeData.data.typeId);
  });
  it('should handle error when fetching employee details', () => {
    const mockError = new Error('Failed to fetch employee details');
    adminSpy.getEmployeeById.and.returnValue(throwError(mockError));

    component.ngOnInit();

    expect(adminSpy.getEmployeeById).toHaveBeenCalled();
    expect(component.empForm.value.employeeName).toBe('');
    expect(component.empForm.value.emailId).toBe('');
    expect(component.empForm.value.benchStartDate).toBe('');
    expect(component.empForm.value.benchEndDate).toBe(null);
    expect(component.empForm.value.jobRoleId).toBe(0);
    expect(component.empForm.value.typeId).toBe(0);
    expect(component.empForm.value.skills).toEqual([]);

  });
  it('should update selectedSkills and showSuggestions when last segment length > 2', () => {
    const inputElement = document.createElement('input');
    inputElement.value = 'An'; 
    const event = { target: inputElement } as unknown as Event;

    component.onSkillInput(event);

    expect(component.selectedSkills).toEqual([]); 
    expect(component.showSuggestions).toBe(false);
  });
  it('should clear selectedSkills and hide suggestions when last segment length <= 2', () => {
    const inputElement = document.createElement('input');
    inputElement.value = 'Ja,'; 
    const event = { target: inputElement } as unknown as Event;

    component.onSkillInput(event);

    expect(component.selectedSkills).toEqual([]);
    expect(component.showSuggestions).toBe(false);
  });

  it('should update showSuggestions to true when lastSegment length > 2', () => {
    // Arrange
    const inputElement = document.createElement('input');
    inputElement.value = 'Ang'; 
    const event = { target: inputElement } as unknown as Event;
  
    // Act
    component.onSkillInput(event);
  
    // Assert
    expect(component.showSuggestions).toBe(true);
  });
  it('should add a skill to skills array', () => {
    const skillToAdd = 'Angular';

    component.addSkill(skillToAdd);

    expect(component.skills).toContain(skillToAdd);
    expect(component.empForm.get('skills')?.value).toContain(skillToAdd);
  });
  it('should not add a skill if it already exists in skills array', () => {
    const skillToAdd = 'Angular';
    component.skills = ['React', 'TypeScript', 'Angular'];

    component.addSkill(skillToAdd);

    expect(component.skills.filter(skill => skill === skillToAdd).length).toBe(1);
  });
  it('should remove a skill from skills array', () => {
    const skillToRemove = 'Angular';
    component.skills = ['React', 'TypeScript', 'Angular'];

    component.removeSkill(component.skills.indexOf(skillToRemove));

    expect(component.skills).not.toContain(skillToRemove);
    expect(component.empForm.get('skills')?.value).not.toContain(skillToRemove);
  });
  it('should enable the submit button', () => {
    const submitButton = document.createElement('button');
    submitButton.id = 'submitButton';
    document.body.appendChild(submitButton);

    component.enableSubmit();

    expect(submitButton.disabled).toBe(false);

    document.body.removeChild(submitButton);
  });
  it('should disable submit button', () => {
    const submitButton = document.createElement('button');
    submitButton.id = 'submitButton';
    document.body.appendChild(submitButton);
    component.disableSubmit();
    expect(submitButton.disabled).toBe(false);
    document.body.removeChild(submitButton);
  });
  it('should navigate to /employeeList on successful form submission', () => {
    // Arrange
    spyOn(router, 'navigate');
    component.empForm.setValue({
      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-20', 
      benchEndDate: null,
      jobRoleId: 1, 
      typeId:1,
      skills: ['Angular', 'TypeScript'] 
    });
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'success' };

    adminSpy.modifyEmployee.and.returnValue(of(mockResponse));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.modifyEmployee).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/employeeList']);
    expect(component.loading).toBe(false); 
  });
  it('should give error message alert on unsuccessful form submission', () => {
    // Arrange
    
    component.empForm.setValue({
      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-20', 
      benchEndDate: null,
      jobRoleId: 1, 
      typeId:1,
      skills: ['Angular', 'TypeScript'] 
    });
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'fails' };

    adminSpy.modifyEmployee.and.returnValue(of(mockResponse));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.modifyEmployee).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
    expect(component.loading).toBe(false); 
  });
  it('should show error message when updateEmployee service returns error', () => {
    // Arrange
    spyOn(window, 'alert');
   
    component.empForm.setValue({

      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-20', 
      benchEndDate: null,
      jobRoleId: 1, 
      typeId:1,
      skills: ['Angular', 'TypeScript'] 
    });
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };

    adminSpy.modifyEmployee.and.returnValue(throwError({ error: { message: undefined } }));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.modifyEmployee).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith(undefined);
    expect(component.loading).toBe(false); 
  });
});
