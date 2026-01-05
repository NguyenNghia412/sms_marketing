import { Component } from "@angular/core";
import { Routes } from "@angular/router";

import { PermissionConstants } from "@/shared/constants/permission.constants";
import { permissionGuard } from "@/shared/guard/permission-guard";
import { NhaMang } from "./nha-mang/nha-mang";
import { UserCredits } from "./user-credits/user-credits";
import { UserCreditsForUser } from "./user-credits-for-user/user-credits-for-user";


export default [
    { path: 'nha-mang',data: { breadcrumb: 'nha-mang', permission: PermissionConstants.MenuConfig}, component: NhaMang, canActivate: [permissionGuard], requireSuperAdmin: true},
    { path: 'user-credits',data: { breadcrumb: 'user-credits', permission: PermissionConstants.MenuConfig}, component: UserCredits, canActivate: [permissionGuard],requireSuperAdmin: true},
    { path: 'user-credits-for-user',data: { breadcrumb: 'user-credits-for-user', permission: PermissionConstants.MenuContact}, component: UserCreditsForUser, canActivate: [permissionGuard]},
] as Routes