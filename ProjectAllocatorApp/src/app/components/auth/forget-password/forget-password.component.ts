import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { ForgetPassword } from 'src/app/models/forget-password.model';
import { SecurityQuestion } from 'src/app/models/security-question.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.css']
})
export class ForgetPasswordComponent implements OnInit{
  username: string | null | undefined;
  questions: SecurityQuestion[] | undefined;
  loading : boolean = false;
  forgetPassword: ForgetPassword = {
    loginId: '',
    securityQuestionId: 0,
    answer: '',
    newPassword: '',
    confirmNewPassword: ''
  };
  
  constructor(
    private authService: AuthService,
    private router : Router
  ){}

  ngOnInit(): void {
    this.loadQuestions();
  };

  loadQuestions(): void{
    this.loading = true;
    this.authService.getAllQuestions().subscribe({
      next:(response: ApiResponse<SecurityQuestion[]>) =>{
        if(response.success){
          this.questions = response.data;
        }
        else{
          console.error('Failed to fetch questions ', response.message);
        }
        this.loading = false;
      },error:(error)=>{
        console.error('Error fetching questions : ',error);
        this.loading = false;
      }
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

  onSubmit(forgetPasswordForm: NgForm): void {
    if(forgetPasswordForm.valid){
      this.authService.resetPassword(this.forgetPassword).subscribe({
        next:(response) => {
          if(response.success){
            alert('Password reset successfull.')
            this.router.navigate(['/signin'])
          }
          else {
            alert(response.message);
          }
        },
        error:(err) => {
          alert(err.error.message);
        }
      });
    }
  };

}
