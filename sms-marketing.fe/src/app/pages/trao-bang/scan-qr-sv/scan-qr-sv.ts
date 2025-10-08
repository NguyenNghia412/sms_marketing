import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, OnDestroy } from '@angular/core';
import { TableModule } from 'primeng/table';
import { LeftSidebar } from "./left-sidebar/left-sidebar";
import { Header } from "./header/header";
import { StudentList } from "./student-list/student-list";
import { Footer } from "./footer/footer";
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { DialogMssv } from './dialog-mssv/dialog-mssv';
import { IViewScanQrCurrentSubPlan, IViewScanQrSubPlan, IViewScanQrTienDoSv } from '@/models/trao-bang/sv-nhan-bang.models';
import { SubPlanStatuses, TraoBangHubConst } from '@/shared/constants/sv-nhan-bang.constants';
import * as signalR from '@microsoft/signalr';
import { ScanQrService } from '@/services/scan-qr.service';
import { NgIcon } from '@ng-icons/core';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-scan-qr-sv',
  imports: [SharedImports, TableModule, LeftSidebar, Header, StudentList, Footer, NgIcon, TagModule],
  templateUrl: './scan-qr-sv.html',
  styleUrl: './scan-qr-sv.scss'
})
export class ScanQrSv extends BaseComponent implements OnDestroy {

  hubConnection: signalR.HubConnection | undefined;
  _svTraoBangService = inject(TraoBangSvService);
  _scanQrService = inject(ScanQrService);

  idSubPlan: number = 0;
  currentSubPlanInfo: IViewScanQrCurrentSubPlan = {};
  students: IViewScanQrTienDoSv[] = [];
  listSubPlan: IViewScanQrSubPlan[] = [];
  pushedSuccessSv: IViewScanQrTienDoSv = {};

  override ngOnInit(): void {
    this.initData();
    this.connectHub();
    this._scanQrService.addListener(mssv => {
      this.pushHangDoi(mssv)
    })
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
    this._svTraoBangService.getHangDoi({ SoLuong: 7 }).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.students = res.data
        } else {
          this.students = [];
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
            this.getListSubPlan();
            this.getHangDoi();
            this.getCurrentSubPlan();
          }
        }
      }).add(() => {
        this.loading = false;
      })
    }
  }

  onNextSubPlan() {
    this.loading = true;
    this._svTraoBangService.nextSubPlan().subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.initData();
        }
      }
    }).add(() => {
      this.loading = false;
    })
  }

  connectHub() {
    const hubUrl = TraoBangHubConst.HUB;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on(TraoBangHubConst.ReceiveSinhVienDangTrao, (...args) => {
      const idSubPlan = args[0];

      if (!idSubPlan) return;

      this.initData();
    });

    this.hubConnection.start().then();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop().then();
    this._scanQrService.clearListener();
  }

}
