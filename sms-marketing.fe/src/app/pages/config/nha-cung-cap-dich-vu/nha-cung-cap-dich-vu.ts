import { IFindPagingNhaCungCapDichVu, IViewNhaCungCapDichVu, IViewUserNhaCungCapDto } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DataTable } from "@/shared/components/data-table/data-table";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { PaginatorState } from "primeng/paginator";
import { TblAction, TblActionTypes } from "./tbl-action/tbl-action";
import { CreateNhaCungCapDichVu } from "./create-nha-cung-cap-dich-vu/create-nha-cung-cap-dich-vu";
import { UpdateNhaCungCapDichVu } from "./update-nha-cung-cap-dich-vu/update-nha-cung-cap-dich-vu";

@Component({
    selector: 'app-nha-cung-cap-dich-vu',
    imports: [...SharedImports, DataTable],
    templateUrl: './nha-cung-cap-dich-vu.html',
    styleUrl: './nha-cung-cap-dich-vu.scss'
})

export class NhaCungCapDichVu extends BaseComponent{
    _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);


    searchForm: FormGroup = new FormGroup({
        seach: new FormControl(''),
        
    })

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
        { header: 'Tên Nhà Cung Cấp', field: 'name', headerContainerStyle: 'min-width: 15rem', cellClass: 'cursor-pointer hover:text-blue-800 hover:underline', clickable: true, cellStyle: 'text-align:center' },
        { header: 'API Key', field: 'apiKey', headerContainerStyle: 'min-width: 20rem', cellViewType: CellViewTypes.SECRET, cellStyle: 'text-align:center' },
        { header: 'API Secret', field: 'apiSecret', headerContainerStyle: 'min-width: 20rem', cellViewType: CellViewTypes.SECRET, cellStyle: 'text-align:center' },
        { header: 'Cấu hình Proxy', field: 'isConfigAuthReq', headerContainerStyle: 'min-width: 10rem', cellViewType: CellViewTypes.BOOL_CHECK, cellStyle: 'text-align:center' },
        { header: 'Base URL', field: 'baseUrl', headerContainerStyle: 'min-width: 20rem', cellStyle: 'text-align:center' },
        { header: 'Brand Names', field: 'brandNames', headerContainerStyle: 'min-width: 15rem', cellStyle: 'text-align:center' },
        { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction, cellStyle: 'text-align:center' }
    ];

    data: IViewNhaCungCapDichVu[] = [];
    query : IFindPagingNhaCungCapDichVu ={
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
    };

    override ngOnInit(): void {
        this.getData(); 

    }
    getData() {
        this.loading = true;
        this._nhaCungCapDichVuService.findPaging({ ...this.query, keyword: this.searchForm.value.search }).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map((item: any) => ({
                        ...item,
                        brandNames: Array.isArray(item.brandNames)
                            ? item.brandNames.map((b: any) => b.tenBrandName).join('; ')
                            : item.brandNames
                    }));
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
        const ref = this._dialogService.open(CreateNhaCungCapDichVu, { header: 'Thêm mới nhà cung cấp dịch vụ', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false });
            ref.onClose.subscribe((result) => {
                if (result) {
                    this.getData();
                }
        });
                
    }
    onOpenUpdate(data: IViewNhaCungCapDichVu) {
        const ref = this._dialogService.open(UpdateNhaCungCapDichVu, { header: 'Cập nhật thông tin nhà cung cấp dịch vụ', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data: data });
            ref.onClose.subscribe((result) => {
                if (result) {
                    this.getData();
                }
            });
    }
    onDelete(data: IViewNhaCungCapDichVu) {
                this.confirmDelete(
                    {
                        header: 'Bạn chắc chắn muốn xóa nhà cung cấp dịch vụ  này?',
                        message: 'Không thể khôi phục sau khi xóa'
                    },
                    () => {
                        this._nhaCungCapDichVuService.delete(data.id || 0).subscribe(
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
    navigateToDetail(nhaCungCapDichVu: IViewUserNhaCungCapDto) {
            if (nhaCungCapDichVu?.id) {
                    this.router.navigate(['/config/nha-cung-cap-dich-vu/user-nha-cung-cap-dich-vu'], {
                            queryParams: {
                                idNhaCungCapDichVu: nhaCungCapDichVu.id
                            }
                        });
                    }
                }
    onCustomEmit(data: { type: string; data: IViewNhaCungCapDichVu; field?: string }) {
        
            if (data.type === TblActionTypes.delete) {
                this.onDelete(data.data);
            } else if (data.type === TblActionTypes.update) {
                this.onOpenUpdate(data.data);
           
            } else if (data.type === 'cellClick' && data.field === 'name') {
                this.navigateToDetail(data.data); 
            }
        
        }
}