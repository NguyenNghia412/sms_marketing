import { CanActivateFn, Router } from '@angular/router';
import { Utils } from '../utils';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { UserService } from '@/services/user.service';
import { SharedService } from '@/services/shared.service';

export const authGuard: CanActivateFn = async (route, state) => {

  const router = inject(Router);
  const _userService = inject(UserService);
  const _sharedService = inject(SharedService);

  const authObject = Utils.getLocalStorage('auth')
  const accessToken = authObject?.accessToken;

  try {
    const res = await firstValueFrom(_userService.getMe());

    _sharedService.setRoles(res.data.roles || [])
    _sharedService.setPermissions(res.data.permissions || [])

  } catch (error) {
    const uri = `auth/login`
    router.navigate([uri], {
      queryParams: {
        redirect_uri: state.url
      }
    })
  }

  if (!accessToken) {
    const uri = `auth/login`
    router.navigate([uri], {
      queryParams: {
        redirect_uri: state.url
      }
    })
  }

  const jwtPayload = Utils.getDecodedJwtPayload();
  if (jwtPayload.user_type === 'SV') {
    const uri = 'auth/access'
    router.navigate([uri]);
  }

  return true;
};
