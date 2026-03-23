import { IFindPagingUserToNhaCungCapDichVuDto, IViewUserNhaCungCapDto } from "@/models/nha-cung-cap-dich-vu.models";
import { NhaCungCapDichVuService } from "@/services/nha-cung-cap-dich-vu.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DataTable } from "@/shared/components/data-table/data-table";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { PaginatorState } from "primeng/paginator";
import { TblAction, TblActionTypes } from "../tbl-action/tbl-action";
import { ActivatedRoute } from "@angular/router";
import { CreateUserSuDungDichVu } from "./create-user-su-dung-dich-vu/create-user-su-dung-dich-vu";
import { UpdateUserSuDungDichVu } from "./update-user-su-dung-dich-vu/update-user-su-dung-dich-vu";


@Component({
    selector: 'app-user-nha-cung-cap-dich-vu',
    imports: [...SharedImports, DataTable],
    templateUrl: './user-su-dung-dich-vu.html',
    styleUrl: './user-su-dung-dich-vu.scss'
})
export class UserNhaCungCapDichVu extends BaseComponent {
    private _route = inject(ActivatedRoute);
    _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);
    idNhaCungCapDichVu : number =0;

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
        { header: 'Họ tên', field: 'fullName', headerContainerStyle: 'min-width: 15rem', cellStyle: 'text-align:center' },
        { header: 'Tên đăng nhập', field: 'userName', headerContainerStyle: 'min-width: 15rem', cellStyle: 'text-align:center' },
        //{ header: 'Nhà cung cấp', field: 'nhaCungCapName', headerContainerStyle: 'min-width: 15rem', cellStyle: 'text-align:center' },
        { header: 'Brand Name', field: 'brandName', headerContainerStyle: 'min-width: 15rem', cellStyle: 'text-align:center' },
        { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction, cellStyle: 'text-align:center' }
    ];

    data: any[] = [];
    query: IFindPagingUserToNhaCungCapDichVuDto = {
        idNhaCungCapDichVu: 0,
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
    };

    override ngOnInit(): void {
        this._route.queryParams.subscribe((params: any) => {
            this.idNhaCungCapDichVu = +params['idNhaCungCapDichVu'] || 0;
            this.query.idNhaCungCapDichVu = this.idNhaCungCapDichVu;
            this.getData();
        });
    }

    getData() {
        this.loading = true;
        this._nhaCungCapDichVuService.findPagingUserNhaCungCapDichVu({ ...this.query, keyword: this.searchForm.value.search }).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.data = res.data.items.map((item: IViewUserNhaCungCapDto) => ({
                        ...item,
                        fullName: item.user?.fullName,
                        userName: item.user?.userName,
                        nhaCungCapName: item.nhaCungCapDichVu?.name,
                        brandName: item.brandName?.tenBrandName,
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
        const ref = this._dialogService.open(CreateUserSuDungDichVu, {
            header: 'Thêm user vào nhà cung cấp dịch vụ',
            closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false,
            data: { idNhaCungCapDichVu: this.idNhaCungCapDichVu }
        });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onOpenUpdate(data: any) {
        const ref = this._dialogService.open(UpdateUserSuDungDichVu, {
            header: 'Cập nhật thông tin',
            closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false,
            data: data
        });
        ref.onClose.subscribe((result) => {
            if (result) {
                this.getData();
            }
        });
    }

    onDelete(data: any) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa user khỏi nhà cung cấp dịch vụ này?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._nhaCungCapDichVuService.deleteUserToNhaCungCapDichVu(data.id || 0).subscribe(
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

    onCustomEmit(data: { type: string; data: any; field?: string }) {
        if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        }
    }
}