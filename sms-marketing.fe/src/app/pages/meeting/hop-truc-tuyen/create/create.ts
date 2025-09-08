import { HopTrucTuyenService } from '@/services/hop-truc-tuyen.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ICreateHopTrucTuyen } from '@/models/hopTrucTuyen.models';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { Utils } from '@/shared/utils';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _hopTrucTuyenService = inject(HopTrucTuyenService);
    private ref = inject(DynamicDialogRef) 

    override form: FormGroup = new FormGroup({
        tenCuocHop: new FormControl('', [Validators.required]),
        moTa: new FormControl(''),
        linkCuocHop: new FormControl('', [Validators.required]),
        thoiGianBatDau: new FormControl(new Date(), [Validators.required]),
        thoiGianKetThuc: new FormControl(new Date(), [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenCuocHop: {
            required: 'Không được bỏ trống'
        },
        linkCuocHop: {
            required: 'Không được bỏ trống'
        },
        thoiGianBatDau: {
            required: 'Không được bỏ trống'
        },
        thoiGianKetThuc: {
            required: 'Không được bỏ trống'
        }
    };

    onSubmit() {
        if (this.isFormInvalid()) return;

        const body: ICreateHopTrucTuyen = this.form.value;
        body.thoiGianBatDau = Utils.formatDateCallApi(body.thoiGianBatDau)
        body.thoiGianKetThuc = Utils.formatDateCallApi(body.thoiGianKetThuc)

        this._hopTrucTuyenService.create(body).subscribe(
            (res) => {
                if (this.isResponseSucceed(res, true, 'Tạo phòng thành công')) {
                  this.ref?.close(true);
                }
            },
            (err) => {
                this.messageError(err?.message);
            }
        );
    }
}
