import { Routes } from "@angular/router";
import { MauNoiDung } from "./mau-noi-dung/mau-noi-dung";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";

export default [
  { path: 'mau-nd', data: { breadcrumb: 'MauNoiDung', permission: PermissionConstants.MenuTemplate }, component: MauNoiDung, canActivate: [permissionGuard] },
] as Routes