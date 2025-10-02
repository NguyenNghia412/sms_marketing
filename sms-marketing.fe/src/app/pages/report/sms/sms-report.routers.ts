import { Component } from "@angular/core";
import { Routes } from "@angular/router";
import { ChienDichReport } from "./chien-dich/chien-dich-report";
import { ChiTietChienDichReport } from "./chi-tiet/chi-tiet-report";
import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";


export default [
    { path: 'chien-dich-report',data: { breadcrumb: 'chien-dich-report', permission: PermissionConstants.MenuReport}, component: ChienDichReport, canActivate: [permissionGuard]},
    { path: 'chi-tiet-chien-dich-report', data: { breadcrumb: 'chi-tiet-chien-dich-report', permission: PermissionConstants.MenuReport }, component: ChiTietChienDichReport, canActivate: [permissionGuard]},
] as Routes