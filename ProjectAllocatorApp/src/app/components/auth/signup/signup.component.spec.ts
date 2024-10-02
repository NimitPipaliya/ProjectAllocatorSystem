import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SignupComponent } from './signup.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { HomeComponent } from '../../home/home.component';
import { SecurityQuestion } from 'src/app/models/security-question.model';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { UserRole } from 'src/app/models/UserRoles';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: Router;

  beforeEach(() => {
    let authServiceSpyObj = jasmine.createSpyObj('AuthService', ['getAllQuestions', 'forgetPassword', 'signUp','getAllRoles']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule,FormsModule, RouterTestingModule.withRoutes([{ path: 'home', component: HomeComponent}])],
      declarations: [SignupComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpyObj },
      ]
    });
    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router);
    //fixture.detectChanges();
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load roles on init', () => {
    let mockRoles: UserRole[] = [
      {
        userRoleId: 1,
        userRoleName: 'Allocator'
      }
    ];

    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];

    let mockApiQueResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    let mockApiResponse: ApiResponse<UserRole[]> = {
      data: mockRoles,
      success: true,
      message: ''
    };

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiQueResponse));
    authServiceSpy.getAllRoles.and.returnValue(of(mockApiResponse));
    fixture.detectChanges();

    expect(authServiceSpy.getAllRoles).toHaveBeenCalled();
    expect(component.roles).toEqual(mockRoles);
  });

  it('should set roles when roles are loaded successfully', () => {
    let mockRoles: UserRole[] = [
      {
        userRoleId: 1,
        userRoleName: 'Allocator'
      }
    ];

    let mockApiResponse: ApiResponse<UserRole[]> = {
      data: mockRoles,
      success: true,
      message: ''
    };

    authServiceSpy.getAllRoles.and.returnValue(of(mockApiResponse))
    component.loadRoles()

    expect(authServiceSpy.getAllRoles).toHaveBeenCalled();
    expect(component.roles).toEqual(mockRoles);
  });

  it('should set console error when loading roles fails', () => {
    let mockApiResponse: ApiResponse<UserRole[]> = {
      data: [],
      success: false,
      message: 'Error fetching questions'
    };
    spyOn(console, 'error');

    authServiceSpy.getAllRoles.and.returnValue(of(mockApiResponse))
    component.loadRoles()

    expect(authServiceSpy.getAllRoles).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch roles ', mockApiResponse.message)
  });

  it('should set console error when loading roles fails', () => {
    let mockError = { error: { message: 'Failed' }}
    spyOn(console, 'error');

    authServiceSpy.getAllRoles.and.returnValue(throwError(mockError))
    component.loadRoles()

    expect(authServiceSpy.getAllRoles).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Error fetching roles : ', mockError)
  });

  it('should load questions on init', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];

    let mockRoles: UserRole[] = [
      {
        userRoleId: 1,
        userRoleName: 'Allocator'
      }
    ];

    let mockApiRoleResponse: ApiResponse<UserRole[]> = {
      data: mockRoles,
      success: true,
      message: ''
    }

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    authServiceSpy.getAllRoles.and.returnValue(of(mockApiRoleResponse));
    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse));
    fixture.detectChanges();

    expect(authServiceSpy.getAllQuestions);
    expect(component.questions).toEqual(mockQuestions);
  });

  it('should set questions when questions are loaded successfully', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalled();
    expect(component.questions).toEqual(mockQuestions);
  });

  it('should set console error when loading questions fails', () => {
    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: [],
      success: false,
      message: 'Error fetching questions'
    };
    spyOn(console, 'error');

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch questions ', mockApiResponse.message)
  });

  it('should set console error when loading questions fails', () => {
    let mockError = { error: { message: 'Failed' }}
    spyOn(console, 'error');

    authServiceSpy.getAllQuestions.and.returnValue(throwError(mockError))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Error fetching questions : ', mockError)
  });

  it('should navigate to home when user signs up successfully', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockResponse: ApiResponse<string> = {
      success: true,
      data: '',
      message: '',
    }
    const mockSignUp = {
      salutation: 'Mr.',
      name: 'Test',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      birthDate: new Date(1/1/2001),
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer'
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(of(mockResponse));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith('User registered successfully!!');
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/home'])
  })

  it('should set alert when response is false', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockResponse: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Error while registering',
    }
    const mockSignUp = {
      name: 'Test',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer'
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(of(mockResponse));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith(mockResponse.message);
  });

  it('should set alert when api returns error', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        securityQuestionName: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockError = { error: {message: 'HTTP Error' } }
    const mockSignUp = {
      name: 'TestUser',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer',
      userRoleId: 2
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(throwError(mockError));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith(mockError.error.message);
  })


});
