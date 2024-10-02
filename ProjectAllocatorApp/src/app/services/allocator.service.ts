import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Employee } from '../models/Employee';
import { AddAllocation } from '../models/Addallocation';
import { UpdateEmpAfterAllocation } from '../models/UpdateEmployeeAfterAllocation';

@Injectable({
  providedIn: 'root'  
})
export class AllocatorService {
  private apiUrl1 = 'http://localhost:5031/api/Admin/';
  private apiUrl = 'http://localhost:5031/api/Allocator/';
  constructor(private http : HttpClient) { }

  getAllEmployees(): Observable<ApiResponse<Employee[]>> {
    return this.http.get<ApiResponse<Employee[]>>(this.apiUrl1 + `GetAllEmployees`);
  }
 
  getAllEmployeesByPagination(page: number, pageSize: number, sortOrder: string, search?: string, sortBy?: string): Observable<ApiResponse<Employee[]>> {
    if (search != null && sortBy != null) {
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl1 + `GetAllEmployeesByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search == null && sortBy != null){
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl1 + `GetAllEmployeesByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search != null && sortBy == null){
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl1 + `GetAllEmployeesByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else {
      return this.http.get<ApiResponse<Employee[]>>(this.apiUrl1 + `GetAllEmployeesByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
  }
 
  getEmployeesCount(search?:string){
    if(search != null){
      return this.http.get<ApiResponse<number>>(this.apiUrl1 + `GetEmployeesCount?search=${search}`);
    }
    else{
      return this.http.get<ApiResponse<number>>(this.apiUrl1 + `GetEmployeesCount`);
    }
  }

  getEmployeeById(id : number) : Observable<ApiResponse<Employee>>{

    return this.http.get<ApiResponse<Employee>>(this.apiUrl1 + `GetEmployeeById?id=${id}`);
   }

   addAllocation(addAllocation: AddAllocation): Observable<ApiResponse<string>> {
       return this.http.post<ApiResponse<string>>(`${this.apiUrl}Create`, addAllocation);
     }

     updateEmployee(updateEmployee : UpdateEmpAfterAllocation): Observable<ApiResponse<string>>{
      return this.http.put<ApiResponse<string>>(`${this.apiUrl1}UpdateEmployees`, updateEmployee);
     }
}
