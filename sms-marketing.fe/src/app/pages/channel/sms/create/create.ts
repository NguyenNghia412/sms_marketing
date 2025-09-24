import { ICreateChienDich } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _chienDichService = inject(ChienDichService);

    override form: FormGroup = new FormGroup({
        tenChienDich: new FormControl('', [Validators.required]),
        ngayBatDau: new FormControl(new Date()),
        ngayKetThuc: new FormControl(new Date()),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenChienDich: {
            required: 'Không được bỏ trống'
        },
    };

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitCreate();
    }

    onSubmitCreate() {
        const body: ICreateChienDich = {
            ...this.form.value
        };
        this.loading = true;
        this._chienDichService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm chiến dịch')) {
                    this._ref?.close(true);
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

}
