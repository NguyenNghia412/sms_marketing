import { DataTable } from "@/shared/components/data-table/data-table";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject, OnInit } from "@angular/core";
import { BaseComponent } from '@/shared/components/base/base-component';
import { ReportSmsService } from "@/services/report-sms.service";
import { CampaginStatuses } from "@/shared/constants/channel.constants";
import { FormGroup, FormControl } from "@angular/forms";
import { IColumn } from "@/shared/models/data-table.models";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { IFindPagingChiTietChienDichReport, IViewChiTietChienDichReport } from "@/models/report-sms.models";
import { PaginatorState } from "primeng/paginator";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: 'app-thong-ke-chien-dich-chi-tiet',
    imports: [...SharedImports, DataTable],
    templateUrl: './chi-tiet-report.html',
    styleUrl: './chi-tiet-report.scss'
})
export class ChiTietChienDichReport extends BaseComponent implements OnInit {
    _reportSmsService = inject(ReportSmsService);
    private route = inject(ActivatedRoute);
    
    statusList = CampaginStatuses.List;
    idChienDich: number = 0;
    idDanhBa: number = 0;
    
    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Họ và tên', field: 'hoVaTen', headerContainerStyle: 'min-width: 8rem' },
        { header: 'Mã số người dùng', field: 'maSoNguoiDung', headerContainerStyle: 'min-width: 8rem' },
        { header: 'Số điện thoại', field: 'soDienThoai', headerContainerStyle: 'min-width: 8rem' },
        { header: 'Brand Name', field: 'tenBrandName', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Chi Phí', field: 'gia', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Message', field: 'messageText', headerContainerStyle: 'min-width: 8rem' },
        { header: 'Thời gian gửi', field: 'ngayGui', headerContainerStyle: 'width: 20rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
    ];

    data: IViewChiTietChienDichReport[] = [];
    query: IFindPagingChiTietChienDichReport = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.idChienDich = +params['idChienDich'];
            this.idDanhBa = +params['idDanhBa'];
            
            if (this.idChienDich && this.idDanhBa) {
                this.getData();
            }
        });
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
    
    }

    getData() {
        if (!this.idChienDich || !this.idDanhBa) {
        
            return;
        }

        this.loading = true;
        this._reportSmsService.findPagingChiTietChienDich(
            this.idChienDich, 
            this.idDanhBa,
            { 
                ...this.query, 
                keyword: this.searchForm.get('search')?.value 
            }
        ).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map(item => ({
                        ...item,
                        tenBrandName: item.brandName?.tenBrandName || '',
                        gia: item.log?.price || 0,
                        messageText: item.log?.message || '',
                        ngayGui : item.log?.ngayGui || ''
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