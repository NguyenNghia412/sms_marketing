import { DataTable } from "@/shared/components/data-table/data-table";
import { SharedImports } from "@/shared/import.shared";
import { Component, inject, OnInit } from "@angular/core";
import { BaseComponent } from '@/shared/components/base/base-component';
import { ReportSmsService } from "@/services/report-sms.service";
import { DanhBaService } from "@/services/danh-ba.service";
import { CampaginStatuses } from "@/shared/constants/channel.constants";
import { FormGroup, FormControl } from "@angular/forms";
import { IColumn } from "@/shared/models/data-table.models";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { IFindPagingChiTietChienDichReport, IViewChiTietChienDichReport } from "@/models/report-sms.models";
import { IListTinNhanError } from "@/models/danh-ba.models";
import { PaginatorState } from "primeng/paginator";
import { ActivatedRoute } from "@angular/router";
import { Breadcrumb } from "primeng/breadcrumb";
import { MenuItem } from "primeng/api";
import { Popover } from 'primeng/popover';
import { ReportStatus } from "@/shared/constants/report.constants";
import { MenuModule } from "primeng/menu";

@Component({
    selector: 'app-thong-ke-chien-dich-chi-tiet',
    imports: [...SharedImports, DataTable, Breadcrumb, Popover, MenuModule],
    templateUrl: './chi-tiet-report.html',
    styleUrl: './chi-tiet-report.scss',
})
export class ChiTietChienDichReport extends BaseComponent implements OnInit {
    
    _reportSmsService = inject(ReportSmsService);
    _danhBaService = inject(DanhBaService);
    private route = inject(ActivatedRoute);
    items: MenuItem[] = [{ label: 'Thống kê', routerLink: '/report/chien-dich-report'  }, { label: 'Thống kê chi tiết ' }];
    home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };
    statusList = ReportStatus.List;
    idChienDich: number = 0;

    get filterButtonLabel(): string {
        const trangThai = this.searchForm.get('trangThai')?.value;
        const parts: string[] = [];
        if (trangThai) {
            const found = this.statusList.find(s => s.code === trangThai);
            if (found) parts.push(`Trạng thái: ${found.name}`);
        }
        return parts.length > 0 ? parts.join(' | ') : 'Tìm kiếm theo';
    }

    get hasActiveFilters(): boolean {
        return !!this.searchForm.get('trangThai')?.value;
    }
    idDanhBa: number = 0;
    isSelectMode = false;
    isAllSelected = false;
    selectedItems: IListTinNhanError[] = [];
    createDanhBaDialogVisible = false;
    tenDanhBaMoi = '';
    actionMenuItems: MenuItem[] = [
        {
            label: 'Tạo nhanh danh bạ mới',
            icon: 'pi pi-plus',
            command: () => this.onToggleSelectMode()
        }
    ];
    
    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        createdTime: new FormControl(''),
        sendTime: new FormControl(''),
        status: new FormControl(''),
        trangThai: new FormControl(''),
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
        
        { header: 'Họ và tên', field: 'hoVaTen', headerContainerStyle: 'min-width: 8rem' },
        //{ header: 'Mã số người dùng', field: 'maSoNguoiDung', headerContainerStyle: 'min-width: 8rem' },
        { header: 'Số điện thoại', field: 'log.soDienThoai', headerContainerStyle: 'min-width: 8rem', cellStyle: 'text-align:center' },
        { header: 'Brand Name', field: 'tenBrandName', headerContainerStyle: 'min-width: 6rem', cellStyle: 'text-align:center' },
        { header: 'Nội dung chi tiết', field: 'log.noiDungChiTiet', headerContainerStyle: 'min-width: 20rem' },
        { header: 'Số Tin Nhắn', field: 'log.soLuongTinNhan', headerContainerStyle: 'min-width: 8rem', cellStyle: 'text-align:center' },
        { header: 'Chi Phí', field: 'gia', headerContainerStyle: 'min-width: 6rem', cellStyle: 'text-align:center' },
        { header: 'Message', field: 'messageText', headerContainerStyle: 'min-width: 8rem', cellStyle: 'text-align:center' },
        { header: 'Người gửi', field: 'users.fullName', headerContainerStyle: 'min-width: 12rem', cellStyle: 'text-align:center' },
        { header: 'Thời gian tạo', field: 'ngayGui', headerContainerStyle: 'width: 20rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss', cellStyle: 'text-align:center' },
    ];

    data: IViewChiTietChienDichReport[] = [];
    query: IFindPagingChiTietChienDichReport = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.idChienDich = +params['idChienDich'];
            this.idDanhBa = params['idDanhBa'] ? +params['idDanhBa'] : 0;
            
            if (this.idChienDich ) {
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
        if ($event.type === 'cellClick' && $event.field === 'checked' && this.isSelectMode) {
            this.onCheckboxChange($event.data);
        }
        if ($event.type === 'headerCheckbox' && $event.field === 'checked' && this.isSelectMode) {
            this.onSelectAll();
        }
    }

    onToggleSelectMode(): void {
        this.isSelectMode = !this.isSelectMode;
        this.isAllSelected = false;
        this.selectedItems = [];
        if (this.isSelectMode) {
            this.addCheckboxColumn();
        } else {
            this.removeCheckboxColumn();
        }
    }

    addCheckboxColumn(): void {
        if (!this.columns.some(c => c.field === 'checked')) {
            this.columns.unshift({
                header: 'Chọn',
                field: 'checked',
                headerContainerStyle: 'width: 5rem',
                cellViewType: CellViewTypes.CHECKBOX
            });
        }
    }

    removeCheckboxColumn(): void {
        this.columns = this.columns.filter(c => c.field !== 'checked');
        this.data = this.data.map(item => {
            const { checked, ...rest } = item as any;
            return rest;
        });
    }

    onCheckboxChange(row: any): void {
        const item: IListTinNhanError = { idDanhBa: row.idDanhBa, idDanhBaSms: row.idDanhBaSms };
        const index = this.selectedItems.findIndex(i => i.idDanhBa === item.idDanhBa && i.idDanhBaSms === item.idDanhBaSms);
        if (index > -1) {
            this.selectedItems.splice(index, 1);
            row.checked = false;
            this.isAllSelected = false;
        } else {
            this.selectedItems.push(item);
            row.checked = true;
        }
        this.data = [...this.data];
    }

    onSelectAll(): void {
        if (this.isAllSelected) {
            this.isAllSelected = false;
            this.selectedItems = [];
            this.data = this.data.map(item => ({ ...item, checked: false } as any));
            return;
        }

        this.loading = true;
        this._reportSmsService.findPagingChiTietChienDich(
            this.idChienDich,
            this.idDanhBa ?? 0,
            {
                pageNumber: 1,
                pageSize: this.totalRecords,
                keyword: this.searchForm.get('search')?.value,
                trangThai: this.searchForm.get('trangThai')?.value || ''
            }
        ).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.selectedItems = res.data.items.map(item => ({
                        idDanhBa: item.idDanhBa!,
                        idDanhBaSms: item.idDanhBaSms!
                    }));
                    this.isAllSelected = true;
                    this.data = this.data.map(item => ({ ...item, checked: true } as any));
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    showCreateDanhBaDialog(): void {
        if (this.selectedItems.length === 0) return;
        this.tenDanhBaMoi = '';
        this.createDanhBaDialogVisible = true;
    }

    createDanhBa(): void {
        if (!this.tenDanhBaMoi.trim()) return;
        this.loading = true;
        this.createDanhBaDialogVisible = false;
        this._danhBaService.createDanhBaThueBaoLoiGuiTinNhan({
            idChienDich: this.idChienDich,
            tenDanhBa: this.tenDanhBaMoi.trim(),
            items: this.selectedItems
        }).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Tạo danh bạ thành công')) {
                    this.selectedItems = [];
                    this.isSelectMode = false;
                    this.removeCheckboxColumn();
                    this.getData();
                }
            },
            error: (err) => {
                this.messageError(err?.message || 'Có lỗi xảy ra');
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    getData() {
        if (!this.idChienDich ) {
        
            return;
        }

        this.loading = true;
        this._reportSmsService.findPagingChiTietChienDich(
            this.idChienDich, 
            this.idDanhBa ??  0,
            { 
                ...this.query, 
                keyword: this.searchForm.get('search')?.value,
                trangThai: this.searchForm.get('trangThai')?.value || ''
            }
        ).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map(item => ({
                        ...item,
                        tenBrandName: item.brandName?.tenBrandName || '',
                        gia: item.log?.price || 0,
                        messageText: item.log?.message || '',
                        ngayGui : item.log?.ngayGui || '',
                        ...(this.isSelectMode ? { checked: this.isAllSelected || this.selectedItems.some(s => s.idDanhBa === item.idDanhBa && s.idDanhBaSms === item.idDanhBaSms) } : {})
                    }));
                    this.totalRecords = res.data.totalItems;
                    
                    const hasChiPhi = res.data.items.some(item => item.log?.price != null);
                    if (!hasChiPhi) {
                        this.columns = this.columns.filter(c => c.field !== 'gia');
                    } else if (!this.columns.some(c => c.field === 'gia')) {
                        const messageIndex = this.columns.findIndex(c => c.field === 'messageText');
                        this.columns.splice(messageIndex, 0, { header: 'Chi Phí', field: 'gia', headerContainerStyle: 'min-width: 6rem' });
                    }
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}