import { ICreateDanhBa, IUpdateDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _danhBaService = inject(DanhBaService);

    override form: FormGroup = new FormGroup({
        id: new FormControl(null),
        tenDanhBa: new FormControl('', [Validators.required]),
        mota: new FormControl(''),
        ghiChu: new FormControl('')
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenDanhBa: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
        if (this.isUpdate) {
            this.form.setValue(this._config.data);
        }
    }

    get isUpdate() {
        return this._config.data?.id;
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

       if (this.isUpdate) {
        this.onSubmitUpdate();
       } else {
        this.onSubmitCreate();
       }
    }

    onSubmitCreate() {
        const body: ICreateDanhBa = {
            ...this.form.value
        };
        this.loading = true;
        this._danhBaService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm danh bạ')) {
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

    onSubmitUpdate() {
        const body: IUpdateDanhBa = {
            ...this.form.value
        };
        this.loading = true;
        this._danhBaService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã lưu')) {
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
