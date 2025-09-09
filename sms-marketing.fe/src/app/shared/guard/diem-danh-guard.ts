import { CanActivateFn, Router } from '@angular/router';
import { Utils } from '../utils';
import { inject } from '@angular/core';

export const diemDanhGuard: CanActivateFn = (route, state) => {

  const router = inject(Router);

  const authObject = Utils.getLocalStorage('auth')
  const accessToken = authObject?.accessToken;

  if (!accessToken) {
    const uri = `auth/login`
    router.navigate([uri], {
      queryParams: {
        redirect_uri: state.url
      }
    })
  }

  return true;
};
