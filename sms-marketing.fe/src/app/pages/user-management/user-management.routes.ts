import { Routes } from "@angular/router";
import { User } from "./user/user";
import { Role } from "./role/role";

export default [
  { path: 'user', data: { breadcrumb: 'user' }, component: User },
  { path: 'role', data: { breadcrumb: 'role' }, component: Role },
] as Routes