import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LocalstorageService } from './helper/localstorage.service';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LocalStorageKeys } from './helper/localstoragekeys';
import { ApiResponse } from '../models/ApiResponse{T}';
import { ChangePassword } from '../models/change-password.model';
import { ForgetPassword } from '../models/forget-password.model';
import { SecurityQuestion } from '../models/security-question.model';
import { RegisterUser } from '../models/RegisterUser';
import { UserRole } from '../models/UserRoles';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5031/api/Auth/';

  constructor(private http: HttpClient, private localStorageHelper: LocalstorageService, private router: Router) { }

  private authState = new BehaviorSubject<boolean>(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));

  private usernameSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.LoginId));
  private userRoleSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.UserRole));

  signIn(username: string, password: string): Observable<ApiResponse<string>> {
    const body={username, password};
    return this.http.post<ApiResponse<string>>(this.apiUrl+"Login", body).pipe(
      tap(response => {
        if(response.success){

          const token = response.data;
 
          const payload = token.split('.')[1];
          const decodedPayload = JSON.parse(atob(payload));
          const userRole = decodedPayload.UserRole;

          this.localStorageHelper.setItem(LocalStorageKeys.TokenName, token);
          this.localStorageHelper.setItem(LocalStorageKeys.LoginId, username);
          this.localStorageHelper.setItem(LocalStorageKeys.UserRole, userRole);

          this.authState.next(true);
          
          
          this.authState.next(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));
          this.usernameSubject.next(username);
          this.userRoleSubject.next(userRole);
        }
      }),
    );
  };

  //service for signout
  signOut(){
    this.localStorageHelper.removeItem(LocalStorageKeys.TokenName);
    this.localStorageHelper.removeItem(LocalStorageKeys.LoginId);
    this.localStorageHelper.removeItem(LocalStorageKeys.UserRole);
    this.router.navigate(['/home']);
    this.authState.next(false);
    this.usernameSubject.next(null);
    this.userRoleSubject.next(null);
  }

  getUsername(): Observable< string | null | undefined> {
    return this.usernameSubject.asObservable();
  };

  getUserRoleId(): Observable< string | null | undefined> {
    return this.userRoleSubject.asObservable();
  };

  isAuthenticated(){
    return this.authState.asObservable();
  };

  changePassword(changePassword: ChangePassword): Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(this.apiUrl+ 'ChangePassword',changePassword)
  };

  resetPassword(forgetPassword: ForgetPassword): Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(this.apiUrl+ 'ForgetPassword',forgetPassword)
  };

  getAllQuestions():Observable<ApiResponse<SecurityQuestion[]>>{
    return this.http.get<ApiResponse<SecurityQuestion[]>>(this.apiUrl+'GetAllSecurityQuestions')
  };

  signUp(user: RegisterUser): Observable<ApiResponse<string>> {
    const body = user;
    return this.http.post<any>(this.apiUrl + "Register", body);
  }

  getAllRoles(): Observable<ApiResponse<UserRole[]>> {
    return this.http.get<ApiResponse<UserRole[]>>(this.apiUrl + 'GetUserRoles')
  }
}
