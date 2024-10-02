import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeComponent } from './home.component';
import { CapitalizePipe } from 'src/app/pipes/capitalize.pipe';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router, RouterLink } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService } from 'src/app/services/auth.service';
import { of } from 'rxjs';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let authService : jasmine.SpyObj<AuthService>;
  let router : Router;
  
  beforeEach(() => {
    const authSpyObj = jasmine.createSpyObj("AuthService",["getUsername","isAuthenticated"]);
    TestBed.configureTestingModule({
      declarations: [HomeComponent,CapitalizePipe],
      imports : [HttpClientTestingModule],
      providers:[
        {
          provide : AuthService, useValue : authSpyObj
        }
      ]
    });
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it("load properly oninit", ()=>{
    //Arrange
    const mockAuthResponse : string | null | undefined = "test";
    authService.getUsername.and.returnValue(of(mockAuthResponse));
    authService.isAuthenticated.and.returnValue(of(true));
    //Act
    component.ngOnInit();

    //Assert

    expect(component.username).toEqual("test");
   });
});
