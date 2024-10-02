import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Employee } from '../models/Employee';
import { Observable } from 'rxjs';
import { JobRole } from '../models/JobRole.model';
import { SP } from '../models/sp.model';
import { AddEmployee } from '../models/AddEmployee';
import { EditEmployee } from '../models/EditEmployee';

@Injectable({
  providedIn: 'root'
})
export class AdminServce {

  private apiUrl = 'http://localhost:5031/api/Admin/';

  constructor(private http: HttpClient) { }
  
  getAllEmployees(): Observable<ApiResponse<Employee[]>> {
    return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + `GetAllEmployees`);
  }

  getAllEmployeesByPagination(page: number, pageSize: number, sortOrder: string, search?: string, sortBy?: string): Observable<ApiResponse<Employee[]>> {
    if (search != null && sortBy != null) {
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + `GetAllEmployeesByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search == null && sortBy != null){
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + `GetAllEmployeesByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search != null && sortBy == null){
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + `GetAllEmployeesByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else {
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + `GetAllEmployeesByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
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

  getEmployeeBasedOnDateRange(startDate: string,endDate:string) : Observable<ApiResponse<SP[]>>{
    return this.http.get<ApiResponse<SP[]>>(this.apiUrl+'GetEmployeeData?startDate='+startDate+'&enddate='+endDate);
  }

    getEmployeeBasedOnJobRole(jobroleId: number): Observable<ApiResponse<Employee[]>> {
        return this.http.get<ApiResponse<Employee[]>>(this.apiUrl + 'GetEmployeesByJobRoleAndType?jobRoleId=' + jobroleId + '&typeId=1');
    }
  addEmployee(addEmployee: AddEmployee): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}AddEmployee`, addEmployee);
  }

      getJobRoles() : Observable < ApiResponse < JobRole[] >> {
          return this.http.get<ApiResponse<JobRole[]>>(this.apiUrl + 'GetAllJobRoles');
      }
  getEmployeeById(employeeId: number): Observable<ApiResponse<Employee>> {
    return this.http.get<ApiResponse<Employee>>(`${this.apiUrl}GetEmployeeById?id=${employeeId}`)
  }

  



  modifyEmployee(editEmployee: EditEmployee): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}UpdateEmployee`, editEmployee);
  }

  deleteEmployee(employeeId: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}RemoveEmployee?id=${employeeId}`)
  }
}
