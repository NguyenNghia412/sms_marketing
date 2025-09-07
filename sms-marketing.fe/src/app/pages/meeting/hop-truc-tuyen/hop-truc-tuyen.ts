import { DataTable } from '@/shared/components/data-table/data-table';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Create } from './create/create';
import { HopTrucTuyenService } from '@/services/hop-truc-tuyen.service';
import { IFindPagingHopTrucTuyen, IViewRowHopTrucTuyen } from '@/models/hopTrucTuyen.models';
import { BaseComponent } from '@/shared/components/base/base-component';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn } from '@/shared/models/data-table.models';

@Component({
    selector: 'app-hop-truc-tuyen',
    imports: [SharedImports, DataTable],
    templateUrl: './hop-truc-tuyen.html',
    styleUrl: './hop-truc-tuyen.scss'
})
export class HopTrucTuyen extends BaseComponent {
    _hopTrucTuyenService = inject(HopTrucTuyenService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX },
        { header: 'Tên', field: 'tenCuocHop', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Link', field: 'linkCuocHop' },
        { header: 'Thời gian bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thời gian kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
    ];

    query: IFindPagingHopTrucTuyen = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE
    };
    data: IViewRowHopTrucTuyen[] = [];

    override ngOnInit(): void {
        this.getData();
    }

    getData() {
        this.loading = true;
        this._hopTrucTuyenService
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

    onOpenCreate() {
        const ref = this._dialogService.open(Create, { header: 'Tạo mới phòng học', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onSearch() {
        this.getData();
    }
}
