import { AuthConstants } from '@/shared/constants/auth.constants';
import { Utils } from '@/shared/utils';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { concatMap, of } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    http = inject(HttpClient);
    router = inject(Router);
    baseUrl = environment.baseUrl;

    login(username: string, password: string) {
        const body = new HttpParams()
            .set('username', username)
            .set('password', password)
            .set('grant_type', environment.authGrantType)
            .set('client_id', environment.authClientId)
            .set('client_secret', environment.authClientSecret ?? '')
            .set('scope', environment.authScope);
        const headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
            Accept: 'text/plain'
        });

        return this.http.post(`${this.baseUrl}/connect/token`, body.toString(), { headers }).pipe(
            concatMap((res: any) => {
                Utils.setLocalStorage('auth', {
                    accessToken: res.access_token,
                    refreshToken: res.refresh_token
                });

                const redirect_uri = Utils.getSessionStorage(AuthConstants.REDIRECT_URI_AFTER_LOGIN) || '/';
                // Utils.removeSessionStorage(AuthConstants.REDIRECT_URI_AFTER_LOGIN)

                this.router.navigate([redirect_uri]);

                return of(res);
            })
        );
    }

    postConnectAuthorize(code: string, code_verifier: string) {
        // Exchange code for tokens
        const body = new HttpParams()
            .set('code', code)
            .set('code_verifier', code_verifier)
            .set('grant_type', 'authorization_code')
            .set('client_id', environment.authClientId)
            .set('client_secret', environment.authClientSecret)
            .set('redirect_uri', `${environment.appUrl}/auth/callback`);

        const headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
            Accept: 'text/plain'
        });

        return this.http.post<any>(`${environment.baseUrl}/connect/token`, body.toString(), { headers }).subscribe({
            next: (res) => {
                Utils.setLocalStorage('auth', {
                    accessToken: res.access_token,
                    refreshToken: res.refresh_token
                });
                Utils.removeSessionStorage(AuthConstants.SESSION_PKCE_CODE_VERIFIER);
                Utils.removeSessionStorage(AuthConstants.PKCE_CODE_CHALLENGE_METHOD);

                const redirect_uri = Utils.getSessionStorage(AuthConstants.REDIRECT_URI_AFTER_LOGIN) || '/';
                // Utils.removeSessionStorage(AuthConstants.REDIRECT_URI_AFTER_LOGIN)

                this.router.navigate([redirect_uri]);
            },
            error: (err) => console.error('Token exchange failed', err)
        });
    }

    logout() {
        Utils.clearLocalStorage();
        Utils.clearSessionStorage();
        this.router.navigate(['auth/login']);
    }
}
