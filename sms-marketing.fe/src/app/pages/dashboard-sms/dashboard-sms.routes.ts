import { Routes } from '@angular/router';
import { DashboardSms } from './dashboard-sms/dashboard-sms';
import { PermissionConstants } from '@/shared/constants/permission.constants';
import { permissionGuard } from '@/shared/guard/permission-guard';


export default [
    { path: 'dashboard-sms', data: { breadcrumb: 'dashboard-sms', permission: PermissionConstants.MenuMarketingSms }, component: DashboardSms, canActivate: [permissionGuard] },
        
] as Routes;
