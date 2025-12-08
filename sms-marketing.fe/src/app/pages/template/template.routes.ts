import { Routes } from "@angular/router";
import { MauNoiDung } from "./mau-noi-dung/mau-noi-dung";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { CreateTemplateEmail } from "./create-template-email/create-template-email";

export default [
  { path: 'mau-nd', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: MauNoiDung, canActivate: [permissionGuard] },
  { path: 'mau-nd/create-template-email', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: CreateTemplateEmail, canActivate: [permissionGuard] },
] as Routes
