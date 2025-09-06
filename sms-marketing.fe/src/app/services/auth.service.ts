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
  http = inject(HttpClient)
  router = inject(Router);
  baseUrl = environment.baseUrl;

  login(username: string, password: string) {
    const body = new HttpParams()
      .set('username', username)
      .set('password', password)
      .set('grant_type', environment.authGrantType)
      .set('client_id', environment.authClientId)
      .set('client_secret', environment.authClientSecret)
      .set('scope', environment.authScope)
      ;

    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
      Accept: 'text/plain',
    });

    return this.http.post(`${this.baseUrl}/connect/token`, body.toString(), { headers }).pipe(
      concatMap((res: any) => {
        Utils.setLocalStorage('auth', {
          accessToken: res.access_token,
          refreshToken: res.refresh_token,
        });
        return of(res);
      })
    )
  }

  logout() {
    Utils.setLocalStorage('auth', '');
    this.router.navigate(['auth/login'])
  }
}
