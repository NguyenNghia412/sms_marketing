import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { TableModule } from 'primeng/table';
import { LeftSidebar } from "./left-sidebar/left-sidebar";
import { Header } from "./header/header";
import { StudentList } from "./student-list/student-list";
import { Footer } from "./footer/footer";
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { DialogMssv } from './dialog-mssv/dialog-mssv';
import { IViewScanQrCurrentSubPlan, IViewScanQrSubPlan, IViewScanQrTienDoSv } from '@/models/trao-bang/sv-nhan-bang.models';

@Component({
  selector: 'app-scan-qr-sv',
  imports: [SharedImports, TableModule, LeftSidebar, Header, StudentList, Footer],
  templateUrl: './scan-qr-sv.html',
  styleUrl: './scan-qr-sv.scss'
})
export class ScanQrSv extends BaseComponent {

  _svTraoBangService = inject(TraoBangSvService);
  idSubPlan: number = 1;
  currentSubPlanInfo: IViewScanQrCurrentSubPlan = {};
  students: IViewScanQrTienDoSv[] = [];
  listSubPlan: IViewScanQrSubPlan[] = [];
  pushedSuccessSv: IViewScanQrTienDoSv = {};

  override ngOnInit(): void {
    this.getHangDoi();
    this.getListSubPlan();
    this.getCurrentSubPlan();
  }


  getListSubPlan() {
    this._svTraoBangService.getQrListSubPlan(1).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.listSubPlan = res.data
        }
      }
    })
  }

  getCurrentSubPlan() {
    this._svTraoBangService.getCurrentSubPlanById(this.idSubPlan).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.currentSubPlanInfo = res.data
        }
      }
    })
  }

  getHangDoi() {
    this._svTraoBangService.getHangDoi({ IdSubPlan: this.idSubPlan, SoLuong: 10 }).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.students = res.data
        }
      }
    })
  }

  pushHangDoi(mssv: string) {
    this.loading = true;
    this._svTraoBangService.pushHangDoi(mssv).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.pushedSuccessSv = res.data;
          this.getHangDoi();
          this.getCurrentSubPlan();
        }
      }
    }).add(() => {
      this.loading = false;
    })
  }

  onOpenDialogMssv() {
    const ref = this._dialogService.open(DialogMssv, { header: 'Chuyển SV vào hàng đợi', closable: true, modal: true, styleClass: 'w-[500px]', focusOnShow: false });
    ref.onClose.subscribe((mssv) => {
      if (mssv) {
        this.pushHangDoi(mssv);
      }
    });
  }

  onChangeSubPlan(idSubPlan: number | null | undefined) {
    if (idSubPlan) {
      this.loading = true;
      this._svTraoBangService.backToDaTraoBangSubPlan(idSubPlan).subscribe({
        next: res => {
          if (this.isResponseSucceed(res)) {
            this.idSubPlan = idSubPlan;
            this.getHangDoi();
            this.getListSubPlan();
            this.getCurrentSubPlan();
          }
        }
      }).add(() => {
        this.loading = false;
      })
    }
  }

}
