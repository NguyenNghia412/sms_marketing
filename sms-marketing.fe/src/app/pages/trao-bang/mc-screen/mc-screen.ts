import { Component, inject, OnDestroy } from '@angular/core';
import { SharedImports } from '@/shared/import.shared';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { SubPlanStatuses, TraoBangConst } from '@/shared/constants/sv-nhan-bang.constants';
import { IViewScanQrCurrentSubPlan, IViewScanQrSubPlan, IViewScanQrTienDoSv, IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import * as signalR from '@microsoft/signalr';
import { LeftSidebar } from '../scan-qr-sv/left-sidebar/left-sidebar';
import { StudentList } from '../scan-qr-sv/student-list/student-list';

@Component({
  selector: 'app-mc-screen',
  imports: [SharedImports, LeftSidebar, StudentList],
  templateUrl: './mc-screen.html',
  styleUrl: './mc-screen.scss'
})
export class McScreen extends BaseComponent implements OnDestroy {

  hubConnection: signalR.HubConnection | undefined;
  _svTraoBangService = inject(TraoBangSvService);
  idSubPlan: number = 0;
  currentSubPlanInfo: IViewScanQrCurrentSubPlan = {};
  students: IViewScanQrTienDoSv[] = [];
  listSubPlan: IViewScanQrSubPlan[] = [];
  svDangTrao: IViewSvDangTraoBang = {};

  override ngOnInit(): void {
    this.initData();
    this.connectHub();
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

   connectHub() {
      const hubUrl = TraoBangConst.HUB;
      console.log(hubUrl)
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
        })
        .build();
  
      this.hubConnection.on('ReceiveChonKhoa', (...args) => {
        console.log(args)
        const idSubPlan = args[0];

        if (!idSubPlan) return;
        
        this.initData();
      });

      this.hubConnection.on('ReceiveCheckIn', (...args) => {
        console.log(args)
        const mssv = args[0];

        if (!mssv) return;
        
        this.getHangDoi();
      });
  
      this.hubConnection.start().then();
    }
  
    ngOnDestroy(): void {
      this.hubConnection?.stop().then();
    }

}
