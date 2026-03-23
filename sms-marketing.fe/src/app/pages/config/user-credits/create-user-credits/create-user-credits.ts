import { ICreateUserCredits } from "@/models/user-credits.models";
import { IDropDownNhaCungCapDichVu, IGetListDropDownUserNhaCungCapDichVuDto } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { UserCreditsService } from "@/services/user-credits.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { DynamicDialogRef } from "primeng/dynamicdialog";



@Component({
    selector: 'app-create-user-credits',
    imports: [SharedImports],
    templateUrl: './create-user-credits.html',
    styleUrl: './create-user-credits.scss'
})
export class CreateUserCredits extends BaseComponent{
    private _ref = inject(DynamicDialogRef);
    private _userCreditsService = inject(UserCreditsService);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);

    listNhaCungCapDichVu: IDropDownNhaCungCapDichVu[] = [];
    listUsers: IGetListDropDownUserNhaCungCapDichVuDto[] = [];

    override form: FormGroup = new FormGroup({
        idNhaCungCapDichVu: new FormControl(null, [Validators.required]),
        userId: new FormControl(null, [Validators.required]),
        hanMucCredit: new FormControl('', [Validators.required]),
        thoiGianBatDauApDungHanMuc: new FormControl(null, [Validators.required]),
        thoiGianKetThucApDungHanMuc: new FormControl(null),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        idNhaCungCapDichVu: {
            required: 'Không được bỏ trống'
        },
        userId: {
            required: 'Không được bỏ trống'
        },
        hanMucCredit: {
            required: 'Không được bỏ trống'
        },
        thoiGianBatDauApDungHanMuc: {
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
        this.form.get('userId')?.reset();
        this.listUsers = [];
        if (idNhaCungCapDichVu) {
            this.getListUsers(idNhaCungCapDichVu);
        }
    }

    getListUsers(idNhaCungCapDichVu: number) {
        this._nhaCungCapDichVuService.getListDropDownUserNhaCungCapDichVu(idNhaCungCapDichVu).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.listUsers = res.data;
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

    toLocalISOString(date: Date | null): string | null {
        if (!date) return null;
        const offset = date.getTimezoneOffset();
        const local = new Date(date.getTime() - offset * 60000);
        return local.toISOString().slice(0, 19);
    }

    onSubmitCreate() {
            const body: ICreateUserCredits = {
                userId: this.form.value.userId,
                idNhaCungCapDichVu: this.form.value.idNhaCungCapDichVu,
                hanMucCredit: this.form.value.hanMucCredit?.toString() || '',
                thoiGianBatDauApDungHanMuc: this.toLocalISOString(this.form.value.thoiGianBatDauApDungHanMuc) as any,
                thoiGianKetThucApDungHanMuc: this.toLocalISOString(this.form.value.thoiGianKetThucApDungHanMuc) as any,
            };
            this.loading = true;
            this._userCreditsService.createUserCredits(body).subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, true, 'Đã thêm hạn mức người dùng thành công!')) {
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