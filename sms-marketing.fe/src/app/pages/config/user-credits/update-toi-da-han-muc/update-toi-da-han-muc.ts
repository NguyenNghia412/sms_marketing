import { IUpdateToiDaHanMucCreditsGiaHan } from "@/models/user-credits.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { DynamicDialogRef, DynamicDialogConfig } from "primeng/dynamicdialog";

@Component({
    selector: 'app-update-toi-da-han-muc',
    imports: [SharedImports],
    templateUrl: './update-toi-da-han-muc.html',
    styleUrl: './update-toi-da-han-muc.scss'
})
export class UpdateToiDaHanMuc extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _userCreditsService = inject(UserCreditsService);

    userCreditsId: number = 0;
    donVi: string = 'VND';

    override form: FormGroup = new FormGroup({
        toiDaHanMucCreditGiaHan: new FormControl('', [Validators.required]),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        toiDaHanMucCreditGiaHan: {
            required: 'Không được bỏ trống'
        },
    };

    override ngOnInit(): void {
        this.userCreditsId = this._config.data?.id ?? 1;
        this.getToiDaHanMucData();
    }

    getToiDaHanMucData() {
        this.loading = true;
        this._userCreditsService.getToiDaHanMucCreditsGiaHan(this.userCreditsId).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.donVi = res.data?.donVi || 'VND';
                    this.form.patchValue({
                        toiDaHanMucCreditGiaHan: res.data?.toiDaHanMucCreditGiaHan || '',
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

        const body: IUpdateToiDaHanMucCreditsGiaHan = {
            id: this.userCreditsId,
            toiDaHanMucCreditGiaHan: this.form.value.toiDaHanMucCreditGiaHan?.toString() || '',
            donVi: this.donVi,
        };

        this.loading = true;
        this._userCreditsService.updateToiDaHanMucCreditsGiaHan(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật tối đa hạn mức gia hạn')) {
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
