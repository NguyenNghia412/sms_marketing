import { Utils } from '@/shared/utils';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { concatMap, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  http = inject(HttpClient)
  router = inject(Router);

  login(username: string, password: string) {
    const body = new HttpParams()
      .set('username', username)
      .set('password', password)
      .set('grant_type', 'password')
      .set('client_id', 'client-web')
      .set('client_secret', 'mBSQUHmZ4be5bQYfhwS7hjJZ2zFOCU2e')
      .set('scope', 'openid offline_access')
      ;

    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
      Accept: 'text/plain',
    });

    return this.http.post('http://localhost:5069/connect/token', body.toString(), { headers }).pipe(
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
