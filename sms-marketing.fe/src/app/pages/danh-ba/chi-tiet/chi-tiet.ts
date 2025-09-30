import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, ViewChild } from '@angular/core';
import { Import } from './import/import';
import { FormControl, FormGroup } from '@angular/forms';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn } from '@/shared/models/data-table.models';
import { IFindPagingNguoiNhan, IViewRowNguoiNhan } from '@/models/danh-ba.models';
import { PaginatorState } from 'primeng/paginator';
import { DataTable } from '@/shared/components/data-table/data-table';
import { MenuItem } from 'primeng/api';
import { Menu, MenuModule } from 'primeng/menu';
import { Breadcrumb } from '@/shared/components/breadcrumb/breadcrumb';
import { CreateQuickSms } from './create-quick-sms/create-quick-sms';
import { TblAction } from './tbl-action/tbl-action';

@Component({
    selector: 'app-chi-tiet',
    imports: [SharedImports, DataTable, Breadcrumb,MenuModule],
    templateUrl: './chi-tiet.html',
    styleUrl: './chi-tiet.scss'
})
export class ChiTiet extends BaseComponent {
    @ViewChild('menu') menu!: Menu;

    items: MenuItem[] = [{ label: 'Danh bạ', routerLink: '/danh-ba/ds'  }, { label: 'Danh sách người nhận' }];

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

    idDanhBa: number = 0;

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        //{ header: 'Mã số', field: 'maSoNguoiDung', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Họ tên', field: 'hoVaTen', headerContainerStyle: 'min-width: 10rem' },
        { header: 'SĐT', field: 'soDienThoai', headerContainerStyle: 'min-width: 10rem' },
        //{ header: 'Email', field: 'emailHuce', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Thao tác', headerContainerStyle: 'width: 8rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowNguoiNhan[] = [];
    query: IFindPagingNguoiNhan = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
        idDanhBa: 0
    };

    override ngOnInit(): void {
        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.idDanhBa = Number(params.get('id'));
            this.query.idDanhBa = this.idDanhBa;
            this.getData();
        });
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

    onPageChanged($event: PaginatorState) {
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
                        this.data = res.data.items;
                        this.totalRecords = res.data.totalItems;
                    }
                }
            })
            .add(() => {
                this.loading = false;
            });
    }
    onDelete(data: IViewRowNguoiNhan) {
            this.confirmDelete(
                {
                    header: 'Bạn chắc chắn muốn xóa người nhận này?',
                    message: 'Không thể khôi phục sau khi xóa'
                },
                () => {
                    this._danhBaService.deleteNguoiNhan(this.idDanhBa,data.id || 0).subscribe(
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
    onCustomEmit(data: { type: string; data: IViewRowNguoiNhan }) {
         if (data.type === 'delete') {
        this.onDelete(data.data);
    }
    }
 
}
