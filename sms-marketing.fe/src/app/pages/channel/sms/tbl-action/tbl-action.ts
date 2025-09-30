import { IViewRowChienDich } from '@/models/sms.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, Input, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';

export const TblActionTypes = {
    duplicate: 'duplicate',
    detail: 'detail',
    delete: 'delete',
}

@Component({
    selector: 'app-tbl-action',
    imports: [SharedImports, Menu],
    templateUrl: 'tbl-action.html'
})
export class TblAction extends BaseComponent {
    private static currentOpenMenu: Menu | null = null;

    tblEmit = inject(TBL_CUSTOM_COMP_EMIT);

    @Input() row: IViewRowChienDich = {};
    @Input() rowIndex: number = 0;
    @Input() data: any;
    @ViewChild('menu', { static: false }) menu!: Menu;

    actionType = TblActionTypes;
    menuItems: MenuItem[] = [];

    override ngOnInit(): void {
        this.menuItems = [
            {
                label: 'Nhân bản',
                icon: 'pi pi-clone',
                command: () => this.onClick(TblActionTypes.duplicate)
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

    onMenuClick(event: any) {
        event?.stopPropagation();
        event?.preventDefault();

        if (TblAction.currentOpenMenu && TblAction.currentOpenMenu !== this.menu) {
            TblAction.currentOpenMenu.hide();
        }

        this.menu?.toggle(event);
        TblAction.currentOpenMenu = this.menu?.visible ? this.menu : null;
    }

    ngOnDestroy(): void {
        if (TblAction.currentOpenMenu === this.menu) {
            TblAction.currentOpenMenu = null;
        }
    }
}