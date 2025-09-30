import { IViewRowDanhBa } from '@/models/danh-ba.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, Input, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';

export const TblActionTypes = {
    detail: 'detail',
    update: 'update',
    delete: 'delete'
}

@Component({
    selector: 'app-tbl-action',
    imports: [SharedImports, Menu],
    templateUrl: 'tbl-action.html'
})
export class TblAction extends BaseComponent {
    tblEmit = inject(TBL_CUSTOM_COMP_EMIT);
    @Input() row: IViewRowDanhBa = {};
    @Input() rowIndex: number = 0;
    @Input() data: any;
    @ViewChild('menu') menu!: Menu;
    
    actionType = TblActionTypes;
    menuItems: MenuItem[] = [];
    
    override ngOnInit(): void {
        this.menuItems = [
            {
                label: 'Cập nhật',
                icon: 'pi pi-pencil',
                command: () => this.onClick(TblActionTypes.update)
            },
            {
                label: 'Xóa',
                icon: 'pi pi-trash',
                command: () => this.onClick(TblActionTypes.delete)
            }
        ];
    }
    
    onClick(customType: string) {
        this.tblEmit.emit({
            data: this.row,
            type: customType
        });
    }
    
    onMenuClick(event: Event) {
        this.menu.toggle(event);
    }
}