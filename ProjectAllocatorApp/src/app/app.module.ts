
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FooterComponent } from './components/shared/footer/footer.component';
import { NavbarComponent } from './components/shared/navbar/navbar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SigninComponent } from './components/auth/signin/signin.component';
import { HomeComponent } from './components/home/home.component';
import { CapitalizePipe } from './pipes/capitalize.pipe';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { ForgetPasswordComponent } from './components/auth/forget-password/forget-password.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { GetemployeebydaterangeComponent } from './components/reports/getemployeebydaterange/getemployeebydaterange.component';
import { GetemployeebyjobroleComponent } from './components/reports/getemployeebyjobrole/getemployeebyjobrole.component';
import { AllocatorListComponent } from './components/allocator/allocator-list/allocator-list.component';
import { AllocateformComponent } from './components/allocator/allocateform/allocateform.component';
import { SetbenchformComponent } from './components/allocator/setbenchform/setbenchform.component';
import { EmployeeListComponent } from './components/admin/employee-list/employee-list.component';
import { EmployeeDetailsComponent } from './components/admin/employee-details/employee-details.component';
import { EmployeeAddComponent } from './components/admin/employee-add/employee-add.component';
import { EmployeeUpdateComponent } from './components/admin/employee-update/employee-update.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { NgModule } from '@angular/core';

import { BenchListComponent } from './components/manager/bench-list/bench-list.component';
import { ManageallocationComponent } from './components/manager/manageallocation/manageallocation.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { AuthService } from './services/auth.service';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    NavbarComponent,
    SigninComponent,
    HomeComponent,
    CapitalizePipe,
    SignupComponent,
    GetemployeebydaterangeComponent,
    GetemployeebyjobroleComponent,
    ChangePasswordComponent,
    ForgetPasswordComponent,
    AllocatorListComponent,
    AllocateformComponent,
    SetbenchformComponent,
    EmployeeListComponent,
    EmployeeDetailsComponent,
    EmployeeAddComponent,
    EmployeeUpdateComponent,
    BenchListComponent,
    ManageallocationComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgbModule
    ],
  providers: [ DatePipe,AuthService,{provide:HTTP_INTERCEPTORS,useClass:AuthInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
