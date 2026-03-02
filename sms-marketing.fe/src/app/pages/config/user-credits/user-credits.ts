import { IFindPagingUserCredits, IViewUserCredits } from "@/models/user-credits.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DataTable } from "@/shared/components/data-table/data-table";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { TblAction, TblActionTypes } from "./tbl-action/tbl-action";
import { PaginatorState } from "primeng/paginator";
import { CreateUserCredits } from "./create-user-credits/create-user-credits";
import { UpdateUserCredits } from "./update-user-credits/update-user-credits";



@Component({
    selector: 'app-user-credits',
    imports: [...SharedImports, DataTable],
    templateUrl: './user-credits.html',
    styleUrl: './user-credits.scss'
})

export class UserCredits extends BaseComponent{

    _userCreditsService = inject(UserCreditsService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        //createdBy: new FormControl(''),
        //sendTime: new FormControl(''),
        //status: new FormControl('')
        userName: new FormControl(''),
        fullName: new FormControl(''),
        email: new FormControl(''),
    });

    columns :IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
        { header: 'Tên', field: 'user.userName', headerContainerStyle: 'min-width: 12rem', cellClass: 'cursor-pointer hover:text-blue-800 hover:underline', clickable: true ,cellStyle: 'text-align:center' },
        { header: 'Họ và tên', field: 'user.fullName', headerContainerStyle: 'min-width: 30rem',cellStyle: 'text-align:center' },
        { header: 'Email', field: 'user.email', headerContainerStyle: 'min-width: 12rem',cellStyle: 'text-align:center' },
        { header: 'Hạn mức credit', field: 'hanMucCredit', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
        { header: 'Thời gian bắt đầu áp dụng hạn mức', field: 'thoiGianBatDauApDungHanMuc', headerContainerStyle: 'min-width: 12rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' ,cellStyle: 'text-align:center'},
        { header: 'Thời gian kết thúc áp dụng hạn mức', field: 'thoiGianKetThucApDungHanMuc', headerContainerStyle: 'min-width: 12rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' ,cellStyle: 'text-align:center'},
        { header: 'Nhà cung cấp', field: '_tenNhaCungCapDichVu', headerContainerStyle: 'min-width: 12rem', cellStyle: 'text-align:center' },
        { header: 'BrandName', field: '_tenBrandName', headerContainerStyle: 'min-width: 12rem', cellStyle: 'text-align:center' },
        { header: 'Credit đã sử dụng', field: 'creditDaSuDung', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
        { header: 'Credit còn lại', field: 'creditChuaSuDung', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
        { header: 'Credit còn lại sau khi hết thời gian áp dụng hạn mức', field: 'creditConSauKhiKetThucThoiGianApDungHanMuc', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
        { header: 'Đơn vị', field: 'donVi', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
        { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction ,cellStyle: 'text-align:center'}
    ];

    data: IViewUserCredits[] = [];
    query : IFindPagingUserCredits = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE,
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    getData(){
        this.loading = true;
        this._userCreditsService.findPaging({...this.query,keyword: this.searchForm.get('search')?.value}).subscribe({
            next: (res) => {
                if( this.isResponseSucceed(res, false)){
                    this.data = res.data.items.map((item: IViewUserCredits) => ({
                        ...item,
                        _tenNhaCungCapDichVu: item.nhaCungCapDichVus?.map(ncc => ncc.tenNhaCungCapDichVu).join('; ') || '',
                        _tenBrandName: item.nhaCungCapDichVus?.flatMap(ncc => ncc.brandNames?.map(bn => bn.tenBrandName) || []).join('; ') || '',
                    }));
                    this.totalRecords = res.data.totalItems;
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }
    onOpenCreate(){
        const ref = this._dialogService.open(CreateUserCredits, { header: 'Thêm mới hạn mức người dùng', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false });
                ref.onClose.subscribe((result) => {
                    if (result) {
                            this.getData();
                    }
            });
    }

    onOpenUpdate(data: IViewUserCredits) {
                const ref = this._dialogService.open(UpdateUserCredits, { header: 'Cập nhật thông tin hạn mức người dùng', closable: true, modal: true, styleClass: 'w-[600px]', focusOnShow: false, data: { id: data.id } });
                ref.onClose.subscribe((result) => {
                    if (result) {
                        this.getData();
                    }
                });
            }
    onDelete(data: IViewUserCredits) {
                this.confirmDelete(
                    {
                        header: 'Bạn chắc chắn muốn xóa hạn mức người dùng này?',
                        message: 'Không thể khôi phục sau khi xóa'
                    },
                    () => {
                        this._userCreditsService.deleteUserCredits(data.id || 0).subscribe(
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

    onCustomEmit(data: { type: string; data: IViewUserCredits; field?: string }) {
        if (data.type === TblActionTypes.delete) {
                this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
                this.onOpenUpdate(data.data);
                  
        }
         else if (data.type === 'cellClick' && data.field === 'user.userName') {
                this.navigateToDetail(data.data); 
    }
    }
    navigateToDetail(user: IViewUserCredits) {
                if (user?.user?.userId) {
                        this.router.navigate(['/config/user-credits/user-credits-for-user'], {
                                queryParams: {
                                    userId: user.user.userId
                                }
                            });
                        }
                    }


}