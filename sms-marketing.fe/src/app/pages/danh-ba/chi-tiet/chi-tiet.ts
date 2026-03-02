import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, EventEmitter, inject, Injector, ViewChild } from '@angular/core';
import { Import } from './import/import';
import { FormControl, FormGroup } from '@angular/forms';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn } from '@/shared/models/data-table.models';
import { IFindPagingNguoiNhan, IViewRowNguoiNhan } from '@/models/danh-ba.models';
import { MenuItem } from 'primeng/api';
import { Menu, MenuModule } from 'primeng/menu';
import { Breadcrumb } from '@/shared/components/breadcrumb/breadcrumb';
import { CreateQuickSms } from './create-quick-sms/create-quick-sms';
import { TblAction } from './tbl-action/tbl-action';
import { UpdateThueBao } from './update-thue-bao/update-thue-bao';
import { UpdateChiTietDuLieuThueBao } from './update-chi-tiet-du-lieu-thue-bao/update-chi-tiet-du-lieu-thue-bao';
import { TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { TBL_CUSTOM_COMP_EMIT } from '@/shared/components/data-table/data-table';

interface IDisplayItem {
    type: 'column' | 'separator';
    col?: IColumn;
    hiddenCols?: IColumn[];
}

@Component({
    selector: 'app-chi-tiet',
    imports: [SharedImports, Breadcrumb, MenuModule, TableModule, PaginatorModule],
    templateUrl: './chi-tiet.html',
    styleUrl: './chi-tiet.scss'
})
export class ChiTiet extends BaseComponent {
    @ViewChild('actionMenu') actionMenu!: Menu;
    @ViewChild('paginator') paginator!: Paginator;

    items: MenuItem[] = [{ label: 'Danh bạ', routerLink: '/danh-ba/ds' }, { label: 'Danh sách người nhận' }];
    home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };
    menuItems: MenuItem[] = [
        {
            label: 'Import Excel',
            icon: 'pi pi-file-import',
            command: () => this.openImportDialog()
        },
        {
            label: 'Thêm nhanh người nhận',
            icon: 'pi pi-user-plus',
            command: () => this.themNhanhNguoiNhan()
        }
    ];

    _danhBaService = inject(DanhBaService);
    private _injector = inject(Injector);

    idDanhBa: number = 0;
    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [];
    displayItems: IDisplayItem[] = [];
    data: IViewRowNguoiNhan[] = [];
    query: IFindPagingNguoiNhan = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
        idDanhBa: 0
    };

    cellViewTypes = CellViewTypes;
    customEmit = new EventEmitter<any>();
    customInjector!: Injector;

    hiddenColumns = new Set<string>();

    contextMenuVisible = false;
    contextMenuX = 0;
    contextMenuY = 0;
    contextMenuTargetCol: IColumn | null = null;

    showColMenuVisible = false;
    showColMenuX = 0;
    showColMenuY = 0;
    showColTargetCols: IColumn[] = [];

    override ngOnInit(): void {
        this.customInjector = Injector.create({
            providers: [{ provide: TBL_CUSTOM_COMP_EMIT, useValue: this.customEmit }],
            parent: this._injector
        });
        this.customEmit.subscribe((data: any) => this.onCustomEmit(data));

        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.idDanhBa = Number(params.get('id'));
            this.query.idDanhBa = this.idDanhBa;
            this.getData();
        });
    }

    onError(message: string) {
        this.messageError(message);
    }

    openImportDialog() {
        const ref = this._dialogService.open(Import, { header: 'Import người nhận', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data: { idDanhBa: this.idDanhBa } });
        ref.onClose.subscribe((result) => {
            if (result) {
                location.reload();
            }
        });
    }

    themNhanhNguoiNhan() {
        const ref = this._dialogService.open(CreateQuickSms, { header: 'Thêm nhanh người nhận', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data: { idDanhBa: this.idDanhBa } });
        ref.onClose.subscribe((result) => {
            if (result) {
                location.reload();
            }
        });
    }

    onSearch() {
        this.getData();
    }

    onPageChanged($event: any) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    getData() {
        this.loading = true;
        this._danhBaService
            .findPagingNguoiNhan({ ...this.query, keyword: this.searchForm.get('search')?.value })
            .subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, false)) {
                        const rawItems = res.data.items as IViewRowNguoiNhan[];
                        if (rawItems.length > 0) {
                            this.columns = this.buildColumns(rawItems[0]);
                            this.data = this.mapDataForCols(rawItems);
                        } else {
                            this.columns = [];
                            this.data = [];
                        }
                        this.totalRecords = res.data.totalItems;
                        this.buildDisplayItems();
                    }
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    private buildColumns(firstItem: IViewRowNguoiNhan): IColumn[] {
        const cols: IColumn[] = [
            { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width:5rem', cellStyle: 'text-align:center' },
            { header: 'Họ và tên được lưu', field: 'hoVaTen', headerContainerStyle: 'width: 12rem', cellStyle: 'text-align: center' },
            { header: 'Số điện thoại được lưu', field: 'soDienThoai', headerContainerStyle: 'width: 12rem', cellStyle: 'text-align: center' },
        ];
        if (firstItem.items && firstItem.items.length > 0) {
            firstItem.items.forEach((item) => {
                cols.push({
                    header: item.tenTruong || '',
                    field: `field_${item.id}`,
                    headerContainerStyle: 'width: 10rem',
                    cellStyle: 'text-align: center'
                });
            });
        }
        cols.push({
            header: 'Thao Tác',
            headerContainerStyle: 'width:6rem',
            cellStyle: 'text-align:center',
            cellViewType: CellViewTypes.CUSTOM_COMP,
            customComponent: TblAction,
        });
        return cols;
    }

    private mapDataForCols(items: IViewRowNguoiNhan[]): any[] {
        return items.map((row) => {
            const mapData: any = {
                id: row.id,
                hoVaTen: row.hoVaTen,
                soDienThoai: row.soDienThoai,
            };
            if (row.items) {
                row.items.forEach((item) => {
                    mapData[`field_${item.id}`] = item.data.data || '';
                });
            }
            return mapData;
        });
    }

    // ---- Column hide/show ----

    buildDisplayItems(): void {
        const items: IDisplayItem[] = [];
        let pendingHidden: IColumn[] = [];
        for (const col of this.columns) {
            const key = this.getColumnKey(col);
            if (this.hiddenColumns.has(key)) {
                pendingHidden.push(col);
            } else {
                if (pendingHidden.length > 0) {
                    items.push({ type: 'separator', hiddenCols: [...pendingHidden] });
                    pendingHidden = [];
                }
                items.push({ type: 'column', col });
            }
        }
        if (pendingHidden.length > 0) {
            items.push({ type: 'separator', hiddenCols: [...pendingHidden] });
        }
        this.displayItems = items;
    }

    getColumnKey(col: IColumn): string {
        return col.field || col.header;
    }

    isFixedColumn(col: IColumn): boolean {
        if (col.cellViewType === CellViewTypes.INDEX) return true;
        if (col.cellViewType === CellViewTypes.CUSTOM_COMP) return true;
        if (col.field === 'hoVaTen') return true;
        if (col.field === 'soDienThoai') return true;
        return false;
    }

    onHeaderRightClick(event: MouseEvent, col: IColumn): void {
        if (this.isFixedColumn(col)) return;
        event.preventDefault();
        event.stopPropagation();
        this.closeAllMenus();
        this.contextMenuTargetCol = col;
        this.contextMenuX = event.clientX;
        this.contextMenuY = event.clientY;
        this.contextMenuVisible = true;
    }

    hideColumn(): void {
        if (!this.contextMenuTargetCol) return;
        const key = this.getColumnKey(this.contextMenuTargetCol);
        this.hiddenColumns.add(key);
        this.contextMenuVisible = false;
        this.contextMenuTargetCol = null;
        this.buildDisplayItems();
    }

    onSeparatorClick(event: MouseEvent, hiddenCols: IColumn[]): void {
        event.stopPropagation();
        this.closeAllMenus();
        this.showColTargetCols = hiddenCols;
        this.showColMenuX = event.clientX;
        this.showColMenuY = event.clientY;
        this.showColMenuVisible = true;
    }

    showColumn(col: IColumn): void {
        const key = this.getColumnKey(col);
        this.hiddenColumns.delete(key);
        this.showColMenuVisible = false;
        this.showColTargetCols = [];
        this.buildDisplayItems();
    }

    showAllHiddenColumns(cols: IColumn[]): void {
        for (const col of cols) {
            this.hiddenColumns.delete(this.getColumnKey(col));
        }
        this.showColMenuVisible = false;
        this.showColTargetCols = [];
        this.buildDisplayItems();
    }

    closeAllMenus(): void {
        this.contextMenuVisible = false;
        this.showColMenuVisible = false;
    }

    getIndexValue(rowIndex: number): number {
        return (this.query.pageNumber * this.query.pageSize) - (this.query.pageSize - rowIndex) + 1;
    }

    goToPageNumber(pageNumber: number): void {
        const totalPages = Math.ceil(this.totalRecords / this.query.pageSize);
        if (pageNumber < 1 || pageNumber > totalPages) {
            this.messageError(`Số trang không hợp lệ! Vui lòng nhập từ 1 đến ${totalPages}`);
            return;
        }
        if (this.paginator) {
            this.paginator.changePage(pageNumber - 1);
        }
        this.query.pageNumber = pageNumber;
        this.getData();
    }



    onDelete(data: IViewRowNguoiNhan) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa người nhận này?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._danhBaService.deleteNguoiNhan(this.idDanhBa, data.id || 0).subscribe(
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

    onUpdateThueBao(data: IViewRowNguoiNhan) {
        const ref = this._dialogService.open(UpdateThueBao, { header: 'Cập nhật thuê bao', closable: true, modal: true, styleClass: 'w-[400px]', focusOnShow: false, data: { idDanhBa: this.idDanhBa, idThueBao: data.id } });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onUpdateChiTietDuLieuThueBao(data: IViewRowNguoiNhan) {
        const ref = this._dialogService.open(UpdateChiTietDuLieuThueBao, { header: 'Cập nhật chi tiết dữ liệu thuê bao', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data: { idDanhBa: this.idDanhBa, idThueBao: data.id } });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onCustomEmit(data: { type: string; data: IViewRowNguoiNhan }) {
        if (data.type === 'updateThueBao') {
            this.onUpdateThueBao(data.data);
        } else if (data.type === 'updateChiTietDuLieuThueBao') {
            this.onUpdateChiTietDuLieuThueBao(data.data);
        } else if (data.type === 'delete') {
            this.onDelete(data.data);
        }
    }
}
