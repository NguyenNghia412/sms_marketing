import { Routes } from "@angular/router";
import { DsDanhBa } from "./ds-danh-ba/ds-danh-ba";

export default [
  { path: 'ds', data: { breadcrumb: 'ds-danh-ba' }, component: DsDanhBa },
] as Routes