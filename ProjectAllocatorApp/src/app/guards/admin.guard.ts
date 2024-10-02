import { CanActivateFn, Router } from '@angular/router';
import { LocalstorageService } from '../services/helper/localstorage.service';
import { inject } from '@angular/core';
import { LocalStorageKeys } from '../services/helper/localstoragekeys';

export const adminGuard: CanActivateFn = (route, state) => {

  const localStorageHelper = inject(LocalstorageService);
  const router = inject(Router);

  const tokenString = localStorageHelper.getItem(LocalStorageKeys.TokenName);
  const token = tokenString ;

  if(token != null){
    const payload = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payload));
    const userRole = decodedPayload.UserRole;

    if(userRole == 3){
      return true;
    }
  };

  if (!token) {
    router.navigate(['/home']);
    return false;
  }
  router.navigate(['/home']);
  return false;
};
