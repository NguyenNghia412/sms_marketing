import { Routes } from '@angular/router';
import { Access } from './access';
import { Error } from './error';
import { Login } from './login/login';
import { AuthCallback } from './callback';

export default [
    { path: 'access', component: Access },
    { path: 'error', component: Error },
    { path: 'login', component: Login },
    { path: 'callback', component: AuthCallback }
] as Routes;
