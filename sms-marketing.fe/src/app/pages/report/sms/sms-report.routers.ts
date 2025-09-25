import { Component } from "@angular/core";
import { Routes } from "@angular/router";
import { ChienDichReport } from "./chien-dich/chien-dich-report";
import { ChiTietChienDichReport } from "./chi-tiet/chi-tiet-report";


export default [
    { path: 'chien-dich-report',data: { breadcrumb: 'chien-dich-report'}, component: ChienDichReport},
    { path: 'chi-tiet-chien-dich-report', data: { breadcrumb: 'chi-tiet-chien-dich-report' }, component: ChiTietChienDichReport},
] as Routes