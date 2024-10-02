import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ChangePassword } from '../models/change-password.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { ForgetPassword } from '../models/forget-password.model';
import { SecurityQuestion } from '../models/security-question.model';
import { UserRole } from '../models/UserRoles';
import { RegisterUser } from '../models/RegisterUser';
import { LocalStorageKeys } from './helper/localstoragekeys';
import { BehaviorSubject } from 'rxjs';
import { Route, RouteReuseStrategy, Router } from '@angular/router';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let authStateSubject: BehaviorSubject<boolean>;
  let usernameSubject: BehaviorSubject<string | null | undefined>;
  let userRoleSubject: BehaviorSubject<string | null | undefined>;
  let router : Router

  const mockApiResponse: ApiResponse<SecurityQuestion[]> = {
    success: true,
    data:[
      {
        securityQuestionId: 1,
        securityQuestionName: 'TestQuestion1'
      },
      {
        securityQuestionId: 2,
        securityQuestionName: 'TestQuestion2'
      }
    ],
    message : ''
  };

  const mockApiUserRoleResponse: ApiResponse<UserRole[]> = {
    success: true,
    data:[
      {
        userRoleId: 1,
        userRoleName: 'Allocator'
      },
      {
        userRoleId: 2,
        userRoleName: 'Manager'
      }
    ],
    message : ''
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, FormsModule],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });
  
  afterEach(()=>{
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should sign out and clear local storage and subjects', () => {
    spyOn(router,'navigate');
    spyOn(localStorage,'removeItem');
    // Act
    service.signOut();
 
    // Assert
    expect(localStorage.removeItem).toHaveBeenCalledWith(LocalStorageKeys.TokenName);
    expect(localStorage.removeItem).toHaveBeenCalledWith(LocalStorageKeys.LoginId);
    expect(localStorage.removeItem).toHaveBeenCalledWith(LocalStorageKeys.UserRole);
  });
  // changePassword

  it('should change password successfully',() => {
    // Arrnage
    const changePassword : ChangePassword = {
      loginId: 'test',
      oldPassword: 'Pass@1234',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123'
    };

    const mockSuccessResponse: ApiResponse<string>={
      data: '',
      success: true,
      message: 'Password changed successfully.'
    };

    //Act
    service.changePassword(changePassword).subscribe(
      response => {
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });
    const req=httpMock.expectOne('http://localhost:5031/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });

  it('should handle failed updation',()=>{
 // Arrnage
 const changePassword : ChangePassword = {
  loginId: 'test',
  oldPassword: 'Pass@1234',
  newPassword: 'Passowrd@123',
  confirmNewPassword: 'Password@123'
  };

  const mockErrorResponse: ApiResponse<string>={
    data: '',
    success: true,
    message: 'Password changed failed.'
  };

    //Act
    service.changePassword(changePassword).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5031/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while updating',()=>{
    //Arrange
    const changePassword : ChangePassword = {
      loginId: 'test',
      oldPassword: 'Pass@1234',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123'
      };

    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.changePassword(changePassword).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5031/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  // resetPassword

  it('should reset password successfully',() => {
    // Arrnage
    const resetPassword : ForgetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      securityQuestionId: 1,
      answer: 'TestAnswer'
    };

    const mockSuccessResponse: ApiResponse<string>={
      data: '',
      success: true,
      message: 'Password changed successfully.'
    };

    //Act
    service.resetPassword(resetPassword).subscribe(
      response => {
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });
    const req=httpMock.expectOne('http://localhost:5031/api/Auth/ForgetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });
  
  it('should handle failed updation',()=>{
    // Arrnage
    const resetPassword : ForgetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      securityQuestionId: 1,
      answer: 'TestAnswer'
    };
   
     const mockErrorResponse: ApiResponse<string>={
       data: '',
       success: true,
       message: 'Password changed failed.'
     };
   
       //Act
       service.resetPassword(resetPassword).subscribe(response=>{
         
         //Assert
         expect(response).toBe(mockErrorResponse);
       });
   
       //Api call
       const req=httpMock.expectOne('http://localhost:5031/api/Auth/ForgetPassword');
       expect(req.request.method).toBe('PUT');
       req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while updating',()=>{
    //Arrange
    const resetPassword : ForgetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      securityQuestionId: 1,
      answer: 'TestAnswer'
    };
    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.resetPassword(resetPassword).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5031/api/Auth/ForgetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  //GetAllQuestions

  it('should fetch all questions successfully',()=>{
    // Arrange
    const apiUrl ='http://localhost:5031/api/Auth/';

    //Act
    service.getAllQuestions().subscribe((response) => {
      //Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should handle an empty categories list', () => {
    // Arrange
    const apiUrl ='http://localhost:5031/api/Auth/';
 
    const emptyResponse: ApiResponse<SecurityQuestion[]> = {
      success: true,
      data: [],
      message: ''
    }
 
    // Act
    service.getAllQuestions().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });
 
    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully',()=>{
    // Arrange
    const apiUrl = "http://localhost:5031/api/Auth/"
    const errorMessage = 'Faild to load questions';
    
    //Act 
    service.getAllQuestions().subscribe(()=>
      fail('expect an error, not questions'),
      (error) =>{
        // Asssertttttt
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );
    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');

    // respond with error
    req.flush(errorMessage,{status:500,statusText:'Internal Server Error'});
  });

  //GetAllRoles

  it('should fetch all user roles successfully',()=>{
    // Arrange
    const apiUrl ='http://localhost:5031/api/Auth/';

    //Act
    service.getAllRoles().subscribe((response) => {
      //Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiUserRoleResponse.data);
    });

    const req = httpMock.expectOne(apiUrl+"GetUserRoles");
    expect(req.request.method).toBe('GET');
    req.flush(mockApiUserRoleResponse);
  });

  it('should handle an empty categories list', () => {
    // Arrange
    const apiUrl ='http://localhost:5031/api/Auth/';
 
    const emptyResponse: ApiResponse<SecurityQuestion[]> = {
      success: true,
      data: [],
      message: ''
    }
 
    // Act
    service.getAllRoles().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });
 
    const req = httpMock.expectOne(apiUrl+"GetUserRoles");
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully',()=>{
    // Arrange
    const apiUrl = "http://localhost:5031/api/Auth/"
    const errorMessage = 'Faild to load questions';
    
    //Act 
    service.getAllRoles().subscribe(()=>
      fail('expect an error, not questions'),
      (error) =>{
        // Asssertttttt
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );
    const req = httpMock.expectOne(apiUrl+"GetUserRoles");
    expect(req.request.method).toBe('GET');

    // respond with error
    req.flush(errorMessage,{status:500,statusText:'Internal Server Error'});
  });

  //SignUp

  it('should sign up a user successfully', () => {
    // Arrange
    const mockUser: RegisterUser = {
      name: 'John Doe',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phone: '1234567890',
      password: 'password123',
      confirmPassword: 'password123',
      securityQuestionId: 1,
      answer: 'answer123',
      userRoleId: 1
    };

    const mockResponse: ApiResponse<string> = {
      success: true,
      message: 'User registered successfully',
      data: ''
    };

    // Act
    service.signUp(mockUser).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User registered successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5031/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should not sign up a user', () => {
    // Arrange
    const mockUser: RegisterUser = {
      name: 'John Doe',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phone: '1234567890',
      password: 'password123',
      confirmPassword: 'pas',
      securityQuestionId: 1,
      answer: 'answer123',
      userRoleId: 2
    };

    const mockResponse: ApiResponse<string> = {
      success: false,
      message: 'User failed to register',
      data: ''
    };

    // Act
    service.signUp(mockUser).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User failed to register');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5031/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while signing up a user', () => {
    // Arrange
    const mockUser: RegisterUser = {
      name: 'John Doe',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phone: '1234567890',
      password: 'password123',
      confirmPassword: 'password123',
      securityQuestionId: 1,
      answer: 'answer123',
      userRoleId: 2
    };

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.signUp(mockUser).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5031/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });

});
