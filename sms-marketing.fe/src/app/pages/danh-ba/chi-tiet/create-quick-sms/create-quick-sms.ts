import { ICreateDanhBaSmsQuick, TruongDataItem } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ChipModule } from 'primeng/chip';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create-quick-sms',
    imports: [SharedImports, ChipModule],
    templateUrl: './create-quick-sms.html',
    styleUrl: './create-quick-sms.scss'
})
export class CreateQuickSms extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);

    truongData: TruongDataItem[] = [];

    override form: FormGroup = new FormGroup({
        IndexTruongHoTen: new FormControl(null, [Validators.required, Validators.min(0)]),
        IndexTruongSoDienThoai: new FormControl(null, [Validators.required, Validators.min(0)]),
        Data: new FormControl(null, [Validators.required]),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        IndexTruongHoTen: {
            required: 'Không được bỏ trống',
            min: 'Giá trị phải lớn hơn hoặc bằng 1'
        },
        IndexTruongSoDienThoai: {
            required: 'Không được bỏ trống',
            min: 'Giá trị phải lớn hơn hoặc bằng 1'
        },
        Data: {
            required: 'Không được bỏ trống'
        },
    };

    override ngOnInit(): void {
        this.getTruongData();
    }

    getTruongData() {
        this.loading = true;
        this._danhBaService.getTruongDataDanhBaSms(this._config.data?.idDanhBa).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.truongData = res.data.truongData;
                }
            },
            error: (err) => {
                this.messageError(err?.message || 'Có lỗi khi tải danh sách trường');
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

        const formValue = this.form.value;

        const body: ICreateDanhBaSmsQuick = {
            indexTruongHoTen: formValue.IndexTruongHoTen,
            indexTruongSoDienThoai: formValue.IndexTruongSoDienThoai,
            data: formValue.Data!.split('\n').filter((line: string) => line.trim())
        };

        this.loading = true;
        this._danhBaService.createDanhBaSmsQuick(this._config.data?.idDanhBa, body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.messageSuccess('Thêm người nhận thành công');
                    this._ref.close(true);
                }
            },
            error: (err) => {
                this.messageError(err?.message || 'Có lỗi xảy ra');
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