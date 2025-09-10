import { IViewRowDotDiemDanh } from '@/models/dot-diem-danh.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, Input, output } from '@angular/core';

export const TblActionTypes = {
    diemDanh: 'diemDanh',
    downloadQr: 'downloadQr',
    delete: 'delete',
    update: 'update',
}

@Component({
    selector: 'app-tbl-action',
    imports: [SharedImports],
    templateUrl: 'tbl-action.html'
})
export class TblAction extends BaseComponent {
    tblEmit = inject(TBL_CUSTOM_COMP_EMIT); // comes from parent

    @Input() row: IViewRowDotDiemDanh = {};
    @Input() rowIndex: number = 0;
    @Input() data: any;

    actionType = TblActionTypes

    onClick(customType: string) {
        this.tblEmit.emit({
            data: this.row,
            type: customType
        });
    }
}
