import { Component, inject } from '@angular/core';
import { LeftSidebar } from "./left-sidebar/left-sidebar";
import { StudentList } from "./student-list/student-list";
import { SharedImports } from '@/shared/import.shared';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { SubPlanStatuses } from '@/shared/constants/sv-nhan-bang.constants';
import { IViewScanQrCurrentSubPlan, IViewScanQrSubPlan, IViewScanQrTienDoSv, IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';

@Component({
  selector: 'app-mc-screen',
  imports: [SharedImports, LeftSidebar, StudentList],
  templateUrl: './mc-screen.html',
  styleUrl: './mc-screen.scss'
})
export class McScreen extends BaseComponent {

  _svTraoBangService = inject(TraoBangSvService);
  idSubPlan: number = 0;
  currentSubPlanInfo: IViewScanQrCurrentSubPlan = {};
  students: IViewScanQrTienDoSv[] = [];
  listSubPlan: IViewScanQrSubPlan[] = [];
  svDangTrao: IViewSvDangTraoBang = {};

  override ngOnInit(): void {
    this.initData();
  }

  initData() {
    this._svTraoBangService.getQrListSubPlan(1).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.listSubPlan = res.data
          const dangTrao = this.listSubPlan.find(x => x.trangThai === SubPlanStatuses.DANG_TRAO_BANG);
          if (typeof dangTrao !== 'undefined') {
            this.idSubPlan = dangTrao.id || 0;
          } else {
            this.idSubPlan = 0;
          }
          this.getCurrentSubPlan();
          this.getHangDoi();
        }
      }
    })
    this.getSvDangTrao();
  }

   getListSubPlan() {
    this._svTraoBangService.getQrListSubPlan(1).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.listSubPlan = res.data
          const dangTrao = this.listSubPlan.find(x => x.trangThai === SubPlanStatuses.DANG_TRAO_BANG);
          if (typeof dangTrao !== 'undefined') {
            this.idSubPlan = dangTrao.id || 0;
          } else {
            this.idSubPlan = 1;
          }
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
        } else {
          this.students = [];
        }
      }
    })
  }

  getSvDangTrao() {
    this._svTraoBangService.getSvDangTraoBang().subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.svDangTrao = res.data
        }
      }
    })
  }

}
