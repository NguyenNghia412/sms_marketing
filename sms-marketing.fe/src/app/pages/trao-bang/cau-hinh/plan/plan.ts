import { IFindPagingConfigPlan, IViewRowConfigPlan } from '@/models/trao-bang/plan.models';
import { TraoBangPlanService } from '@/services/trao-bang/plan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PaginatorState } from 'primeng/paginator';
import { Create } from './create/create';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';

@Component({
  selector: 'app-plan',
  imports: [SharedImports, DataTable],
  templateUrl: './plan.html',
  styleUrl: './plan.scss'
})
export class Plan extends BaseComponent {
  _planService = inject(TraoBangPlanService);

  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });

  columns: IColumn[] = [
    { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
    { header: 'Tên chương trình', field: 'ten', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Mô tả', field: 'moTa', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Bắt đầu', field: 'thoiGianBatDau', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
    { header: 'Kết thúc', field: 'thoiGianKetThuc', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
    { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
  ];

  data: IViewRowConfigPlan[] = [];
  query: IFindPagingConfigPlan = {
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
    this._planService
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
    const ref = this._dialogService.open(Create, { header: 'Tạo chương trình', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onOpenUpdate(data: IViewRowConfigPlan) {
    const ref = this._dialogService.open(Create, { header: 'Cập nhật chương trình', closable: true, modal: true, styleClass: 'w-[700px]', focusOnShow: false, data });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onDelete(data: IViewRowConfigPlan) {
    this.confirmDelete(
      {
        header: 'Bạn chắc chắn muốn xóa chương trình?',
        message: 'Không thể khôi phục sau khi xóa'
      },
      () => {
        this._planService.delete(data.id || 0).subscribe(
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

  onCustomEmit(data: { type: string; data: IViewRowConfigPlan; field?: string }) {
    if (data.type === TblActionTypes.update) {
      this.onOpenUpdate(data.data);
    } else if (data.type === TblActionTypes.delete) {
      this.onDelete(data.data);
    }
  }
}
