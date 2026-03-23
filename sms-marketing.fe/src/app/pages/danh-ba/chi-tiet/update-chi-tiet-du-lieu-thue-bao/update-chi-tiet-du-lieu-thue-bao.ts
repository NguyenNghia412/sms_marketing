import { IUpdateDataChiTietThueBaoRequest, IViewChiTietThueBaoNguoiNhan, IViewChiTietThueBaoNguoiNhanDataById } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-update-chi-tiet-du-lieu-thue-bao',
    imports: [SharedImports],
    templateUrl: './update-chi-tiet-du-lieu-thue-bao.html',
    styleUrl: './update-chi-tiet-du-lieu-thue-bao.scss'
})
export class UpdateChiTietDuLieuThueBao extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _danhBaService = inject(DanhBaService);

    data?: IViewChiTietThueBaoNguoiNhan;
    items: IViewChiTietThueBaoNguoiNhanDataById[] = [];
    idDanhBa: number = 0;
    idThueBao: number = 0;

    override ngOnInit(): void {
        this.idDanhBa = this._config.data?.idDanhBa;
        this.idThueBao = this._config.data?.idThueBao;
        this.getData();
    }

    onSubmit() {
        const body: IUpdateDataChiTietThueBaoRequest = {
            idDanhBa: this.idDanhBa,
            idThueBao: this.idThueBao,
            items: this.items.map(item => ({
                idData: item.idData,
                data: item.data
            }))
        };

        this.loading = true;
        this._danhBaService.updateChiTietThueBaoById(body).subscribe({
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
        this._danhBaService.getChiTietThueBaoById(this.idDanhBa, this.idThueBao).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data;
                    this.items = res.data.items || [];
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}
