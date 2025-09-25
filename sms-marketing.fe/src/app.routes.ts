import { Routes } from '@angular/router';
import { AppLayout } from './app/layout/component/app.layout';
import { Dashboard } from './app/pages/dashboard/dashboard';
import { Documentation } from './app/pages/documentation/documentation';
import { Landing } from './app/pages/landing/landing';
import { Notfound } from './app/pages/notfound/notfound';
import { authGuard } from '@/shared/guard/auth-guard';
import { diemDanhGuard } from '@/shared/guard/diem-danh-guard';

export const appRoutes: Routes = [
    {
        path: '',
        component: AppLayout,
        canActivate: [authGuard],
        children: [
            { path: '', component: Dashboard },
            { path: 'channel', loadChildren: () => import('./app/pages/channel/channel.routes') },
            { path: 'danh-ba', loadChildren: () => import('./app/pages/danh-ba/danh-ba.routes') },
            { path: 'template', loadChildren: () => import('./app/pages/template/template.routes') },
            { path: 'report', loadChildren:()  => import('./app/pages/report/sms/sms-report.routers')},
            { path: 'meeting', loadChildren: () => import('./app/pages/meeting/meetings.routes') },
            { path: 'uikit', loadChildren: () => import('./app/pages/uikit/uikit.routes') },
            { path: 'documentation', component: Documentation },
            { path: 'pages', loadChildren: () => import('./app/pages/pages.routes') }
        ]
    },
    { path: 'diem-danh', canActivate: [diemDanhGuard], loadChildren: () => import('./app/pages/diem-danh/diem-danh.routes')  },
    { path: 'landing', component: Landing },
    { path: 'notfound', component: Notfound },
    { path: 'auth', loadChildren: () => import('./app/pages/auth/auth.routes') },
    { path: '**', redirectTo: '/notfound' }
];
