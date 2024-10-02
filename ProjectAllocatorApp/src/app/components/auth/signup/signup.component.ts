import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SecurityQuestion } from 'src/app/models/SecurityQuestions';
import { UserRole } from 'src/app/models/UserRoles';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  loading : boolean = false;
  signUpForm !: FormGroup;
  questions !: SecurityQuestion[] ;
  roles !: UserRole[];
  constructor(private formBuilder : FormBuilder,private authService : AuthService,  private router : Router){}
  ngOnInit(): void {
    this.signUpForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      loginId: ['', [Validators.required, Validators.minLength(2)]],
      phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(12)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&].{8,}$/)]],
      confirmPassword: ['', [Validators.required]],
      gender:['',Validators.required],
      securityQuestionId : [,[Validators.required,this.questionValidator]],
      answer: ['', [Validators.required]],
      userRoleId : [,[Validators.required, this.roleValidator]]
    }, {
      validator: this.passwordMatchValidator // Custom validator for password match
    });
    this.loadQuestions();
    this.loadRoles();
  }
  get formControl(){
    return this.signUpForm.controls;
   }

   questionValidator(control:any){
    return control.value=='' ? {invalidQuestion:true}: null
  }

  roleValidator(control:any){
    return control.value=='' ? {invalidRole:true}: null
  }
  passwordMatchValidator(form: FormGroup): { [key: string]: any } | null {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
  
    const passwordsMatch = password === confirmPassword;
    console.log('Passwords Match:', passwordsMatch);
  
    return passwordsMatch ? null : { passwordMismatch: true };
  }

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

  loadRoles(): void{
    this.loading = true;
    this.authService.getAllRoles().subscribe({
      next:(response: ApiResponse<UserRole[]>) =>{
        if(response.success){
          this.roles = response.data;
        }
        else{
          console.error('Failed to fetch roles ', response.message);
        }
        this.loading = false;
      },error:(error)=>{
        console.error('Error fetching roles : ',error);
        this.loading = false;
      }
    });
  };

  onSubmit(): void {
    if (this.signUpForm.valid) {
      console.log(this.signUpForm.value);

      this.authService.signUp(this.signUpForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            alert('User registered successfully!!');
            this.router.navigate(['/home']);
          } else {
            alert(response.message);
          }
        },
        error: (err) => {
          alert(err.error.message);
        }
      });
    }
  }

}
