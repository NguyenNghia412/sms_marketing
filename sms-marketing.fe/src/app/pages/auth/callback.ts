import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { AppFloatingConfigurator } from '../../layout/component/app.floatingconfigurator';

@Component({
    selector: 'app-callback',
    standalone: true,
    template: ``
})
export class AuthCallback implements OnInit {
    ngOnInit(): void {
        const query = window.location.search.substring(1); // ?code=...&state=...
        this.handleAuthCallback(query);
    }

    handleAuthCallback(query: string) {
        console.log('query', query);
        const params = new URLSearchParams(query);
        const code = params.get('code');

        if (!code) {
            console.error('No authorization code found');
            return;
        }

        // Exchange code for tokens
        const body = new URLSearchParams({
            grant_type: 'authorization_code',
            code: code,
            redirect_uri: 'http://localhost:4200/callback',
            client_id: 'angular-client'
        });

        //   this.http.post<any>('http://localhost:5000/connect/token', body).subscribe({
        //     next: (tokens) => {
        //       localStorage.setItem('access_token', tokens.access_token);
        //       localStorage.setItem('refresh_token', tokens.refresh_token);
        //       this.router.navigate(['/']);
        //     },
        //     error: (err) => console.error('Token exchange failed', err)
        //   });
    }
}
