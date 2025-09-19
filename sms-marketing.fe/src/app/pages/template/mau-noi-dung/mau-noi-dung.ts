import { IFindPagingMauNoiDung, IViewRowMauNoiDung } from '@/models/template.models';
import { TemplateService } from '@/services/template.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PaginatorState } from 'primeng/paginator';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';

@Component({
  selector: 'app-mau-noi-dung',
  imports: [SharedImports, DataTable],
  templateUrl: './mau-noi-dung.html',
  styleUrl: './mau-noi-dung.scss'
})
export class MauNoiDung extends BaseComponent {
_templateService = inject(TemplateService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Nội dung', field: 'noiDung', headerContainerStyle: 'min-width: 10rem', },
        { header: 'Thời gian tạo', field: 'createdDate',headerContainerStyle: 'width: 10rem' , cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowMauNoiDung[] = [];
    query: IFindPagingMauNoiDung = {
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
        this._templateService
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
    //     const ref = this._dialogService.open(Create, { header: 'Tạo danh bạ mới', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false });
    //     ref.onClose.subscribe((result) => {
    //         if (result) {
    //             this.getData();
    //         }
    //     });
    }

    onOpenUpdate(data: IViewRowMauNoiDung) {
    //     const ref = this._dialogService.open(Create, { header: 'Cập nhật danh bạ', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false, data: data });
    //     ref.onClose.subscribe((result) => {
    //         if (result) {
    //             this.getData();
    //         }
    //     });
    }

    onCustomEmit(data: { type: string; data: IViewRowMauNoiDung }) {
        if (data.type === TblActionTypes.use) {
            // const uri = '/danh-ba/chi-tiet';
            // this.router.navigate([uri], {
            //     queryParams: {
            //         id: data.data.id
            //     }
            // });
        } else if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        }
    }

    onDelete(data: IViewRowMauNoiDung) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa template?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._templateService.delete(data.idMauNoiDung || 0).subscribe(
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
