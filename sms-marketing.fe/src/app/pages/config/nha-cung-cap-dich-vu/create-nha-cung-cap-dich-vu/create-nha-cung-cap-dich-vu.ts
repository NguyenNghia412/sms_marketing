import { ICreateNhaCungCapDichVu } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogRef } from "primeng/dynamicdialog";
import { InputGroupModule } from "primeng/inputgroup";
import { InputGroupAddonModule } from "primeng/inputgroupaddon";
import { RadioButton } from "primeng/radiobutton";

@Component({
    selector: 'app-create-nha-cung-cap-dich-vu',
    imports: [SharedImports, InputGroupModule, InputGroupAddonModule, RadioButton],
    templateUrl: './create-nha-cung-cap-dich-vu.html',
    styleUrl: './create-nha-cung-cap-dich-vu.scss'
})
export class CreateNhaCungCapDichVu extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);
    showApiKey = false;
    showApiSecret = false;

    override form: FormGroup = new FormGroup({
        name: new FormControl('', [Validators.required]),
        apiKey: new FormControl('', [Validators.required]),
        apiSecret: new FormControl('', [Validators.required]),
        baseUrl: new FormControl('', [Validators.required]),
        brandNames: new FormControl(''),
        isConfigAuthReq: new FormControl(false),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        name: {
            required: 'Không được bỏ trống'
        },
        apiKey: {
            required: 'Không được bỏ trống'
        },
        apiSecret: {
            required: 'Không được bỏ trống'
        },
        baseUrl: {
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
        const formValue = this.form.value;
        const body: ICreateNhaCungCapDichVu = {
            name: formValue.name,
            apiKey: formValue.apiKey,
            apiSecret: formValue.apiSecret,
            baseUrl: formValue.baseUrl,
            isConfigAuthReq: formValue.isConfigAuthReq,
            brandNames: formValue.brandNames
                ? formValue.brandNames
                      .split(';')
                      .filter((s: string) => s.trim())
                      .map((s: string) => ({
                          tenBrandName: s.trim(),
                          thoiGianBatDauHoatDong: null,
                          thoiGianKetThucHoatDong: null,
                      }))
                : [],
        };
        this.loading = true;
        this._nhaCungCapDichVuService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm nhà cung cấp dịch vụ')) {
                    this._ref?.close(true);
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            },
        });
    }

    onCancel() {
        this._ref.close();
    }
}