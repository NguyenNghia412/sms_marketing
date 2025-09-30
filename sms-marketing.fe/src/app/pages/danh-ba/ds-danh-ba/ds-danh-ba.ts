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
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { PaginatorState } from 'primeng/paginator';

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
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên danh bạ', field: 'tenDanhBa', headerContainerStyle: 'min-width: 10rem', },
        { header: 'Mô tả', field: 'mota', headerContainerStyle: 'min-width: 20rem' },
        { header: 'Số người nhận', field: 'soLuongNguoiNhan', headerContainerStyle: 'min-width: 5rem' },
        { header: 'Thời gian tạo', field: 'createdDate',headerContainerStyle: 'width: 15rem' , cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 5rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowDanhBa[] = [];
    query: IFindPagingDanhBa = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) +1;
        this.getData();
    }

    getData() {
        this.loading = true;
        this._danhBaService
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
        const ref = this._dialogService.open(Create, { header: 'Tạo danh bạ mới', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onOpenUpdate(data: IViewRowDanhBa) {
        const ref = this._dialogService.open(Create, { header: 'Cập nhật danh bạ', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false, data: data });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onCustomEmit(data: { type: string; data: IViewRowDanhBa }) {
        if (data.type === TblActionTypes.detail) {
            const uri = '/danh-ba/chi-tiet';
            this.router.navigate([uri], {
                queryParams: {
                    id: data.data.id
                }
            });
        } else if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        }
    }

    onDelete(data: IViewRowDanhBa) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa danh bạ?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._danhBaService.delete(data.id || 0).subscribe(
                    (res) => {
                        if (this.isResponseSucceed(res, true, 'Đã xóa')) {
                            this.getData();
                        }
                    },
                    (err) => {
                        this.messageError(err?.message);
                    }
                );
            }
        );
    }
}