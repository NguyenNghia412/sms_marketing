import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Dashboard } from './dashboard/dashboard';
import { AppLayout } from './layout/component/app.layout';
import { Notfound } from './notfound/notfound';

export const routes: Routes = [
  {
    path: '',
    component: AppLayout,
    children: [
      {
        component: Dashboard,
        path: 'dashboard',
      },
    ],
  },
  { path: 'notfound', component: Notfound },
  {
    component: Login,
    path: 'auth/login',
  },
  { path: '**', redirectTo: '/notfound' },
];
