import { IFindPagingUserCreditsForUser, IViewUserCredits } from "@/models/user-credits.models";
import { UserCreditsService } from "@/services/user-credits.service";
import { BaseComponent } from "@/shared/components/base/base-component";
import { DataTable } from "@/shared/components/data-table/data-table";
import { LoaiApiCreditStatuses } from "@/shared/constants/config.constants";
import { CellViewTypes } from "@/shared/constants/data-table.constants";
import { SharedImports } from "@/shared/import.shared";
import { IColumn } from "@/shared/models/data-table.models";
import { Component, inject } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { TblAction } from "../user-credits/tbl-action/tbl-action";
import { PaginatorState } from "primeng/paginator";

@Component({
    selector: 'app-user-credits-for-user',
    imports: [...SharedImports, DataTable],
    templateUrl: './user-credits-for-user.html',
    styleUrl: './user-credits-for-user.scss'
})

export class UserCreditsForUser extends BaseComponent{
    _userCreditsService = inject(UserCreditsService);
    searchForm: FormGroup = new FormGroup({
        search: new FormControl(''),
        //createdBy: new FormControl(''),
        //sendTime: new FormControl(''),
        //status: new FormControl('')
        //userName: new FormControl(''),
        //fullName: new FormControl(''),
        //email: new FormControl(''),
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem', cellStyle: 'text-align:center' },
                { header: 'Tên', field: 'user.userName', headerContainerStyle: 'min-width: 12rem', cellClass: 'cursor-pointer hover:text-blue-800 hover:underline', clickable: true ,cellStyle: 'text-align:center' },
                { header: 'Họ và tên', field: 'user.fullName', headerContainerStyle: 'min-width: 30rem',cellStyle: 'text-align:center' },
                { header: 'Email', field: 'user.email', headerContainerStyle: 'min-width: 12rem',cellStyle: 'text-align:center' },
                { header: 'Hạn mức credit', field: 'hanMucCredit', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
                { header: 'Thời gian bắt đầu áp dụng hạn mức', field: 'thoiGianBatDauApDungHanMuc', headerContainerStyle: 'min-width: 12rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' ,cellStyle: 'text-align:center'},
                { header: 'Thời gian kết thúc áp dụng hạn mức', field: 'thoiGianKetThucApDungHanMuc', headerContainerStyle: 'min-width: 12rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' ,cellStyle: 'text-align:center'},
                /*{ header: 'Loại API Credit', field: 'loaiApiCreditText', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.STATUS,
                    statusSeverityFunction: (rowData: IViewUserCredits) => {
                        return LoaiApiCreditStatuses.getSeverity(rowData.loaiApiCredit ?? 0);
                    } ,cellStyle: 'text-align:center'
                },*/
                { header: 'Credit đã sử dụng', field: 'creditDaSuDung', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
                { header: 'Credit còn lại', field: 'creditChuaSuDung', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
                { header: 'Creadit còn lại sau khi hết thời gian áp dụng hạn mức', field: 'creditConSauKhiKetThucThoiGianApDungHanMuc', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
                { header: 'Đơn vị', field: 'donVi', headerContainerStyle: 'min-width: 12rem' ,cellStyle: 'text-align:center'},
                //{ header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction ,cellStyle: 'text-align:center'}
    ];

    data: IViewUserCredits[] = [];
    query :IFindPagingUserCreditsForUser ={
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
        this._userCreditsService.getByUserId({...this.query,keyword: this.searchForm.get('search')?.value}).subscribe({
            next: (res) => {
                if( this.isResponseSucceed(res, false)){    
                    this.data = res.data.items.map((item: { loaiApiCredit: number; }) => ({
                        
                            ...item,
                            //loaiApiCreditText: this.getLoaiApiCreditText(item.loaiApiCredit ?? 0)
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
}