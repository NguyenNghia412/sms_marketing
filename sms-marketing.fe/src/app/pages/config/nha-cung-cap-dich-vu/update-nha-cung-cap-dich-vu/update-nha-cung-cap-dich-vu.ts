import { IUpdateNhaCungCapDichVu } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { InputGroupModule } from "primeng/inputgroup";
import { InputGroupAddonModule } from "primeng/inputgroupaddon";
import { RadioButton } from "primeng/radiobutton";

@Component({
    selector: 'app-update-nha-cung-cap-dich-vu',
    imports: [SharedImports, InputGroupModule, InputGroupAddonModule, RadioButton],
    templateUrl: './update-nha-cung-cap-dich-vu.html',
    styleUrl: './update-nha-cung-cap-dich-vu.scss'
})
export class UpdateNhaCungCapDichVu extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);

    showApiKey = false;
    showApiSecret = false;
    nhaCungCapId: number;

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

    override ngOnInit(): void {
        this.nhaCungCapId = this._config.data?.id;
        this.getDataById();
    }

    getDataById() {
        this.loading = true;
        this._nhaCungCapDichVuService.getById(this.nhaCungCapId).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    const data = res.data;
                    this.form.patchValue({
                        name: data.name,
                        apiKey: data.apiKey,
                        apiSecret: data.apiSecret,
                        baseUrl: data.baseUrl,
                        isConfigAuthReq: data.isConfigAuthReq,
                        brandNames: Array.isArray(data.brandNames)
                            ? data.brandNames.map((b: any) => b.tenBrandName).join(';')
                            : data.brandNames
                    });
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }
        this.onSubmitUpdate();
    }

    onSubmitUpdate() {
        const formValue = this.form.value;
        const body: IUpdateNhaCungCapDichVu = {
            id: this.nhaCungCapId,
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
                          idBrandName: 0,
                          tenBrandName: s.trim(),
                          thoiGianBatDauHoatDong: undefined,
                          thoiGianKetThucHoatDong: undefined,
                      }))
                : [],
        };
        this.loading = true;
        this._nhaCungCapDichVuService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật nhà cung cấp dịch vụ')) {
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