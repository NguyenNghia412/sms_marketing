import { IFindPagingChienDichReport, IViewChienDichReport } from "@/models/report-sms.models";
import { ReportSmsService } from "@/services/report-sms.service";
import { DataTable } from "@/shared/components/data-table/data-table";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject, ViewChild } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { BaseComponent } from '@/shared/components/base/base-component';
import { PaginatorState } from "primeng/paginator";
import { CampaginStatuses } from "@/shared/constants/channel.constants";
import { Router } from "@angular/router";
import { Popover } from "primeng/popover";
import { ConfirmDialog } from "primeng/confirmdialog";
import { ConfirmationService } from "primeng/api";
import { DatePicker } from "primeng/datepicker";

@Component({
    selector: 'app-thong-ke-chien-dich',
    imports: [...SharedImports, DataTable, Popover, ConfirmDialog, DatePicker],
    templateUrl: './chien-dich-report.html',
    styleUrl: './chien-dich-report.scss',
    providers: [ConfirmationService]
})
export class ChienDichReport extends BaseComponent {
    @ViewChild('exportPopover') exportPopover!: Popover;
    @ViewChild('monthPopover') monthPopover!: Popover;
    
    _reportSmsService = inject(ReportSmsService);
    override router = inject(Router);
    confirmationService = inject(ConfirmationService);
    
    statusList = CampaginStatuses.List;
    isExportMode = false;
    selectedChienDichs: number[] = [];
    selectedMonthYear: Date | null = null;
    
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
            cellClass: 'cursor-pointer text-blue-600 hover:text-blue-800 hover:underline',
            clickable: true 
        },
        { header: 'Tên danh bạ', field: 'danhBa.tenDanhBa', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Nội dung', field: 'noiDung', headerContainerStyle: 'min-width: 20rem' },
        { header: 'Tổng số thuê bao', field: 'tongSoSms', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Gửi thành công', field: 'smsSentSuccess', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Gửi thất bại', field: 'smsSentFailed', headerContainerStyle: 'min-width: 6rem' },
        { header: 'Tổng chi phí', field: 'tongChiPhi', headerContainerStyle: 'min-width: 6rem' },
        //{ header: 'Trạng Thái', field: 'trangThaiText', headerContainerStyle: 'width: 10rem' },
        { header: 'Thời gian gửi', field: 'ngayGui', headerContainerStyle: 'width: 20rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' }
    ];
    
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
        if ($event.type === 'cellClick') {
            if ($event.field === 'checked' && this.isExportMode) {
                this.onCheckboxChange($event.data);
            } else if ($event.field === 'tenChienDich' && !this.isExportMode) {
                this.navigateToDetail($event.data);
            }
        }
    }

    navigateToDetail(chienDich: IViewChienDichReport) {
        if (chienDich.idChienDich && chienDich.danhBa?.idDanhBa) {
            this.router.navigate(['/report/chi-tiet-chien-dich-report'], {
                queryParams: {
                    idChienDich: chienDich.idChienDich,
                    idDanhBa: chienDich.danhBa?.idDanhBa
                }
            });
        }
    }

    onExportTheoThang() {

    }

    onMonthYearSelect() {
        this.monthPopover.hide();
        this.exportPopover.hide();
        if (this.selectedMonthYear) {
            this.onConfirmExportThang();
        }
    }

    onConfirmExportThang() {
        if (!this.selectedMonthYear) {
            return;
        }

        const thang = this.selectedMonthYear.getMonth() + 1;
        const nam = this.selectedMonthYear.getFullYear();

        this.confirmationService.confirm({
            message: `Bạn có chắc chắn muốn download thống kê tháng ${thang}/${nam}?`,
            header: 'Xác nhận',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.downloadExportThang();
            }
        });
    }

    downloadExportThang() {
        if (!this.selectedMonthYear) {
            return;
        }

        this.loading = true;
        const thang = this.selectedMonthYear.getMonth() + 1;
        const nam = this.selectedMonthYear.getFullYear();

        this._reportSmsService.downloadThongKeTheoThang({ 
            thang: thang, 
            nam: nam 
        }).subscribe({
            complete: () => {
                this.loading = false;
                this.selectedMonthYear = null;
            }
        });
    }

    onExportTheoChienDich() {
        this.exportPopover.hide(); 
        this.isExportMode = !this.isExportMode;
        this.selectedChienDichs = [];
        if (this.isExportMode) {
            this.addCheckboxColumn();
        } else {
            this.removeCheckboxColumn();
        }
    }

    addCheckboxColumn() {
        if (!this.columns.some(c => c.field === 'checked')) {
            this.columns.unshift({
                header: 'Chọn',
                field: 'checked',
                headerContainerStyle: 'width: 5rem',
                cellViewType: CellViewTypes.CHECKBOX
            });
        }
    }

    removeCheckboxColumn() {
        this.columns = this.columns.filter(c => c.field !== 'checked');
        this.data = this.data.map(item => {
            const { checked, ...rest } = item as any;
            return rest;
        });
    }

    onCheckboxChange(chienDich: any) {
        if (chienDich && chienDich.idChienDich) {
            const index = this.selectedChienDichs.indexOf(chienDich.idChienDich);
            if (index > -1) {
                this.selectedChienDichs.splice(index, 1);
                chienDich.checked = false;
            } else {
                this.selectedChienDichs.push(chienDich.idChienDich);
                chienDich.checked = true;
            }
            this.data = [...this.data];
        }
    }

    onConfirmExport() {
        if (this.selectedChienDichs.length === 0) {
            return;
        }

        this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn download thống kê của các chiến dịch trên?',
            header: 'Xác nhận',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.downloadExport();
            }
        });
    }

    downloadExport() {
        this.loading = true;
        this._reportSmsService.downloadThongKeTheoChienDich({ idChienDichs: this.selectedChienDichs }).subscribe({
            complete: () => {
                this.selectedChienDichs = [];
                this.isExportMode = false;
                this.removeCheckboxColumn();
                this.loading = false;
            }
        });
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
                        trangThaiText: item.trangThai ? 'Thành công' : 'Thất bại',
                        checked: false
                    }));
                    this.totalRecords = res.data.totalItems;
                    
                    const hasTongChiPhi = res.data.items.some(item => item.tongChiPhi != null);
                    if (!hasTongChiPhi) {
                        this.columns = this.columns.filter(c => c.field !== 'tongChiPhi');
                    } else if (!this.columns.some(c => c.field === 'tongChiPhi')) {
                        const trangThaiIndex = this.columns.findIndex(c => c.field === 'trangThaiText');
                        this.columns.splice(trangThaiIndex, 0, { header: 'Tổng chi phí', field: 'tongChiPhi', headerContainerStyle: 'min-width: 6rem' });
                    }
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}