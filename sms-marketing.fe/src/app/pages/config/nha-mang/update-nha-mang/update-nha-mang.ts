import { IUpdateNhaMang, IViewNhaMang } from "@/models/nha-mang.models";
import { IViewBrandname } from "@/models/sms.models";
import { ChienDichService } from "@/services/chien-dich.service";
import { NhaMangService } from "@/services/nha-mang.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";

@Component({
    selector: 'app-update-nha-mang',
    imports: [SharedImports],
    templateUrl: './update-nha-mang.html',
    styleUrl: './update-nha-mang.scss'
})
export class UpdateNhaMang extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _nhaMangService = inject(NhaMangService);
    private _chienDichService = inject(ChienDichService);

    listBrandName: IViewBrandname[] = [];
    data: IViewNhaMang;
    nhaMangId: number;

    override form: FormGroup = new FormGroup({
        tenNhaMang: new FormControl('', [Validators.required]),
        prefix: new FormControl('', [Validators.required]),
        idBrandName: new FormControl('', [Validators.required]),
        donGia: new FormControl('', [Validators.required]),
        thoiHan: new FormControl(null),
    });
    override ValidationMessages: Record<string, Record<string, string>> = {
        tenNhaMang: {
            required: 'Không được bỏ trống'
        },
        prefix: {
            required: 'Không được bỏ trống'
        },
        idBrandName: {
            required: 'Không được bỏ trống'
        },
        donGia: {
            required: 'Không được bỏ trống'
        },
    };
    override ngOnInit(): void {
        this.nhaMangId = this._config.data?.id;
        this.getListBrandName();
        this.getNhaMangData();
    }

    getListBrandName() {
        this._chienDichService.getListBrandname().subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listBrandName = res.data;
                }
            }
        });
    }
    getNhaMangData() {
        this.loading = true;
        this._nhaMangService.getById(this.nhaMangId).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data;
                    this.form.patchValue({
                        tenNhaMang: this.data.tenNhaMang,
                        prefix: this.data.prefix,
                        idBrandName: this.data.brandName?.id,
                        donGia: this.data.donGia?.donGia,
                        thoiHan: this.data.donGia?.thoiHan
                    });
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
    onCancel() {
        this._ref.close();
    }
    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitUpdate();
    }
    onSubmitUpdate() {
        const body: IUpdateNhaMang = {
            id: this.nhaMangId,
            ...this.form.value
        };
        this.loading = true;
        this._nhaMangService.updateNhaMang(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật thông tin nhà mạng')) {
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