import { ICreateDanhBaSmsQuick } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create-quick-sms',
    imports: [SharedImports],
    templateUrl: './create-quick-sms.html',
    styleUrl: './create-quick-sms.scss'
})
export class CreateQuickSms extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);

    override form: FormGroup = new FormGroup({
        Data: new FormControl(null, [Validators.required]),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        Data: {
            required: 'Không được bỏ trống'
        },
    };

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        const formValue = this.form.value;

        const body: ICreateDanhBaSmsQuick = {
            data: formValue.Data!.split('\n').filter((line: string) => line.trim())
        };

        this.loading = true;
        this._danhBaService.createDanhBaSmsQuick(this._config.data?.idDanhBa, body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.messageSuccess('Thêm người nhận thành công');
                    this._ref.close(true);
                }
            },
            error: (err) => {
                this.messageError(err?.message || 'Có lỗi xảy ra');
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    onCancel() {
        this._ref.close();
    }
}