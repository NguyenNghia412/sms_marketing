import { Component, inject } from '@angular/core';
import { SharedImports } from '../shared/shared.imports';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

export const ValidationMessages: Record<string, Record<string, string>> = {
  username: {
    required: 'Không được bỏ trống',
  },
  password: {
    required: 'Không được bỏ trống',
  },
};

@Component({
  selector: 'app-login',
  imports: [SharedImports, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

  private router = inject(Router);

  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // force show errors
      return;
    }
    console.log(this.loginForm.value);
    this.router.navigate(['dashboard']);
  }

  getErrorMessage(control: AbstractControl | null, fieldName: string): string | null {
    if (!control || !control.errors || !control.touched) return null;

    const errors = control.errors;
    const messages = ValidationMessages[fieldName];

    for (const errorKey of Object.keys(errors)) {
      if (messages[errorKey]) {
        return messages[errorKey];
      }
    }

    return null;
  }

  getError(field: string) {
    return this.getErrorMessage(this.loginForm.get(field), field);
  }

}
