import { ICreateNhaMang } from "@/models/nha-mang.models";
import { IViewBrandname } from "@/models/sms.models";
import { ChienDichService } from "@/services/chien-dich.service";
import { NhaMangService } from "@/services/nha-mang.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";

import { DynamicDialogRef } from "primeng/dynamicdialog";

@Component({
    selector: 'app-create-nha-mang',
    imports: [SharedImports],
    templateUrl: './create-nha-mang.html',
    styleUrl: './create-nha-mang.scss'
})
export class CreateNhaMang extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _nhaMangService = inject(NhaMangService);
    private _chienDichService = inject(ChienDichService);

    listBrandName :IViewBrandname[] = [];

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
        this.getListBrandName();
    }
    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitCreate();
    }

    onSubmitCreate() {
            const body: ICreateNhaMang = {
                ...this.form.value
            };
            this.loading = true;
            this._nhaMangService.createNhaMang(body).subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, true, 'Đã thêm nhà mạng')) {
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
        onCancel() {
            this._ref.close();
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

}
    
