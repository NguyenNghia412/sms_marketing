import { IAddUserToNhaCungCapDichVuDto, IGetListBrandNameResponseDto } from "@/models/nha-cung-cap-dich-vu.models";
import { IGetListUsers } from "@/models/user.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { UserService } from "@/services/user.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";

@Component({
    selector: 'app-create-user-su-dung-dich-vu',
    imports: [SharedImports],
    templateUrl: './create-user-su-dung-dich-vu.html',
    styleUrl: './create-user-su-dung-dich-vu.scss'
})
export class CreateUserSuDungDichVu extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);
    private _userService = inject(UserService);

    idNhaCungCapDichVu: number = 0;
    listBrandName: IGetListBrandNameResponseDto[] = [];
    listUsers: IGetListUsers[] = [];

    override form: FormGroup = new FormGroup({
        idUser: new FormControl(null, [Validators.required]),
        idBrandName: new FormControl([], [Validators.required]),
        thoiGianBatDauSuDungDichVu: new FormControl(null),
        thoiGianKetThucSuDungDichVu: new FormControl(null),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        idUser: {
            required: 'Không được bỏ trống'
        },
        idBrandName: {
            required: 'Không được bỏ trống'
        },
    };

    override ngOnInit(): void {
        this.idNhaCungCapDichVu = this._config.data?.idNhaCungCapDichVu || 0;
        this.getListBrandName();
        this.getListUsers();
    }

    getListBrandName() {
        this._nhaCungCapDichVuService.getListBrandName(this.idNhaCungCapDichVu).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listBrandName = res.data;
                }
            },
        });
    }

    getListUsers() {
        this._userService.getListUsers().subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listUsers = res.data;
                }
            },
        });
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }
        this.onSubmitCreate();
    }

    onSubmitCreate() {
        const formValue = this.form.value;
        const body: IAddUserToNhaCungCapDichVuDto = {
            idNhaCungCapDichVu: this.idNhaCungCapDichVu,
            idUser: formValue.idUser,
            idBrandName: formValue.idBrandName,
            thoiGianBatDauSuDungDichVu: formValue.thoiGianBatDauSuDungDichVu || null,
            thoiGianKetThucSuDungDichVu: formValue.thoiGianKetThucSuDungDichVu || null,
        };
        this.loading = true;
        this._nhaCungCapDichVuService.addUserToNhaCungCapDichVu(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm user vào nhà cung cấp dịch vụ')) {
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
