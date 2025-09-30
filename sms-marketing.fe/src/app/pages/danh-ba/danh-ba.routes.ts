import { Routes } from "@angular/router";
import { DsDanhBa } from "./ds-danh-ba/ds-danh-ba";
import { ChiTiet } from "./chi-tiet/chi-tiet";

export default [
  { path: 'ds', data: { breadcrumb: 'ds-danh-ba' }, component: DsDanhBa },
  { path: 'ds/chi-tiet', data: { breadcrumb: 'danh-ba-chi-tiet' }, component: ChiTiet },
] as Routes