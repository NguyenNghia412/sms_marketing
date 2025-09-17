import { IFindPagingDanhBa, IViewRowDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Create } from './create/create';
import { TblAction } from './tbl-action/tbl-action';

@Component({
    selector: 'app-ds-danh-ba',
    imports: [...SharedImports, DataTable],
    templateUrl: './ds-danh-ba.html',
    styleUrl: './ds-danh-ba.scss'
})
export class DsDanhBa extends BaseComponent {
    _danhBaService = inject(DanhBaService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX },
        { header: 'Tên danh bạ', field: 'tenDanhBa', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Thời gian tạo', field: 'createdDate', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowDanhBa[] = [];
    query: IFindPagingDanhBa = {
        pageNumber: 1,
        pageSize: 10
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    getData() {
        this.loading = true;
        this._danhBaService
            .findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value })
            .subscribe({
                next: (res: any) => {
                    if (this.isResponseSucceed(res, false)) {
                        this.data = res.data.items;
                    }
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    onOpenCreate() {
        const ref = this._dialogService.open(Create, { header: 'Tạo danh bạ mới', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }
}
