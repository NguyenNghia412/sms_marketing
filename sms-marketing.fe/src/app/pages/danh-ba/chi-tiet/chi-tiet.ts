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

    columns: IColumn[] = []
    

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
    onError(message:string){
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
                        const rawItems = res.data.items as IViewRowNguoiNhan[];

                        if(rawItems.length > 0){
                            this.columns = this.buildColumns(rawItems[0]);
                            this.data = this.mapDataForCols(rawItems);
                        }else{
                            this.columns = [];
                            this.data = [];
                        }

                        this.totalRecords = res.data.totalItems;

                      
                    }
                }
            })
            .add(() => {
                this.loading = false;
            });
    }

    private buildColumns(firstItem: IViewRowNguoiNhan): IColumn[] {
        const cols: IColumn[]= [
            { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width:5rem',cellStyle:' text-align:center'}
        ];
        if(firstItem.items && firstItem.items.length > 0){
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


    private mapDataForCols(items: IViewRowNguoiNhan[]): any[]{
        return items.map((row) =>{
            const mapData: any ={
                id: row.id,
                hoVaTen: row.hoVaTen,
                soDienThoai: row.soDienThoai,
            };

            if(row.items){
                row.items.forEach((item)=>{
                    mapData[`field_${item.id}`] = item.data.data || '';
                });
            }

            return mapData;
        })
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
