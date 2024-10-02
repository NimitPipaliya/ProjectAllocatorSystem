import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SigninComponent } from './signin.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('SigninComponent', () => {
  let component: SigninComponent;
  let fixture: ComponentFixture<SigninComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: Router;

  beforeEach(() => {
    let authServiceSpyObj = jasmine.createSpyObj('AuthService', ['signIn'])
    let routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, RouterTestingModule],
      declarations: [SigninComponent],
      providers: [
        {
          provide: AuthService, useValue: authServiceSpyObj
        }
      ]
    });
    fixture = TestBed.createComponent(SigninComponent);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to home when user signs in successfully', () => {
    // Arrange
    component.username = "test";
    component.password = "fakePassword"
    const mockResponse: ApiResponse<string> = {
      data: 'fakeToken',
      success: true,
      message: ''
    }
    spyOn(routerSpy, 'navigate')
    authServiceSpy.signIn.and.returnValue(of(mockResponse))

    // Act
    component.login()

    // Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledOnceWith(component.username, component.password)
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home'])
  });

  it('should set alert message when response is false', () => {
    // Arrange
    component.username = "test";
    component.password = "fakePassword"
    const mockResponse: ApiResponse<string> = {
      data: '',
      success: false,
      message: 'Verification failed'
    }
    spyOn(window, 'alert')
    authServiceSpy.signIn.and.returnValue(of(mockResponse))

    // Act
    component.login()

    // Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledOnceWith(component.username, component.password)
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message)
  });

  it('should set alert message when api returns error', () => {
    // Arrange
    component.username = "test";
    component.password = "fakePassword"
    const mockError = { error: { message: 'HTTP Error' } }
    spyOn(window, 'alert')
    authServiceSpy.signIn.and.returnValue(throwError(mockError))

    // Act
    component.login()

    // Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledOnceWith(component.username, component.password)
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message)
  });
  
});

