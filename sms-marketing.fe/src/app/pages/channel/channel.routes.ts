import { Routes } from "@angular/router";
import { Sms } from "./sms/sms";
import { GuiTinNhan } from "./gui-tin-nhan/gui-tin-nhan";

export default [
  { path: 'sms', data: { breadcrumb: 'sms' }, component: Sms },
  { path: 'gui-sms', data: { breadcrumb: 'gui-sms' }, component: GuiTinNhan },
] as Routes