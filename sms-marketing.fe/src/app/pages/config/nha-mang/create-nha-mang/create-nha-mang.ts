import { ICreateNhaMang } from "@/models/nha-mang.models";
import { IDropDownNhaCungCapDichVu, IGetListBrandNameResponseDto } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
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
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);

    listBrandName: IGetListBrandNameResponseDto[] = [];
    listNhaCungCapDichVu: IDropDownNhaCungCapDichVu[] = [];

    override form: FormGroup = new FormGroup({
        idNhaCungCapDichVu: new FormControl(null, [Validators.required]),
        tenNhaMang: new FormControl('', [Validators.required]),
        prefix: new FormControl('', [Validators.required]),
        idBrandName: new FormControl('', [Validators.required]),
        donGia: new FormControl('', [Validators.required]),
        thoiHan: new FormControl(null),
    });
    override ValidationMessages: Record<string, Record<string, string>> = {
        idNhaCungCapDichVu: {
            required: 'Không được bỏ trống'
        },
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
        this.getListNhaCungCapDichVu();
    }

    getListNhaCungCapDichVu() {
        this._nhaCungCapDichVuService.getDropdown().subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listNhaCungCapDichVu = res.data;
                }
            }
        });
    }

    onNhaCungCapChange(idNhaCungCapDichVu: number) {
        this.form.get('idBrandName')?.reset();
        this.listBrandName = [];
        if (idNhaCungCapDichVu) {
            this.getListBrandName(idNhaCungCapDichVu);
        }
    }

    getListBrandName(idNhaCungCapDichVu: number) {
        this._nhaCungCapDichVuService.getListBrandName(idNhaCungCapDichVu).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listBrandName = res.data;
                }
            }
        });
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

}
