import { Routes } from "@angular/router";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { Plan } from "./cau-hinh/plan/plan";
import { SubPlan } from "./cau-hinh/sub-plan/sub-plan";
import { SvNhanBang } from "./cau-hinh/sv-nhan-bang/sv-nhan-bang";
import { ScanQrSv } from "./scan-qr-sv/scan-qr-sv";
import { McScreen } from "./mc-screen/mc-screen";

export default [
  { path: 'config/plan', data: { breadcrumb: 'plan', permission: PermissionConstants.MenuTraoBangCauHinhChuongTrinh }, component: Plan, canActivate: [permissionGuard], },
  { path: 'config/sub-plan', data: { breadcrumb: 'sub-plan', permission: PermissionConstants.MenuTraoBangCauHinhKhoa }, component: SubPlan, canActivate: [permissionGuard] },
  { path: 'config/sv', data: { breadcrumb: 'sv', permission: PermissionConstants.MenuTraoBangCauHinhSinhVienNhanBang }, component: SvNhanBang, canActivate: [permissionGuard] },
  { path: 'scan-qr-sv', data: { breadcrumb: 'scan-qr-sv', permission: PermissionConstants.MenuTraoBangQuetQr }, component: ScanQrSv, canActivate: [permissionGuard] },
  { path: 'mc-screen', data: { breadcrumb: 'mc-screen', permission: PermissionConstants.MenuTraoBangMc }, component: McScreen, canActivate: [permissionGuard] },
] as Routes