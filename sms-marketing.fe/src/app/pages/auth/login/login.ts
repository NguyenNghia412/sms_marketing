import { AppFloatingConfigurator } from '@/layout/component/app.floatingconfigurator';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MessageService } from 'primeng/api';
import { environment } from 'src/environments/environment';
import { BaseComponent } from '@/shared/components/base/base-component';
import { AuthConstants } from '@/shared/constants/auth.constants';
import { Utils } from '@/shared/utils';

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

    override loading: boolean = false;
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
        const { codeChallenge, codeVerifier } = await Utils.generatePKCECodes();

        window.location.href = `${backendUrl}/login/google`;
    }

    async loginMs() {
        const backendUrl = environment.baseUrl;
        const redirectUri = `${environment.appUrl}/auth/callback`;
        const { codeChallenge, codeVerifier } = await Utils.generatePKCECodes();

        sessionStorage.setItem(AuthConstants.SESSION_PKCE_CODE_VERIFIER, codeVerifier);

        const url =
            `${backendUrl}/connect/authorize?` +
            `client_id=${encodeURIComponent(environment.authClientId)}` +
            `&redirect_uri=${encodeURIComponent(redirectUri)}` +
            `&response_type=code` +
            `&scope=openid offline_access` +
            `&prompt=login&code_challenge=${codeChallenge}&code_challenge_method=${AuthConstants.PKCE_CODE_CHALLENGE_METHOD}`;

        window.location.href = url;
    }

    
}
