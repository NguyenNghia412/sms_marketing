import { IUpdateUserCredits, IViewUserCredits } from "@/models/user-credits.models";
import { IViewRowUser } from "@/models/user.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { UserService } from "@/services/user.service";
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
    private _userService = inject(UserService);


    listUsers: IViewRowUser[] = [];
    data: IViewUserCredits;
    userCreditsId: number;

     override form: FormGroup = new FormGroup({
        userId: new FormControl('', [Validators.required]),
        hanMucCredit: new FormControl('', [Validators.required]),
        thoiGianBatDauApDungHanMuc: new FormControl('', [Validators.required]),
        thoiGianKetThucApDungHanMuc: new FormControl(null),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
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
        this.userCreditsId = this._config.data?.id;
        this.getUserCreditsData();
        this.getListUsers();
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitUpdate();
    }
    onSubmitUpdate() {
            const body: IUpdateUserCredits = {
                id:this.userCreditsId,
                ...this.form.value
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
                    this.form.patchValue({
                        userId: this.data.user?.userId,
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

    getListUsers() {
        this.loading = true;
        this._userService.getListUsers().subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res)) {
                    this.listUsers = res.data || [];
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
