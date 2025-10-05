import { IFindPagingConfigSubPlan, IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { TraoBangSubPlanService } from '@/services/trao-bang/sub-plan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { PaginatorState } from 'primeng/paginator';
import { Create } from './create/create';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-sub-plan',
  imports: [SharedImports, DataTable],
  templateUrl: './sub-plan.html',
  styleUrl: './sub-plan.scss'
})
export class SubPlan extends BaseComponent {
  _subPlanService = inject(TraoBangSubPlanService);

  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });

  columns: IColumn[] = [
    { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
    { header: 'Tên chương trình', field: 'ten', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Mô tả', field: 'moTa', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Mở bài', field: 'moBai', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Kết bài', field: 'ketBai', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Ghi chú', field: 'note', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Thứ tự', field: 'order', headerContainerStyle: 'width: 10rem' },
    { header: 'Hiển thị', field: 'isShow', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.CHECKBOX },
    { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
  ];

  data: IViewRowConfigSubPlan[] = [];
  query: IFindPagingConfigSubPlan = {
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
    this._subPlanService
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

  onOpenCreate() {
    const ref = this._dialogService.open(Create, { header: 'Tạo khoa', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onOpenUpdate(data: IViewRowConfigSubPlan) {
    const ref = this._dialogService.open(Create, { header: 'Cập nhật khoa', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false, data });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onDelete(data: IViewRowConfigSubPlan) {
    this.confirmDelete(
      {
        header: 'Bạn chắc chắn muốn xóa khoa?',
        message: 'Không thể khôi phục sau khi xóa'
      },
      () => {
        this._subPlanService.delete(data.id || 0, data.idPlan || 0).subscribe(
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

  onCustomEmit(data: { type: string; data: IViewRowConfigSubPlan; field?: string }) {
    if (data.type === TblActionTypes.update) {
      this.onOpenUpdate(data.data);
    } else if (data.type === TblActionTypes.delete) {
      this.onDelete(data.data);
    }
  }
}
