import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
    selector: 'app-sms-main',
    imports: [SharedImports, DataTable],
    templateUrl: './sms-main.html',
    styleUrl: './sms-main.scss'
})
export class SmsMain {

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    listSMSTempalte: SMSTempalte[] = []

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tên mẫu nội dung', field: 'tenMauNoiDung', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Mẫu nội dung', field: 'mauNoiDung', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];
}
