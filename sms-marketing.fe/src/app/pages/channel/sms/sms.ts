import { IFindPagingChienDich, IViewChienDich } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CampaginStatuses } from '@/shared/constants/channel.constants';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
    selector: 'app-sms',
    imports: [...SharedImports, DataTable],
    templateUrl: './sms.html',
    styleUrl: './sms.scss'
})
export class Sms implements OnInit {
    
    _chienDichService = inject(ChienDichService)

    statusList = CampaginStatuses.List;

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX },
        { header: 'Id', field: 'id', },
        { header: 'Tên chiến dịch', field: 'tenChienDich', headerContainerStyle: 'min-width: 12rem' },
        { header: 'Thời gian tạo', field: 'createdDate', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Thời gian gửi', field: 'ngayBatDau', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        { header: 'Nội dung', field: 'noiDung', headerContainerStyle: 'min-width: 12rem', },
    ];

    data: IViewChienDich[] = [];
    query: IFindPagingChienDich = {
        pageNumber: 1,
        pageSize: 5
    }

    ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    getData() {
        this._chienDichService.findPaging({...this.query, keyword: this.searchForm.get('search')?.value}).subscribe((res: any) => {
            console.log(res)
            if (res && res.status === 1 && res.data) {
                this.data = res.data.items
            }
        }, err => {
            console.error(err)
        })
    }
}
