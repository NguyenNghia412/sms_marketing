import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { IFindPagingSvNhanBang, IViewRowSvNhanBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { PaginatorState } from 'primeng/paginator';
import { Create } from './create/create';
import { TraoBangSubPlanService } from '@/services/trao-bang/sub-plan.service';
import { IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { concatMap } from 'rxjs';
import { IBaseResponseWithData } from '@/shared/models/request-paging.base.models';

@Component({
  selector: 'app-sv-nhan-bang',
  imports: [SharedImports, DataTable],
  templateUrl: './sv-nhan-bang.html',
  styleUrl: './sv-nhan-bang.scss'
})
export class SvNhanBang extends BaseComponent {
 _svNhanBangService = inject(TraoBangSvService);
 _subPlanService = inject(TraoBangSubPlanService);

  searchForm: FormGroup = new FormGroup({
    search: new FormControl(''),
    idSubPlan: new FormControl(null),
  });

  columns: IColumn[] = [
    { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
    { header: 'MSSV', field: 'maSoSinhVien', headerContainerStyle: 'width: 10rem' },
    { header: 'Họ tên', field: 'hoVaTen', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Tên khoa', field: 'khoaQuanLy', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Lớp', field: 'lop', headerContainerStyle: 'width: 10rem' },
    { header: 'Ngành', field: 'tenNganhDaoTao', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Ghi chú', field: 'note', headerContainerStyle: 'min-width: 10rem' },
    { header: 'Thứ tự', field: 'order', headerContainerStyle: 'width: 10rem' },
    { header: 'QR', field: 'linkQR', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.LINK_BLANK },
    { header: 'Trạng thái', field: 'trangThai', headerContainerStyle: 'width: 10rem' },
    { header: 'Hiển thị', field: 'isShow', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.CHECKBOX },
    { header: 'Thao tác', headerContainerStyle: 'width: 6rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
  ];

  listSubPlan: IViewRowConfigSubPlan[] = [];
  data: IViewRowSvNhanBang[] = [];
  query: IFindPagingSvNhanBang = {
    pageNumber: this.START_PAGE_NUMBER,
    pageSize: this.MAX_PAGE_SIZE,
  };

  override ngOnInit(): void {
    this.initData();
  }

  initData() {
    this.loading = true;
    this._subPlanService.getList(1).pipe(
      concatMap((res) => {
        if (this.isResponseSucceed(res)) {
          this.listSubPlan = res.data;
          if (Array.isArray(res.data) && res.data.length > 0) {
            this.query.IdSubPlan = res.data[0].id;
            this.searchForm.patchValue({
              idSubPlan: this.query.IdSubPlan,
            });
          }
        }
        return this._svNhanBangService.findPaging(this.query);
      })
    ).subscribe({
      next: res => {
        if (this.isResponseSucceed(res, false)) {
            this.data = res.data.items;
            this.totalRecords = res.data.totalItems;
          }
      }
    }).add(() => {
      this.loading = false;
    })
  }

  onSearch() {
    this.getData();
  }

  onChangeSubPlan() {
    this.query.pageNumber = this.START_PAGE_NUMBER;
    this.onSearch();
  }

  onPageChanged($event: PaginatorState) {
    this.query.pageNumber = ($event.page ?? 0) + 1;
    this.getData();
  }

  getData() {
    this.loading = true;
    this._svNhanBangService
      .findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value, IdSubPlan: this.searchForm.get('idSubPlan')?.value })
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
    const ref = this._dialogService.open(Create, { header: 'Tạo SV', closable: true, modal: true, styleClass: 'w-[900px]', focusOnShow: false });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onOpenUpdate(data: IViewRowSvNhanBang) {
    const ref = this._dialogService.open(Create, { header: 'Cập nhật SV', closable: true, modal: true, styleClass: 'w-[900px]', focusOnShow: false, data });
    ref.onClose.subscribe((result) => {
      if (result) {
        this.getData();
      }
    });
  }

  onDelete(data: IViewRowSvNhanBang) {
    this.confirmDelete(
      {
        header: 'Bạn chắc chắn muốn xóa SV?',
        message: 'Không thể khôi phục sau khi xóa'
      },
      () => {
        // this._svNhanBangService.delete(data.id || 0, data.idPlan || 0).subscribe(
        //   (res) => {
        //     if (this.isResponseSucceed(res, true, 'Đã xóa')) {
        //       this.getData();
        //     }
        //   },
        //   (err) => {
        //     this.messageError(err?.message);
        //   }
        // );
      }
    );
  }

  onCustomEmit(data: { type: string; data: IViewRowSvNhanBang; field?: string }) {
    if (data.type === TblActionTypes.update) {
      this.onOpenUpdate(data.data);
    } else if (data.type === TblActionTypes.delete) {
      this.onDelete(data.data);
    }
  }
}
