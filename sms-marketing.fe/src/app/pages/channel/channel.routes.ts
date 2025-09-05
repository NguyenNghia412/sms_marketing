import { Routes } from "@angular/router";
import { Sms } from "./sms/sms";

export default [
  { path: 'sms', data: { breadcrumb: 'sms' }, component: Sms },
] as Routes