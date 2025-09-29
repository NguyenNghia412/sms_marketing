import { DanhBaService } from "@/services/danh-ba.service";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { ICreateDanhBaChienDichQuick } from "@/models/danh-ba.models";

@Component({
    selector: 'app-create-danh-ba-quick',
    imports: [SharedImports],
    templateUrl: './create-quick.html',
    styleUrl: './create-quick.scss'
})
export class CreateQuick extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);

    loaiDanhBaOptions = [
        { label: 'SMS', value: 1 },
        { label: 'Email', value: 2 }
    ];

    override form: FormGroup = new FormGroup({
        TenDanhBa: new FormControl(null, [Validators.required]),
        Type: new FormControl(1, [Validators.required]),
        TruongData: new FormControl(null, [Validators.required]),
        Data: new FormControl(null, [Validators.required]),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        TenDanhBa: {
            required: 'Không được bỏ trống'
        },
        Type: {
            required: 'Không được bỏ trống'
        },
        TruongData: {
            required: 'Không được bỏ trống'
        },
        Data: {
            required: 'Không được bỏ trống'
        },
    };

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        const formValue = this.form.value;

        const body: ICreateDanhBaChienDichQuick = {
            tenDanhBa: formValue.TenDanhBa!,
            type: formValue.Type!,
            truong: [formValue.TruongData!],
            data: formValue.Data!.split('\n').filter((line: string) => line.trim())
        };

        this.loading = true;
        this._danhBaService.createDanhBaChienDichQuick(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.messageSuccess('Tạo danh bạ thành công');
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