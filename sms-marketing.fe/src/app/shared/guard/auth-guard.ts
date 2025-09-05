import { CanActivateFn, Router } from '@angular/router';
import { Utils } from '../utils';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {

  const router = inject(Router);

  const authObject = Utils.getLocalStorage('auth')
  const accessToken = authObject?.accessToken;

  if (!accessToken) {
    router.navigate(['auth/login'])
  }

  return true;
};
