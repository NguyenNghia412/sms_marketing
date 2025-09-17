import { ICreateDotDiemDanh } from '@/models/dot-diem-danh.models';
import { DotDiemDanhService } from '@/services/dot-diem-danh.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Utils } from '@/shared/utils';
import { Component, inject } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _dotDiemDanhService = inject(DotDiemDanhService);
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);

    override form: FormGroup = new FormGroup({
        tenDotDiemDanh: new FormControl('', [Validators.required]),
        tenMonHoc: new FormControl(''),
        maMonHoc: new FormControl(''),
        thoiGianBatDau: new FormControl(new Date(), [Validators.required]),
        thoiGianKetThuc: new FormControl(new Date(), [Validators.required]),
        ghiChu: new FormControl('')
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenDotDiemDanh: {
            required: 'Không được bỏ trống'
        },
        thoiGianBatDau: {
            required: 'Không được bỏ trống'
        },
        thoiGianKetThuc: {
            required: 'Không được bỏ trống'
        }
    };

    get idCuocHop() {
      return this._config.data?.idCuocHop
    }

    onSubmit() {
        if (this.isFormInvalid()) return;

        const body: ICreateDotDiemDanh = this.form.value;
        body.idCuocHop = this.idCuocHop;
        body.thoiGianBatDau = Utils.formatDateCallApi(body.thoiGianBatDau);
        body.thoiGianKetThuc = Utils.formatDateCallApi(body.thoiGianKetThuc);

        this._dotDiemDanhService.create(body).subscribe(
            (res) => {
                if (this.isResponseSucceed(res, true, 'Tạo đợt điểm danh thành công')) {
                    this._ref?.close(true);
                }
            },
            (err) => {
                this.messageError(err?.message);
            }
        );
    }
}
