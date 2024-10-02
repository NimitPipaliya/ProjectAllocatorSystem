import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ChangePassword } from 'src/app/models/change-password.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  user: ChangePassword ={
    loginId: '',
    oldPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  }
  loading: boolean = false;

  constructor(private authService: AuthService,
    private router: Router,
  ){}

  ngOnInit(): void {
    this.authService.getUsername().subscribe((username : string|null|undefined)=>{
      this.user.loginId =username;
    });
  };

  checkPasswords(form: NgForm):void {
    const password = form.controls['newPassword'];
    const confirmPassword = form.controls['confirmNewPassword'];
 
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword.setErrors(null);
    }
  };

  onSubmit(changePasswordForm: NgForm){
    if(changePasswordForm.valid){
      this.loading = true;
      let changePassword :ChangePassword = {
        loginId: this.user.loginId,
        oldPassword: changePasswordForm.controls['oldPassword'].value,
        newPassword: changePasswordForm.controls['newPassword'].value,
        confirmNewPassword: changePasswordForm.controls['confirmNewPassword'].value
      };
      this.authService.changePassword(changePassword).subscribe({
        next:(response) => {
          if(response.success){
            alert('Password changed successfully.')
            this.authService.signOut();
            this.router.navigate(['/signin']);
          }
          else{
            alert(response.message);
          }
          this.loading = false;
        },
        error:(err) => {
          console.log(err.error.message);
          alert(err.error.message);
          this.loading=false;
        },
        complete:() =>{
          this.loading = false;
          console.log("completed");
        }
      });
    }
  };
}
