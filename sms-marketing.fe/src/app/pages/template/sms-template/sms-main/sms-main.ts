import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { SMSTempalte } from '../../models/sms-template.models';
import { IFindPagingMauNoiDung } from '@/models/template.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { PaginatorState } from 'primeng/paginator';
import { TemplateService } from '@/services/template.service';
import { CreateTemplateSms } from '../create-template-sms/create-template-sms';

@Component({
    selector: 'app-sms-main',
    imports: [SharedImports, DataTable],
    templateUrl: './sms-main.html',
    styleUrl: './sms-main.scss'
})
export class SmsMain extends BaseComponent {
    _templateService = inject(TemplateService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    data: SMSTempalte[] = [];

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên mẫu nội dung', field: 'tenMauNoiDung', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Mẫu nội dung', field: 'mauNoiDung', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

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
        const ref = this._dialogService.open(CreateTemplateSms, { header: 'Tạo template SMS', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onOpenUpdate(data: SMSTempalte) {
        const ref = this._dialogService.open(CreateTemplateSms, { header: 'Cập nhật template SMS', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false, data });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onCustomEmit(data: { type: string; data: SMSTempalte }) {
        if (data.type === TblActionTypes.use) {
        } else if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        }
    }

    onDelete(data: any) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa template?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._templateService.delete(data.id || 0).subscribe(
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
