import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { LocalstorageService } from '../services/helper/localstorage.service';
import { LocalStorageKeys } from '../services/helper/localstoragekeys';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private localStorageHelper: LocalstorageService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token=this.localStorageHelper.getItem(LocalStorageKeys.TokenName);
    if(token){
      const cloneReq = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${token}`)
      });

      return next.handle(cloneReq);
    }else{
      return next.handle(request);
    }
    
  }
}
