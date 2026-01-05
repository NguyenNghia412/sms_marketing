import { ICreateUserCredits } from "@/models/user-credits.models";
import { IViewRowUser } from "@/models/user.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { UserService } from "@/services/user.service";
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
    private _userService = inject(UserService);

    listUsers : IViewRowUser[] = [];
    donVi: string = 'VNĐ';

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
        this.getListUsers();
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        this.onSubmitCreate();
    }

    

    onSubmitCreate() {
            const body: ICreateUserCredits = {
                userId: this.form.value.userId,
                hanMucCredit: this.form.value.hanMucCredit?.toString() || '',
                thoiGianBatDauApDungHanMuc: this.form.value.thoiGianBatDauApDungHanMuc,
                thoiGianKetThucApDungHanMuc: this.form.value.thoiGianKetThucApDungHanMuc,
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