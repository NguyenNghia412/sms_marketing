import { IFindPagingRole, IViewRowRole } from '@/models/role.models';
import { PermissionService } from '@/services/permission.service';
import { RoleService } from '@/services/role.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PaginatorState } from 'primeng/paginator';
import { Create } from './create/create';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';

@Component({
    selector: 'app-role',
    imports: [SharedImports, DataTable],
    templateUrl: './role.html',
    styleUrl: './role.scss'
})
export class Role extends BaseComponent {
    _roleService = inject(RoleService);
    _permissionService = inject(PermissionService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên', field: 'name', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowRole[] = [];
    query: IFindPagingRole = {
        pageNumber: this.START_PAGE_NUMBER,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    getData() {
        this.loading = true;
        this._roleService
            .findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value })
            .subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, false)) {
                        this.data = res.data.items;
                        this.totalRecords = res.data.totalItems;
                    }
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    onOpenCreate() {
        const ref = this._dialogService.open(Create, { header: 'Tạo vai trò', closable: true, modal: true, styleClass: 'w-[1000px]', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onOpenUpdate(data: IViewRowRole) {
        const ref = this._dialogService.open(Create, { header: 'Cập nhật vai trò', closable: true, modal: true, styleClass: 'w-[1000px]', focusOnShow: false, data });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onCustomEmit(data: { type: string; data: IViewRowRole; field?: string }) {
        if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        } else if (data.type === TblActionTypes.delete) {
        }
    }
}
