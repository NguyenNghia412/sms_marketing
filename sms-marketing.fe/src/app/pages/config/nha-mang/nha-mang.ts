import { IViewRowChienDich } from "@/models/sms.models";
import { NhaMangService } from "@/services/nha-mang.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DataTable } from "@/shared/components/data-table/data-table";

import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject, ViewChild } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { Popover } from "primeng/popover";
import { TblAction, TblActionTypes } from "./tbl-action/tbl-action";
import { IFindPagingNhaMang, IViewNhaMang } from "@/models/nha-mang.models";
import { PaginatorState } from "primeng/paginator";
import { CreateNhaMang } from "./create-nha-mang/create-nha-mang";
import { UpdateNhaMang } from "./update-nha-mang/update-nha-mang";


@Component({
    selector: 'app-nha-mang',
    imports: [...SharedImports, DataTable],
    templateUrl: './nha-mang.html',
    styleUrl: './nha-mang.scss'
})
export class NhaMang extends BaseComponent {
    @ViewChild('filterPanel') filterPanel!: Popover;
    _nhaMangService = inject(NhaMangService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        //createdBy: new FormControl(''),
        //sendTime: new FormControl(''),
        //status: new FormControl('')
        tenNhaMang: new FormControl(''),
    });

    columns: IColumn[] = [
            { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
            { header: 'Tên Nhà mạng', field: 'tenNhaMang', headerContainerStyle: 'min-width: 12rem', cellClass: 'cursor-pointer hover:text-blue-800 hover:underline', clickable: true ,cellStyle: 'text-align:center' },
            { header: 'Prefix đầu số', field: 'prefix', headerContainerStyle: 'min-width: 30rem',cellStyle: 'text-align:center' },
            { header: 'BrandName', field: 'brandName.tenBrandName', headerContainerStyle: 'min-width: 12rem',cellStyle: 'text-align:center' },
            { header: 'Đơn giá', field: 'donGia.donGia', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
            { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction ,cellStyle: 'text-align:center'}
        ];

    data: IViewNhaMang[] = [];
    query : IFindPagingNhaMang = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
    };

    override ngOnInit(): void {
        this.getData(); 

    }
    
  
   

    getData(){
        this.loading = true;
        this._nhaMangService.findPaging({...this.query,keyword: this.searchForm.value.search}).subscribe({
            next: (res) => {
                if( this.isResponseSucceed(res, false)){
                    this.data = res.data.items;
                    this.totalRecords = res.data.totalItems;
                }
            }
        })
        .add(() => {
            this.loading = false;
        });
    }
    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }
    onSearch() {
        this.getData();
    }
    onOpenCreate() {
         const ref = this._dialogService.open(CreateNhaMang, { header: 'Thêm mới nhà mạng', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false });
                ref.onClose.subscribe((result) => {
                    if (result) {
                        this.getData();
                    }
                });
            
    }
    onOpenUpdate(data: IViewNhaMang) {
            const ref = this._dialogService.open(UpdateNhaMang, { header: 'Cập nhật thông tin nhà mạng', closable: true, modal: true, styleClass: 'w-96', focusOnShow: false, data: data });
            ref.onClose.subscribe((result) => {
                if (result) {
                    this.getData();
                }
            });
        }
    onDelete(data: IViewNhaMang) {
            this.confirmDelete(
                {
                    header: 'Bạn chắc chắn muốn xóa nhà mạng này?',
                    message: 'Không thể khôi phục sau khi xóa'
                },
                () => {
                    this._nhaMangService.deleteNhaMang(data.id || 0).subscribe(
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
    onCustomEmit(data: { type: string; data: IViewNhaMang; field?: string }) {
    
        if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
       
        }   
    
    }
}