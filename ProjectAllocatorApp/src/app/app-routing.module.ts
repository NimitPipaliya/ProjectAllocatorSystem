import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SigninComponent } from './components/auth/signin/signin.component';
import { HomeComponent } from './components/home/home.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { ForgetPasswordComponent } from './components/auth/forget-password/forget-password.component';
import { managerGuard } from './guards/manager.guard';
import { allocatorGuard } from './guards/allocator.guard';
import { SignupComponent } from './components/auth/signup/signup.component';
import { GetemployeebydaterangeComponent } from './components/reports/getemployeebydaterange/getemployeebydaterange.component';
import { GetemployeebyjobroleComponent } from './components/reports/getemployeebyjobrole/getemployeebyjobrole.component';
import { AllocatorListComponent } from './components/allocator/allocator-list/allocator-list.component';
import { SetbenchformComponent } from './components/allocator/setbenchform/setbenchform.component';
import { AllocateformComponent } from './components/allocator/allocateform/allocateform.component';
import { EmployeeListComponent } from './components/admin/employee-list/employee-list.component';
import { EmployeeDetailsComponent } from './components/admin/employee-details/employee-details.component';
import { EmployeeAddComponent } from './components/admin/employee-add/employee-add.component';
import { EmployeeUpdateComponent } from './components/admin/employee-update/employee-update.component';
import { BenchListComponent } from './components/manager/bench-list/bench-list.component';
import { ManageallocationComponent } from './components/manager/manageallocation/manageallocation.component';
import { adminGuard } from './guards/admin.guard';

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: HomeComponent},
  {path:'signin', component: SigninComponent},
  {path:'signup', component: SignupComponent},
  {path:'getemployeebasedondaterange',component:GetemployeebydaterangeComponent, canActivate:[ adminGuard]},
  {path:'getemployeebyjobrole',component:GetemployeebyjobroleComponent, canActivate:[ adminGuard]},
  {path:'changepassword', component: ChangePasswordComponent, canActivate: [allocatorGuard ]},
  {path:'changepassword1', component: ChangePasswordComponent, canActivate: [managerGuard ]},
  {path: 'forgetPassword', component: ForgetPasswordComponent},
  {path:'allocatorlist', component: AllocatorListComponent, canActivate:[ allocatorGuard]},
  {path:'setbenchform/:id', component: SetbenchformComponent, canActivate:[ allocatorGuard]},
  {path:'allocateproject/:id', component: AllocateformComponent,canActivate:[ allocatorGuard]},
  {path:'employeeList', component: EmployeeListComponent, canActivate:[ adminGuard]},
  {path:'employeeDetail/:employeeId', component: EmployeeDetailsComponent, canActivate:[ adminGuard]},
  {path:'addEmployee', component: EmployeeAddComponent, canActivate:[ adminGuard]},
  {path:'updateEmployee/:employeeId', component: EmployeeUpdateComponent, canActivate:[ adminGuard]},
  {path:'benchlist', component: BenchListComponent, canActivate:[ managerGuard]},
  {path:'manageallocation/:id', component: ManageallocationComponent, canActivate:[ managerGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
