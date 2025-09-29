import { IViewRowChienDich } from '@/models/sms.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, Input } from '@angular/core';

export const TblActionTypes = {
    delete: 'delete',
}

@Component({
    selector: 'app-tbl-action',
    imports: [SharedImports],
    templateUrl: 'tbl-action.html'
})
export class TblAction extends BaseComponent {
    tblEmit = inject(TBL_CUSTOM_COMP_EMIT); // comes from parent

    @Input() row: IViewRowChienDich = {} ;
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
