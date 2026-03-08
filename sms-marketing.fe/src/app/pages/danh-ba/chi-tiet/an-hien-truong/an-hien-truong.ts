import { TruongDataItem } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-an-hien-truong',
    imports: [SharedImports],
    templateUrl: './an-hien-truong.html'
})
export class AnHienTruong extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _danhBaService = inject(DanhBaService);

    truongData: TruongDataItem[] = [];
    checkedMap: Record<number, boolean> = {};

    override ngOnInit(): void {
        const hiddenIds: number[] = this._config.data?.hiddenFieldIds || [];
        hiddenIds.forEach(id => this.checkedMap[id] = true);
        this.getTruongData();
    }

    getTruongData(): void {
        this.loading = true;
        this._danhBaService.getTruongDataDanhBaSms(this._config.data?.idDanhBa).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.truongData = res.data.truongData;
                    this.truongData.forEach(item => {
                        if (item.id !== undefined && !(item.id in this.checkedMap)) {
                            this.checkedMap[item.id] = false;
                        }
                    });
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

    onConfirm(): void {
        const hiddenIds = this.truongData
            .filter(item => item.id !== undefined && this.checkedMap[item.id])
            .map(item => item.id!);
        this._ref.close(hiddenIds);
    }

    onCancel(): void {
        this._ref.close(null);
    }
}
