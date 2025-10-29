import { IFindPagingDotDiemDanh, IViewRowDotDiemDanh } from '@/models/dot-diem-danh.models';
import { DotDiemDanhService } from '@/services/dot-diem-danh.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Create } from './create/create';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { concatMap } from 'rxjs';

@Component({
    selector: 'app-dot-diem-danh',
    imports: [SharedImports, DataTable],
    templateUrl: './dot-diem-danh.html',
    styleUrl: './dot-diem-danh.scss'
})
export class DotDiemDanh extends BaseComponent {
    _dotDiemDanhService = inject(DotDiemDanhService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });
    idCuocHop: number | null | undefined;

    query: IFindPagingDotDiemDanh = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
        idCuocHop: 0
    };
    data: IViewRowDotDiemDanh[] = [];

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX },
        { header: 'Tên', field: 'tenDotDiemDanh', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Môn học', field: 'tenMonHoc', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Thời gian bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thời gian kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thao tác', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    override ngOnInit(): void {
        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.idCuocHop = Number(params.get('idCuocHop'));
            this.query.idCuocHop = this.idCuocHop;
            this.getData();
        });
    }

    getData() {
        this.loading = true;
        this._dotDiemDanhService
            .findPaging(this.query)
            .subscribe((res) => {
                if (this.isResponseSucceed(res)) {
                    this.data = res.data.items;
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    onSearch() {
        this.getData();
    }

    onOpenCreate() {
        const ref = this._dialogService.open(Create, {
            header: 'Tạo mới đợt điểm danh',
            closable: true,
            modal: true,
            contentStyle: { width: '500px' },
            focusOnShow: false,
            data: {
                idCuocHop: this.idCuocHop
            }
        });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onCustomEmit(data: { type: string; data: IViewRowDotDiemDanh }) {
        if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.downloadQr) {
            this.onDownloadQr(data.data);
        }
    }

    onDelete(data: IViewRowDotDiemDanh) {
        this.confirmDelete({ header: 'Bạn chắc chắn muốn xóa đợt điểm danh?', message: 'Không thể khôi phục sau khi xóa' }, () => {
            this._dotDiemDanhService.delete(this.idCuocHop || 0, data.id || 0).subscribe(
                (res) => {
                    if (this.isResponseSucceed(res, true, 'Đã xóa')) {
                        this.getData();
                    }
                },
                (err) => {
                    this.messageError(err?.message);
                }
            );
        });
    }

    onDownloadQr(data: IViewRowDotDiemDanh) {
      this._dotDiemDanhService.downloadQrDiemDanh(data.id ?? 0);
        // this._dotDiemDanhService
        //     .generateQrDiemDanh(data.id ?? 0)
        //     .pipe(
        //         concatMap((res) => {
        //             return this._dotDiemDanhService.downloadQrDiemDanh(data.id ?? 0);
        //         })
        //     )
        //     .subscribe(
        //         (res) => {
        //             console.log(res);
        //         },
        //         (err) => {
        //             this.messageError(err?.message);
        //         }
        //     );
    }
}
