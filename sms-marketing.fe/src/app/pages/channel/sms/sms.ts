import { IFindPagingChienDich, IViewChienDich } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CampaginStatuses } from '@/shared/constants/channel.constants';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Create } from './create/create';
import { PaginatorState } from 'primeng/paginator';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';

@Component({
    selector: 'app-sms',
    imports: [...SharedImports, DataTable],
    templateUrl: './sms.html',
    styleUrl: './sms.scss'
})
export class Sms extends BaseComponent {
    _chienDichService = inject(ChienDichService);

    statusList = CampaginStatuses.List;

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên chiến dịch', field: 'tenChienDich', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Nội dung', field: 'noiDung', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Thời gian tạo', field: 'createdDate', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thời gian gửi', field: 'ngayBatDau', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewChienDich[] = [];
    query: IFindPagingChienDich = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    getData() {
        this.loading = true;
        this._chienDichService.findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value }).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map(item => {
                        if (item.mauNoiDungs && item.mauNoiDungs.length > 0) {
                            item.noiDung = item.mauNoiDungs[0].noiDung;
                        }
                        return item;
                    });
                    this.totalRecords = res.data.totalItems;
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    onOpenCreate() {
        const ref = this._dialogService.open(Create, { header: 'Tạo chiến dịch', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onOpenUpdate(data: IViewChienDich) {
        const ref = this._dialogService.open(Create, { header: 'Tạo chiến dịch', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onDelete(data: IViewChienDich) {
            this.confirmDelete(
                {
                    header: 'Bạn chắc chắn muốn xóa chiến dịch sms?',
                    message: 'Không thể khôi phục sau khi xóa'
                },
                () => {
                    this._chienDichService.delete(data.id || 0).subscribe(
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

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    onCustomEmit(data: { type: string; data: IViewChienDich }) {
        if (data.type === TblActionTypes.detail) {
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
}
