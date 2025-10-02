import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthConstants } from '../constants/auth.constants';
import { SharedService } from '@/services/shared.service';

export const permissionGuard: CanActivateFn = (route, state) => {

    const router = inject(Router);
    const _sharedService = inject(SharedService);

    const requiredPermission = route.data['permission'] as string;

    if (_sharedService.roles.includes(AuthConstants.SUPER_ADMIN_ROLE)) {
        return true;
    }
    if (_sharedService.permissions?.includes(requiredPermission)) {
        return true;
    }

    const uri = 'auth/access'
    router.navigate([uri]);
    return false;
};
