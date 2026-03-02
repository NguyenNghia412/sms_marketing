import { IUpdateUserCredits, IViewUserCredits } from "@/models/user-credits.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogRef, DynamicDialogConfig } from "primeng/dynamicdialog";

@Component({
    selector: 'app-update-user-credits',
    imports: [SharedImports],
    templateUrl: './update-user-credits.html',
    styleUrl: './update-user-credits.scss'
})
export class UpdateUserCredits extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _userCreditsService = inject(UserCreditsService);

    data: IViewUserCredits;
    userCreditsId: number;
    fullName: string = '';
    userName: string = '';
    tenNhaCungCapDichVu: string = '';
    tenBrandName: string = '';

     override form: FormGroup = new FormGroup({
        hanMucCredit: new FormControl('', [Validators.required]),
        thoiGianBatDauApDungHanMuc: new FormControl(null, [Validators.required]),
        thoiGianKetThucApDungHanMuc: new FormControl(null),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        hanMucCredit: {
            required: 'Không được bỏ trống'
        },
        thoiGianBatDauApDungHanMuc: {
            required: 'Không được bỏ trống'
        },
    };

    override ngOnInit(): void {
        this.userCreditsId = this._config.data?.id;
        this.getUserCreditsData();
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitUpdate();
    }
    toLocalISOString(date: Date | null): string | null {
        if (!date) return null;
        const offset = date.getTimezoneOffset();
        const local = new Date(date.getTime() - offset * 60000);
        return local.toISOString().slice(0, 19);
    }

    onSubmitUpdate() {
            const body: IUpdateUserCredits = {
                id: this.userCreditsId,
                hanMucCredit: this.form.value.hanMucCredit?.toString() || '',
                thoiGianBatDauApDungHanMuc: this.toLocalISOString(this.form.value.thoiGianBatDauApDungHanMuc) as any,
                thoiGianKetThucApDungHanMuc: this.toLocalISOString(this.form.value.thoiGianKetThucApDungHanMuc) as any,
            };
            this.loading = true;
            this._userCreditsService.updateUserCredits(body).subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, true, 'Đã cập nhật hạn mức credit người dùng')) {
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

    getUserCreditsData() {
        this.loading = true;
        this._userCreditsService.getById(this.userCreditsId).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data;
                    this.fullName = this.data.user?.fullName || '';
                    this.userName = this.data.user?.userName || '';
                    this.tenNhaCungCapDichVu = this.data.nhaCungCapDichVus?.map(ncc => ncc.tenNhaCungCapDichVu).join('; ') || '';
                    this.tenBrandName = this.data.nhaCungCapDichVus?.flatMap(ncc => ncc.brandNames?.map(bn => bn.tenBrandName) || []).join('; ') || '';
                    this.form.patchValue({
                        hanMucCredit: this.data.hanMucCredit,
                        thoiGianBatDauApDungHanMuc: this.data.thoiGianBatDauApDungHanMuc ? new Date(this.data.thoiGianBatDauApDungHanMuc) : null,
                        thoiGianKetThucApDungHanMuc: this.data.thoiGianKetThucApDungHanMuc ? new Date(this.data.thoiGianKetThucApDungHanMuc) : null
                    });
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}
