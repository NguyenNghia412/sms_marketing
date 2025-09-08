import { AppFloatingConfigurator } from '@/layout/component/app.floatingconfigurator';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MessageService } from 'primeng/api';
import { environment } from 'src/environments/environment';
import { BaseComponent } from '@/shared/components/base/base-component';

@Component({
    selector: 'app-login',
    imports: [...SharedImports, AppFloatingConfigurator],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login extends BaseComponent {
    private _authService = inject(AuthService);

    override form = new FormGroup({
        username: new FormControl('', [Validators.required]),
        password: new FormControl('', [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        username: {
            required: 'Không được bỏ trống'
        },
        password: {
            required: 'Không được bỏ trống'
        }
    };

    override  loading: boolean = false;
    errorMsg: string = '';

    onSubmit() {
        if (this.isFormInvalid()) return;

        this.errorMsg = '';
        this.loading = true;

        this._authService
            .login(this.form.value.username!, this.form.value.password!)
            .subscribe({
                next: (res) => {
                    this.router.navigate(['/']);
                },
                error: (resErr) => {
                    this.errorMsg = resErr?.error?.error_description || 'Có sự cố xảy ra. Vui lòng thử lại sau';
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    async redirectBackendAuthorize() {
        const backendUrl = environment.baseUrl;
        const clientId = environment.authClientId;
        const redirectUri = 'http://localhost:4200/auth/callback';
        const { codeChallenge, codeVerifier } = await this.generatePKCECodes();

        // window.location.href = `${backendUrl}/connect/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=code&scope=openid offline_access&code_challenge=${codeChallenge}&code_challenge_method=S256`;
        window.location.href = `${backendUrl}/login/google`;
    }

    base64UrlEncode(arrayBuffer: ArrayBuffer) {
        let str = String.fromCharCode(...new Uint8Array(arrayBuffer));
        return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    }

    async generatePKCECodes() {
        const array = new Uint8Array(32);
        crypto.getRandomValues(array);

        // code_verifier
        const codeVerifier = this.base64UrlEncode(array.buffer);

        // code_challenge = SHA256(code_verifier)
        const encoder = new TextEncoder();
        const data = encoder.encode(codeVerifier);
        const digest = await crypto.subtle.digest('SHA-256', data);
        const codeChallenge = this.base64UrlEncode(digest);

        return { codeVerifier, codeChallenge };
    }
}
