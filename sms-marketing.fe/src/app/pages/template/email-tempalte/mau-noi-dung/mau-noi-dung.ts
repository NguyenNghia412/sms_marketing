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
import { EmailTempalte } from '../../models/data-email-template.models';
import { TemplateEmailService } from '@/services/template-email.service';

@Component({
    selector: 'app-mau-noi-dung',
    imports: [SharedImports, DataTable],
    templateUrl: './mau-noi-dung.html',
    styleUrl: './mau-noi-dung.scss'
})
export class MauNoiDung extends BaseComponent {
    _templateEmailService = inject(TemplateEmailService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    listEmailTempalte: EmailTempalte[] = [];
    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên mẫu nội dung', field: 'tenMauNoiDung', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: EmailTempalte[] = [];
    query: IFindPagingMauNoiDung = {
        pageNumber: 1,
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
        this.loading = false;
        this._templateEmailService
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
        this.router.navigate(['template/mau-email/create-template-email'], {});
    }

    onOpenUpdate(data: any) {
        this.router.navigate(['template/mau-email/create-template-email'], {
            queryParams: {
                id: encodeURIComponent(JSON.stringify(data.id))
            }
        });
    }

    onCustomEmit(data: { type: string; data: EmailTempalte }) {
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

    onDelete(data: EmailTempalte) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa template?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._templateEmailService.delete(data.id || 0).subscribe(
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
