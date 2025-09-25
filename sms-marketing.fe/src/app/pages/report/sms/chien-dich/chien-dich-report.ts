import { IFindPagingChienDichReport, IViewChienDichReport } from "@/models/report-sms.models";
import { ReportSmsService } from "@/services/report-sms.service";
import { DataTable } from "@/shared/components/data-table/data-table";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { BaseComponent } from '@/shared/components/base/base-component';
import { PaginatorState } from "primeng/paginator";
import { CampaginStatuses } from "@/shared/constants/channel.constants";
import { Router } from "@angular/router";

@Component({
    selector: 'app-thong-ke-chien-dich',
    imports: [...SharedImports, DataTable],
    templateUrl: './chien-dich-report.html',
    styleUrl: './chien-dich-report.scss'

})
export class ChienDichReport extends BaseComponent{
    _reportSmsService = inject(ReportSmsService);
     override router = inject(Router);
    statusList = CampaginStatuses.List;
    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });
    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { 
           header: 'Tên chiến dịch', 
            field: 'tenChienDich', 
            headerContainerStyle: 'min-width: 6rem',
            cellClass: 'cursor-pointer text-blue-600 hover:text-blue-800 hover:underline'
        },
        
        { header: 'Tên danh bạ', field: 'danhBa.tenDanhBa', headerContainerStyle: 'min-width: 6rem', },
        { header: 'Gửi thành công', field:'smsSentSuccess',headerContainerStyle: 'min-width: 6rem'},
        { header: 'Gửi thất bại', field:'smsSentFailed',headerContainerStyle: 'min-width: 6rem'},
        { header: 'Tổng chi phí', field:'tongChiPhi',headerContainerStyle: 'min-width: 6rem'},
        { header: 'Trạng Thái', field: 'trangThaiText', headerContainerStyle: 'width: 10rem' },
        { header: 'Thời gian gửi', field: 'ngayGui', headerContainerStyle: 'width: 20rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },



    ]
     data: IViewChienDichReport[] = [];
        query: IFindPagingChienDichReport = {
            pageNumber: 1,
            pageSize: this.MAX_PAGE_SIZE
        };
    
        override ngOnInit(): void {
            this.getData();
        }
    
         onSearch() {
        this.query.pageNumber = 1; 
        this.getData();
    }

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    
    onCustomEmit($event: any) {
        if ($event.type === 'cellClick' && $event.field === 'tenChienDich') {
            this.navigateToDetail($event.rowData);
        }
    }

    navigateToDetail(chienDich: IViewChienDichReport) {
    //console.log('Navigating with:', chienDich.idChienDich, chienDich.danhBa?.idDanhBa);
    if (chienDich.idChienDich && chienDich.danhBa?.idDanhBa) {
        this.router.navigate(['/report/chi-tiet-chien-dich-report'], {
            queryParams: {
                idChienDich: chienDich.idChienDich,
                idDanhBa: chienDich.danhBa?.idDanhBa
                }
            });
        }
    }

    getData() {
        this.loading = true;
        this._reportSmsService.findPaging({ 
            ...this.query, 
            keyword: this.searchForm.get('search')?.value 
        }).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map(item => ({
                        ...item,
                        trangThaiText: item.trangThai ? 'Thành công' : 'Thất bại'
                    }));
                    this.totalRecords = res.data.totalItems;
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}