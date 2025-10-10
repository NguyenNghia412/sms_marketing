import { Routes } from "@angular/router";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { Plan } from "./cau-hinh/plan/plan";
import { SubPlan } from "./cau-hinh/sub-plan/sub-plan";
import { SvNhanBang } from "./cau-hinh/sv-nhan-bang/sv-nhan-bang";
import { ScanQrSv } from "./scan-qr-sv/scan-qr-sv";
import { McScreen } from "./mc-screen/mc-screen";

export default [
  { path: 'config/plan', title: 'Chương trình', data: { breadcrumb: 'plan', permission: PermissionConstants.MenuTraoBangCauHinhChuongTrinh }, component: Plan, canActivate: [permissionGuard], },
  { path: 'config/sub-plan', title: 'Khoa', data: { breadcrumb: 'sub-plan', permission: PermissionConstants.MenuTraoBangCauHinhKhoa }, component: SubPlan, canActivate: [permissionGuard] },
  { path: 'config/sv', title: 'Sinh viên nhận bằng', data: { breadcrumb: 'sv', permission: PermissionConstants.MenuTraoBangCauHinhSinhVienNhanBang }, component: SvNhanBang, canActivate: [permissionGuard] },
  { path: 'scan-qr-sv', title: 'Checkin', data: { breadcrumb: 'scan-qr-sv', permission: PermissionConstants.MenuTraoBangQuetQr }, component: ScanQrSv, canActivate: [permissionGuard] },
  { path: 'mc-screen', title: 'Điều khiển', data: { breadcrumb: 'mc-screen', permission: PermissionConstants.MenuTraoBangMc }, component: McScreen, canActivate: [permissionGuard] },
] as Routes