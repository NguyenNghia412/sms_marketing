import { GuiEmail } from './gui-email/gui-email';
import { Routes } from '@angular/router';
import { Sms } from './sms/sms';
import { GuiTinNhan } from './gui-tin-nhan/gui-tin-nhan';
import { permissionGuard } from '@/shared/guard/permission-guard';
import { PermissionConstants } from '@/shared/constants/permission.constants';
import { Email } from './email/email';

export default [
    { path: 'sms', data: { breadcrumb: 'sms', permission: PermissionConstants.MenuMarketingSms }, component: Sms, canActivate: [permissionGuard] },
    { path: 'gui-sms', data: { breadcrumb: 'gui-sms', permission: PermissionConstants.MenuMarketingSms }, component: GuiTinNhan, canActivate: [permissionGuard] },
    //
    { path: 'email', data: { breadcrumb: 'email', permission: PermissionConstants.MenuMarketingSms }, component: Email, canActivate: [permissionGuard] },
    { path: 'gui-email', data: { breadcrumb: 'gui-email', permission: PermissionConstants.MenuMarketingSms }, component: GuiEmail, canActivate: [permissionGuard] },
] as Routes;
