import { IViewRowHopTrucTuyen } from '@/models/hopTrucTuyen.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { ICustomEmit } from '@/shared/models/data-table.models';
import { Component, inject, Input, output } from '@angular/core';

@Component({
    selector: 'app-tbl-action',
    imports: [SharedImports],
    templateUrl: 'tbl-action.html'
})
export class TblAction extends BaseComponent {
    @Input() row: IViewRowHopTrucTuyen = {};
    @Input() rowIndex: number = 0;
    @Input() data: any;

    tblEmit = inject(TBL_CUSTOM_COMP_EMIT); // comes from parent

    onClick(customType: string) {
        this.tblEmit.emit({
            data: this.row,
            type: customType
        });
    }
}
