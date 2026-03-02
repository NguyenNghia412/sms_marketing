import { IUpdateDanhBaSmsRequest, IViewChiTietDanhBaSms } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-update-thue-bao',
    imports: [SharedImports],
    templateUrl: './update-thue-bao.html',
    styleUrl: './update-thue-bao.scss'
})
export class UpdateThueBao extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _danhBaService = inject(DanhBaService);

    data?: IViewChiTietDanhBaSms;
    idDanhBa: number = 0;
    idThueBao: number = 0;

    override form: FormGroup = new FormGroup({
        hoVaTen: new FormControl('', [Validators.required]),
        soDienThoai: new FormControl('', [Validators.required]),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        hoVaTen: {
            required: 'Không được bỏ trống'
        },
        soDienThoai: {
            required: 'Không được bỏ trống'
        },
    };

    override ngOnInit(): void {
        this.idDanhBa = this._config.data?.idDanhBa;
        this.idThueBao = this._config.data?.idThueBao;
        this.getData();
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        const body: IUpdateDanhBaSmsRequest = {
            idDanhBa: this.idDanhBa,
            id: this.idThueBao,
            hoVaTen: this.form.value.hoVaTen || '',
            soDienThoai: this.form.value.soDienThoai || '',
        };

        this.loading = true;
        this._danhBaService.updateThueBaoById(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật thành công')) {
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

    getData() {
        this.loading = true;
        this._danhBaService.getThueBaoById(this.idDanhBa, this.idThueBao).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data;
                    this.form.patchValue({
                        hoVaTen: this.data?.hoVaTen,
                        soDienThoai: this.data?.soDienThoai,
                    });
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}