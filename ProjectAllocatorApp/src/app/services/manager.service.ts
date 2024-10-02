import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Employee } from '../models/Employee';
import { BenchAllocation } from '../models/BenchAllocation';
import { Employees } from '../models/Employees';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private apiUrl1 = 'http://localhost:5031/api/Admin/';
  private apiUrl = 'http://localhost:5031/api/Manager/';
  constructor(private http : HttpClient) { }
  getAllEmployees(): Observable<ApiResponse<Employees[]>> {
    return this.http.get<ApiResponse<Employees[]>>(this.apiUrl + `GetAllEmployees`);
  }

  getAllEmployeesByPagination(page: number, pageSize: number, sortOrder: string, search?: string, sortBy?: string): Observable<ApiResponse<Employees[]>> {
    if (search != null && sortBy != null) {
      return this.http.get<ApiResponse<Employees[]>>(this.apiUrl + `GetAllEmployeesByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }

    else if(search == null && sortBy != null){
      return this.http.get<ApiResponse<Employees[]>>(this.apiUrl + `GetAllEmployeesByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search != null && sortBy == null){
      return this.http.get<ApiResponse<Employees[]>>(this.apiUrl + `GetAllEmployeesByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else {
      return this.http.get<ApiResponse<Employees[]>>(this.apiUrl + `GetAllEmployeesByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
  }
 
  getEmployeesCount(search?:string){
    if(search != null){
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetEmployeesCount?search=${search}`);
    }
    else{
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetEmployeesCount`);
    }
  }

  getEmployeeById(id : number) : Observable<ApiResponse<BenchAllocation>>{
    return this.http.get<ApiResponse<BenchAllocation>>(this.apiUrl + `GetEmployeeById/${id}`);
   }
}
