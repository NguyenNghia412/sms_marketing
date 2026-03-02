import { IUpdateUserToNhaCungCapDichVuDto } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";

@Component({
    selector: 'app-update-user-su-dung-dich-vu',
    imports: [SharedImports],
    templateUrl: './update-user-su-dung-dich-vu.html',
    styleUrl: './update-user-su-dung-dich-vu.scss'
})
export class UpdateUserSuDungDichVu extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);

    idUserNhaCungCapDichVu: number = 0;
    fullName: string = '';
    userName: string = '';
    brandName: string = '';

    override form: FormGroup = new FormGroup({
        thoiGianBatDauSuDungDichVu: new FormControl(null),
        thoiGianKetThucSuDungDichVu: new FormControl(null),
    });

    override ngOnInit(): void {
        this.idUserNhaCungCapDichVu = this._config.data?.id || 0;
        this.getDataById();
    }

    getDataById() {
        this.loading = true;
        this._nhaCungCapDichVuService.findById(this.idUserNhaCungCapDichVu).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    const data = res.data;
                    this.fullName = data.user?.fullName || '';
                    this.userName = data.user?.userName || '';
                    this.brandName = data.brandName?.tenBrandName || '';
                    this.form.patchValue({
                        thoiGianBatDauSuDungDichVu: data.thoiGianBatDauSuDungDichVu ? new Date(data.thoiGianBatDauSuDungDichVu) : null,
                        thoiGianKetThucSuDungDichVu: data.thoiGianKetThucSuDungDichVu ? new Date(data.thoiGianKetThucSuDungDichVu) : null,
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
        const body: IUpdateUserToNhaCungCapDichVuDto = {
            idUserNhaCungCapDichVu: this.idUserNhaCungCapDichVu,
            thoiGianBatDauSuDungDichVu: formValue.thoiGianBatDauSuDungDichVu || null,
            thoiGianKetThucSuDungDichVu: formValue.thoiGianKetThucSuDungDichVu || null,
        };
        this.loading = true;
        this._nhaCungCapDichVuService.updateUserToNhaCungCapDichVu(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật thông tin')) {
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
