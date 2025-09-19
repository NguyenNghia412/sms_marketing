import { Routes } from "@angular/router";
import { MauNoiDung } from "./mau-noi-dung/mau-noi-dung";

export default [
  { path: 'mau-nd', data: { breadcrumb: 'MauNoiDung' }, component: MauNoiDung },
] as Routes