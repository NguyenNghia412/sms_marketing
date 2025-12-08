import { Routes } from "@angular/router";
import { MauNoiDung } from "./email-tempalte/mau-noi-dung/mau-noi-dung";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { CreateTemplateEmail } from "./email-tempalte/create-template-email/create-template-email";
import { SmsMain } from "./sms-template/sms-main/sms-main";
import { CreateTemplateSms } from "./sms-template/create-template-sms/create-template-sms";

export default [
    { path: 'mau-email', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: MauNoiDung, canActivate: [permissionGuard] },
    { path: 'mau-email/create-template-email', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: CreateTemplateEmail, canActivate: [permissionGuard] },
    { path: 'mau-sms', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: SmsMain, canActivate: [permissionGuard] },
    { path: 'mau-email/create-template-sms', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: CreateTemplateSms, canActivate: [permissionGuard] },
] as Routes
