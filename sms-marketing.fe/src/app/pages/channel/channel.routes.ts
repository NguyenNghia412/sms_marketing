import { Routes } from "@angular/router";
import { Sms } from "./sms/sms";
import { GuiTinNhan } from "./gui-tin-nhan/gui-tin-nhan";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { PermissionConstants } from "@/shared/constants/permission.constants";

export default [
  { path: 'sms', data: { breadcrumb: 'sms', permission: PermissionConstants.MenuMarketingSms }, component: Sms, canActivate: [permissionGuard], },
  { path: 'gui-sms', data: { breadcrumb: 'gui-sms', permission: PermissionConstants.MenuMarketingSms }, component: GuiTinNhan, canActivate: [permissionGuard] },
] as Routes