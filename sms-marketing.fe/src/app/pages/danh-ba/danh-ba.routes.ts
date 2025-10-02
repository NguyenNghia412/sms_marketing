import { Routes } from "@angular/router";
import { DsDanhBa } from "./ds-danh-ba/ds-danh-ba";
import { ChiTiet } from "./chi-tiet/chi-tiet";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";

export default [
  { path: 'ds', data: { breadcrumb: 'ds-danh-ba', permission: PermissionConstants.MenuContact }, component: DsDanhBa, canActivate: [permissionGuard] },
  { path: 'ds/chi-tiet', data: { breadcrumb: 'danh-ba-chi-tiet', permission: PermissionConstants.MenuContact }, component: ChiTiet, canActivate: [permissionGuard] },
] as Routes