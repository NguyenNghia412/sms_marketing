import { Routes } from "@angular/router";
import { HopTrucTuyen } from "./hop-truc-tuyen/hop-truc-tuyen";
import { DotDiemDanh } from "./dot-diem-danh/dot-diem-danh";

export default [
  { path: 'hop-truc-tuyen', data: { breadcrumb: 'HopTrucTuyen' }, component: HopTrucTuyen },
  { path: 'dot-diem-danh', data: { breadcrumb: 'DotDiemDanh' }, component: DotDiemDanh },
] as Routes