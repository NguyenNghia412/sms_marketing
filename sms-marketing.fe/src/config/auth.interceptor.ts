import { Utils } from '@/shared/utils';
import { HttpClient, HttpErrorResponse, HttpHandlerFn, HttpHeaders, HttpParams, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    const token = Utils.getAccessToken();
    const baseUrl = environment.baseUrl;
    const http = inject(HttpClient);
    const router = inject(Router);

    // Clone the request to add the authentication header.
    const newReq = req.clone({
        headers: req.headers.append('Authorization', `Bearer ${token}`),
        url: req.url.startsWith('http') ? req.url : `${baseUrl}${req.url}`
    });
    return next(newReq).pipe(
        catchError((error: HttpErrorResponse) => {
            // Only handle 401
            if (error.status === 401) {
                const refreshToken = Utils.getRefreshToken();
                if (!refreshToken) {
                    Utils.setLocalStorage('auth', {});
                    router.navigate(['/auth/login']);
                }

                const body = new HttpParams().set('grant_type', 'refresh_token').set('client_id', environment.authClientId).set('refresh_token', refreshToken).set('client_secret', environment.authClientSecret);
                const headers = new HttpHeaders({
                    'Content-Type': 'application/x-www-form-urlencoded',
                    Accept: 'text/plain'
                });

                // Call /connect/token to refresh
                return http.post<any>(`${environment.baseUrl}/connect/token`, body.toString(), { headers }).pipe(
                    switchMap((res) => {
                        // Save new tokens

                        Utils.setLocalStorage('auth', {
                            accessToken: res.access_token,
                            refreshToken: res.refresh_token
                        });

                        console.log(res)

                        // Retry original request with new token
                        const newReq = req.clone({
                            setHeaders: {
                                Authorization: `Bearer ${res.access_token}`
                            },
                            url: req.url.startsWith('http') ? req.url : `${baseUrl}${req.url}`
                        });

                        return next(newReq);
                    }),
                    catchError((refreshErr) => {
                        Utils.setLocalStorage('auth', {});
                        router.navigate(['/auth/login']);
                        return throwError(() => refreshErr);
                    })
                );
            }

            return throwError(() => error);
        })
    );
}
