import { Routes } from "@angular/router";
import { DiemDanhQr } from "./diem-danh-qr/diem-danh-qr";

export default [
  { path: 'qr', data: { breadcrumb: 'DiemDanhQr' }, component: DiemDanhQr },
] as Routes