import { Component, inject, OnDestroy, Renderer2 } from '@angular/core';
import { SharedImports } from '@/shared/import.shared';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { SubPlanStatuses, TraoBangHubConst } from '@/shared/constants/sv-nhan-bang.constants';
import { IViewScanQrCurrentSubPlan, IViewScanQrSubPlan, IViewScanQrTienDoSv, IViewSvBatDauLuiResponse, IViewSvDangTraoBang, IViewTienDoTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import * as signalR from '@microsoft/signalr';
import { LeftSidebar } from '../scan-qr-sv/left-sidebar/left-sidebar';
import { StudentList } from '../scan-qr-sv/student-list/student-list';
import { concatMap } from 'rxjs';
import { SvInfo } from "./sv-info/sv-info";

@Component({
  selector: 'app-mc-screen',
  imports: [SharedImports, LeftSidebar, StudentList, SvInfo],
  templateUrl: './mc-screen.html',
  styleUrl: './mc-screen.scss'
})
export class McScreen extends BaseComponent implements OnDestroy {

  private removeListener?: () => void;
  hubConnection: signalR.HubConnection | undefined;
  _svTraoBangService = inject(TraoBangSvService);
  renderer = inject(Renderer2)

  idSubPlan: number = 0;
  currentSubPlanInfo: IViewScanQrCurrentSubPlan | null = {};
  students: IViewScanQrTienDoSv[] = [];
  listSubPlan: IViewScanQrSubPlan[] = [];
  svDangTrao: IViewSvDangTraoBang | null = {};
  svChuanBi: IViewSvDangTraoBang | null = {};
  svBatDauLui: IViewSvBatDauLuiResponse | null = {};
  svChuanBiTiếpTheo: IViewSvDangTraoBang | null = {};
  tienDo: IViewTienDoTraoBang = { tienDo: '' };

  //isLockNextTraoBang: boolean = false;
  isLoadingNext: boolean = false;
  isLoadingPrev: boolean = false;
  isViewingSvBatDauLui: boolean = false;
 isHiddenSvBatDauLuiButton: boolean = true;
  

  countDown = 0;
  timeLeft = 0;

  override ngOnInit(): void {
    this.initData();
    this.connectHub();
    this.removeListener = this.renderer.listen('document', 'keydown', (event: KeyboardEvent) => {
      if (event.key === 'Enter' && event.shiftKey) {
        this.prevTraoBang();
        console.log('shift enter')
      }
      if (event.key === 'Enter') {
        this.onClickNextTraoBang();
        console.log('enter')
      }
      
    });

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
    this._svTraoBangService.getHangDoi({ SoLuong: 7 })
      .pipe(
        concatMap(res => {
          if (this.isResponseSucceed(res)) {
            this.students = res.data
          } else {
            this.students = [];
          }
          return this._svTraoBangService.getSvChuanBi(this.idSubPlan);
        })
      )
      .subscribe({
        next: res => {
          if (this.isResponseSucceed(res)) {
            this.svChuanBi = res.data
          }
        }
      })
  }

  getSvDangTrao() {
    this._svTraoBangService.getSvDangTraoBang()
      .subscribe({
        next: res => {
          if (this.isResponseSucceed(res)) {
            this.svDangTrao = res.data
          }
        }
      })
  }

  onClickNextTraoBang() {

    //if (this.isLockNextTraoBang) {
      //return;
    //}
    if (this.isLoadingNext || this.isLoadingPrev) {
      return;
    }
    this.isViewingSvBatDauLui = false;

    this.isLoadingNext = true;

    this._svTraoBangService.nextSvNhanBang(this.idSubPlan).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          // this.svDangTrao = res.data
          this.getSvDangTrao();
          this.getHangDoi();
          this.getCurrentSubPlan();
          const data = [...this.listSubPlan]
          this.listSubPlan = data;
        }
      },
    })/*.add(() => {
      this.timeLeft = this.countDown;
      const x = setInterval(() => {
        this.timeLeft -=1
        if (this.timeLeft === 0) {
          clearInterval(x);
        }
      }, 1000);
      setTimeout(() => {
        this.isLockNextTraoBang = false;
      }, this.countDown * 1000);
    });*/
                .add(() => {
                this.isLoadingNext = false;
            });
  }

  onClickViewSinhVienBatDauLui() {
  this.isHiddenSvBatDauLuiButton = true;
  this._svTraoBangService.getSvBatDauLui(this.idSubPlan)
    .subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.svBatDauLui = res.data;
          this.svDangTrao = res.data?.svBatDauLui || null;

          this.svChuanBi = res.data?.svChuanBiTiepTheo || null;

          const mssv = res.data?.svBatDauLui?.maSoSinhVien || '';

          if (mssv) {
            this._svTraoBangService.getHangDoiSinhVienBatDauLui({ Mssv: mssv, SoLuong: 7 })
              .subscribe({
                next: resHangDoi => {
                  if (this.isResponseSucceed(resHangDoi)) {
                    this.students = resHangDoi.data;
                  }
                  this.isViewingSvBatDauLui = true;
                }
              });
          } else {
            this.isViewingSvBatDauLui = true;
          }
            this._svTraoBangService.getCurrentSubPlanById(this.idSubPlan).subscribe({
              next: res => {
                if (this.isResponseSucceed(res)) {
            this.currentSubPlanInfo = res.data
          }
            
      }
    })
             this._svTraoBangService.getTienDoTraoBang().subscribe({
              next: (res) => {
                if (this.isResponseSucceed(res)) {
            this.tienDo = res.data;
            const data = [...this.listSubPlan]
            this.listSubPlan = data;
          }
          }
        })
  }
  }
  });
}
  onClickPrevTraoBang(event: any) {
    event?.target?.blur();
    this.prevTraoBang();
  }

  prevTraoBang() {
    if (this.isLoadingPrev || this.isLoadingNext) {
      return;
    }
    this.isHiddenSvBatDauLuiButton = false;
    this.isLoadingPrev = true;
    this._svTraoBangService.prevSvNhanBang(this.idSubPlan).subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          // this.svDangTrao = res.data
          this.getSvDangTrao();
          this.getHangDoi();
          this.getCurrentSubPlan();
          const data = [...this.listSubPlan]
          this.listSubPlan = data;
        }
      }
    }).add(() => {
      this.isLoadingPrev = false;
    });
  }

  connectHub() {
    const hubUrl = TraoBangHubConst.HUB;
    console.log(hubUrl)
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on(TraoBangHubConst.ReceiveChonKhoa, (...args) => {
      const idSubPlan = args[0];

      // if (!idSubPlan) return;

      this.initData();
    });

    this.hubConnection.on(TraoBangHubConst.ReceiveCheckIn, (...args) => {
      const mssv = args[0];

      // if (!mssv) return;

      this.getHangDoi();
    });

    this.hubConnection.start().then();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop().then();
    if (this.removeListener) {
      this.removeListener();
    }
  }

}
