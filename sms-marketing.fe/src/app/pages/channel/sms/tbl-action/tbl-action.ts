import { IViewRowChienDich } from '@/models/sms.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, Input, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { Button } from 'primeng/button';

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
    private readonly MENU_OFFSET_X = 5;
    private readonly MENU_WIDTH = 180;

    tblEmit = inject(TBL_CUSTOM_COMP_EMIT);

    @Input() row: IViewRowChienDich = {};
    @Input() rowIndex: number = 0;
    @Input() data: any;
    @ViewChild('menu', { static: false }) menu!: Menu;
    @ViewChild('actionBtn', { static: false }) actionBtn!: Button;

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

    onClick(customType: string): void {
        this.tblEmit.emit({
            data: this.row,
            type: customType
        });
    }

    onMenuClick(event: Event): void {
        event?.stopPropagation();
        event?.preventDefault();

        if (TblAction.currentOpenMenu && TblAction.currentOpenMenu !== this.menu) {
            TblAction.currentOpenMenu.hide();
        }

        this.menu?.toggle(event);
        
        setTimeout(() => {
            this.setMenuPosition();
        }, 0);

        TblAction.currentOpenMenu = this.menu?.visible ? this.menu : null;
    }

    private setMenuPosition(): void {
        if (!this.actionBtn?.el?.nativeElement || !this.menu?.containerViewChild?.nativeElement) return;

        const buttonRect = this.actionBtn.el.nativeElement.getBoundingClientRect();
        const menuElement = this.menu.containerViewChild.nativeElement;
        const leftPosition = buttonRect.left - this.MENU_WIDTH - this.MENU_OFFSET_X;
        
        menuElement.style.position = 'fixed';
        menuElement.style.left = `${leftPosition}px`;
        menuElement.style.top = `${buttonRect.top}px`;
        menuElement.style.zIndex = '9999';
    }

    ngOnDestroy(): void {
        if (TblAction.currentOpenMenu === this.menu) {
            TblAction.currentOpenMenu = null;
        }
    }
}