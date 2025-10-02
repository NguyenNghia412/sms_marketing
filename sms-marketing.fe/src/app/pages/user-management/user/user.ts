import { IFindPagingUser, IViewRowUser } from '@/models/user.models';
import { UserService } from '@/services/user.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PaginatorState } from 'primeng/paginator';

@Component({
    selector: 'app-user',
    imports: [SharedImports, DataTable],
    templateUrl: './user.html',
    styleUrl: './user.scss'
})
export class User extends BaseComponent {
    _userService = inject(UserService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tài khoản', field: 'userName', headerContainerStyle: 'min-width: 10rem', },
        { header: 'Họ tên', field: 'fullName', headerContainerStyle: 'min-width: 10rem', },
        { header: 'Email', field: 'email', headerContainerStyle: 'min-width: 10rem', },
        { header: 'SĐT', field: 'phoneNumber', headerContainerStyle: 'min-width: 10rem', },
        { header: 'Thời gian tạo', field: 'createdAt',headerContainerStyle: 'width: 10rem' , cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy hh:mm:ss' },
        // { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: IViewRowUser[] = [];
    query: IFindPagingUser = {
        pageNumber: this.START_PAGE_NUMBER,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.getData();
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
        this._userService
            .findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value })
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

    onOpenCreate() {}

    onCustomEmit(event: any) {}
}
