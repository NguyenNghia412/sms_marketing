import { IBaseResponse } from '@/shared/models/request-paging.base.models';
import { Directive, inject, OnInit } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

@Directive()
export abstract class BaseComponent implements OnInit {
    protected router = inject(Router);
    protected _activatedRoute = inject(ActivatedRoute);
    protected messageService = inject(MessageService);
    protected _dialogService = inject(DialogService);

    ValidationMessages: Record<string, Record<string, string>> = {};
    MAX_PAGE_SIZE = 10;
    form!: FormGroup; // declare, but not initialize
    loading: boolean = false;

    ngOnInit(): void {}

    isFormInvalid() {
        if (this.form.invalid) {
            this.form.markAllAsTouched(); // force show errors
            return true;
        }
        return false;
    }

    getErrorMessage(control: AbstractControl | null, fieldName: string): string | null {
        if (!control || !control.errors || !control.touched) return null;

        const errors = control.errors;
        const messages = this.ValidationMessages[fieldName];

        for (const errorKey of Object.keys(errors)) {
            if (messages[errorKey]) {
                return messages[errorKey];
            }
        }

        return null;
    }

    getError(field: string) {
        return this.getErrorMessage(this.form.get(field), field);
    }

    isResponseSucceed(res: IBaseResponse, isShowErrorMsg = true, successMsg = '') {
        if (res && res.status === 1) {
            if (successMsg) {
                this.messageSuccess(successMsg);
            }
            return true;
        } else {
            if (isShowErrorMsg) {
                this.messageError(res.message || 'Có sự cố xảy ra. Vui lòng thử lại sau.');
            }
            return false;
        }
    }

    messageSuccess(msg: string) {
        this.messageService.add({
            closable: true,
            severity: 'success',
            detail: msg,
            life: 5000
        });
    }

    messageError(msg: string) {
        this.messageService.add({
            closable: true,
            severity: 'error',
            detail: msg || 'Có sự cố xảy ra. Vui lòng thử lại sau.',
            life: 5000
        });
    }
}
