import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAddComponent } from './employee-add.component';
import { AdminServce } from 'src/app/services/admin.service';
import { Router } from '@angular/router';
import { EmployeeListComponent } from '../employee-list/employee-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';
import { AddEmployee } from 'src/app/models/AddEmployee';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';

describe('EmployeeAddComponent', () => {
  let component: EmployeeAddComponent;
  let fixture: ComponentFixture<EmployeeAddComponent>;
  let adminSpy : jasmine.SpyObj<AdminServce>;
  let mockRouter: jasmine.SpyObj<Router>;
  let router : Router;

  beforeEach(() => {
    adminSpy = jasmine.createSpyObj('AdminServce',['addEmployee']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,FormsModule,ReactiveFormsModule,RouterTestingModule.withRoutes([{path:'employeeList',component : EmployeeListComponent}])],
      declarations: [EmployeeAddComponent],
      providers : [
        {
          provide : AdminServce,useValue : adminSpy
        },
        // {
        //    provide: Router, useClass: class { navigate = jasmine.createSpy('navigate'); } 
        // }
      ]
    });
    fixture = TestBed.createComponent(EmployeeAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize form with default values', () => {
    expect(component.empForm).toBeDefined();
    // expect(component.empForm.valid).toBeFalsy();
    expect(component.empForm.get('employeeName')).toBeTruthy();
    expect(component.empForm.get('emailId')).toBeTruthy();
    expect(component.empForm.get('benchStartDate')).toBeTruthy();
    expect(component.empForm.get('benchEndDate')).toBeTruthy();
    expect(component.empForm.get('jobRoleId')).toBeTruthy();
    expect(component.empForm.get('skills')).toBeTruthy();
  });
  it('should disable submit button if form is invalid', () => {
    const submitBtn = document.getElementById('submitButton') as HTMLButtonElement;
    expect(submitBtn).toBeTruthy();
    expect(submitBtn.disabled).toBeTruthy();
  });
  it('should enable submit button if form is valid', async () => {
    component.empForm.patchValue({
      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-18', 
      jobRoleId: 1,
      skills: ['Angular','.net']
    });
    fixture.detectChanges();

    await fixture.whenStable();

    const submitBtn = document.getElementById('submitButton') as HTMLButtonElement;
    expect(submitBtn.disabled).toBeFalsy();
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
  it('should navigate to /employeeList on successful form submission', () => {
    // Arrange
    spyOn(router, 'navigate');
    component.empForm.setValue({
      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-20', 
      benchEndDate: null,
      jobRoleId: 1, 
      skills: ['Angular', 'TypeScript'] 
    });
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'success' };

    adminSpy.addEmployee.and.returnValue(of(mockResponse));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.addEmployee).toHaveBeenCalled();
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
      skills: ['Angular', 'TypeScript'] 
    });
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'fails' };

    adminSpy.addEmployee.and.returnValue(of(mockResponse));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.addEmployee).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
    expect(component.loading).toBe(false); 
  });
  it('should show error message when addEmployee service returns error', () => {
    // Arrange
    spyOn(window, 'alert');
   
    component.empForm.setValue({
      employeeName: 'Nimit',
      emailId: 'Nimit@gmail.com',
      benchStartDate: '2024-07-20', 
      benchEndDate: null,
      jobRoleId: 1, 
      skills: ['Angular', 'TypeScript'] 
    });
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };

    adminSpy.addEmployee.and.returnValue(throwError({ error: { message: undefined } }));
    component.loading = false; 

    // Act
    component.OnSubmit();

    // Assert
    expect(adminSpy.addEmployee).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith(undefined);
    expect(component.loading).toBe(false); 
  });
});
