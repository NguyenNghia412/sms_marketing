import { IViewRowDanhBa } from '@/models/danh-ba.models';
import { ICreateChienDich, IUpdateChienDich, IViewBrandname, IViewChienDich } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _danhBaService = inject(DanhBaService);
    private _chienDichService = inject(ChienDichService);

    override form: FormGroup = new FormGroup({
        id: new FormControl(null),
        idBrandName: new FormControl(null, [Validators.required]),
        idDanhBa: new FormControl(null, [Validators.required]),
        tenChienDich: new FormControl('', [Validators.required]),
        ngayBatDau: new FormControl(new Date()),
        ngayKetThuc: new FormControl(new Date()),
        moTa: new FormControl(''),
        noiDung: new FormControl('', [Validators.required]),
        isFlashSms: new FormControl(true)
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        idBrandName: {
            required: 'Không được bỏ trống'
        },
        idDanhBa: {
            required: 'Không được bỏ trống'
        },
        tenChienDich: {
            required: 'Không được bỏ trống'
        },
        noiDung: {
            required: 'Không được bỏ trống'
        }
    };

    listDanhBa: IViewRowDanhBa[] = [];
    listBrandname: IViewBrandname[] = [];

    override ngOnInit(): void {
        this._danhBaService.getList().subscribe({
            next: (value) => {
                this.listDanhBa = value.data;
            }
        });
        this._chienDichService.getListBrandname().subscribe({
            next: (value) => {
                this.listBrandname = value.data;
            }
        });

        if (this.isUpdate) {
            const oldChienDich: IViewChienDich = { ...this._config.data };

            let idDanhBa = 0;

            if (oldChienDich.danhBas && oldChienDich.danhBas.length > 0) {
                idDanhBa = oldChienDich.danhBas[0].idDanhBa;
            }
            this.form.setValue({
                id: oldChienDich.id,
                idBrandName: oldChienDich.idBrandName,
                tenChienDich: oldChienDich.tenChienDich,
                ngayBatDau: oldChienDich.ngayBatDau,
                ngayKetThuc: oldChienDich.ngayKetThuc,
                moTa: oldChienDich.moTa,
                isFlashSms: oldChienDich.isFlashSms,
                noiDung: oldChienDich.noiDung,
                idDanhBa
            });
        }
    }

    get isUpdate() {
        return this._config.data?.id;
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }
        if (this.isUpdate) {
            this.onSubmitUpdate();
        } else {
            this.onSubmitCreate();
        }
    }

    onSubmitCreate() {
        const body: ICreateChienDich = {
            ...this.form.value
        };
        this.loading = true;
        this._chienDichService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm chiến dịch')) {
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

    onSubmitUpdate() {
        const body: IUpdateChienDich = {
            ...this.form.value
        };
        this.loading = true;
        this._chienDichService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật')) {
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
}
