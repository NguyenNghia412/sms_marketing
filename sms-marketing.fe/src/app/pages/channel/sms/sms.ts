import { IFindPagingChienDich, IViewRowChienDich } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CampaginStatuses } from '@/shared/constants/channel.constants';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Create } from './create/create';
import { PaginatorState } from 'primeng/paginator';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { DomSanitizer } from '@angular/platform-browser';
import { Popover } from 'primeng/popover';

@Component({
    selector: 'app-sms',
    imports: [...SharedImports, DataTable, Popover],
    templateUrl: './sms.html',
    styleUrl: './sms.scss'
})
export class Sms extends BaseComponent {
    @ViewChild('filterPanel') filterPanel!: Popover;

    _chienDichService = inject(ChienDichService);
    _sanitizer = inject(DomSanitizer);
    statusList = CampaginStatuses.List;

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdBy: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });

    columns: IColumn[] = [
        //{ header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle:'text-align:center' },
        { header: 'Tên chiến dịch', field: 'tenChienDich', headerContainerStyle: 'min-width: 12rem', cellClass: 'cursor-pointer hover:text-blue-800 hover:underline', clickable: true },
        { header: 'Nội dung', field: 'noiDung', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Người tạo', field: 'users.fullName', headerContainerStyle: 'min-width:9rem' },
        { header: 'Số thuê bao', field: 'soLuongThueBao', headerContainerStyle: 'min-width: 8rem', cellStyle: 'text-align:center' },
        { header: 'Gửi thành công', field: 'soLuongSmsDaGuiThanhCong', headerContainerStyle: 'min-width: 9rem', cellStyle: 'text-align:center' },
        { header: 'Gửi thất bại', field: 'soLuongSmsDaGuiThatBai', headerContainerStyle: 'min-width: 8rem', cellStyle: 'text-align:center' },
        {
            header: 'Trạng thái',
            field: 'trangThaiText',
            headerContainerStyle: 'width: 8rem',
            cellViewType: CellViewTypes.STATUS,
            statusSeverityFunction: (rowData: IViewRowChienDich) => {
                return CampaginStatuses.getSeverityByCode(rowData.trangThai ? CampaginStatuses.DA_GUI : CampaginStatuses.CHUA_GUI);
            }
        },
        //{ header: 'Thời gian tạo', field: 'createdDate', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thời gian tạo', field: 'ngayBatDau', headerContainerStyle: 'width: 8rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss', cellStyle: 'text-align:center' },
        { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowChienDich[] = [];
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
                    this.data = res.data.items.map((item) => ({
                        ...item,
                        soLuongThueBao: item.soLuongThueBao ?? 0,
                        soLuongSmsDaGuiThanhCong: item.soLuongSmsDaGuiThanhCong ?? 0,
                        soLuongSmsDaGuiThatBai: item.soLuongSmsDaGuiThatBai ?? 0,
                        trangThaiText: item.trangThai
                            ? 'Đã gửi'
                            : 'Nháp'
                    }));
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

    onOpenUpdate(data: IViewRowChienDich) {
        const ref = this._dialogService.open(Create, { header: 'Tạo chiến dịch', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onDelete(data: IViewRowChienDich) {
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

    onDuplicate(data: IViewRowChienDich) {
        this._chienDichService.duplicate(data.id || 0).subscribe(
            (res) => {
                if (this.isResponseSucceed(res, true, 'Đã nhân bản chiến dịch')) {
                    this.getData();
                }
            },
            (err) => {
                this.messageError(err?.message);
            }
        );
    }

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    onCustomEmit(data: { type: string; data: IViewRowChienDich; field?: string }) {
        if (data.type === TblActionTypes.detail) {
            this.navigateToDetail(data.data);
        } else if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.duplicate) {
            this.onDuplicate(data.data);
        } else if (data.type === 'cellClick' && data.field === 'tenChienDich') {
            this.navigateToDetail(data.data);
        }
    }

    navigateToDetail(chienDich: IViewRowChienDich) {
        if (chienDich?.id) {
            this.router.navigate(['/channel/gui-sms'], {
                queryParams: {
                    idChienDich: chienDich.id
                }
            });
        }
    }
}
