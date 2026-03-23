import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { SharedService } from '@/services/shared.service';

export const homeRedirectGuard: CanActivateFn = () => {
    const router = inject(Router);
    const sharedService = inject(SharedService);

    if (sharedService.isSuperAdmin()) {
        router.navigate(['/dashboard-sms/dashboard-sms']);
    } else {
        router.navigate(['/channel/sms']);
    }

    return false;
};
