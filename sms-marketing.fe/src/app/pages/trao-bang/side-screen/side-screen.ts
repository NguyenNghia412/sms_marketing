import { IViewSubPlanSideScreen } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SvNhanBangStatuses, TraoBangHubConst } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, OnDestroy } from '@angular/core';
import { DividerModule } from 'primeng/divider';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-side-screen',
  imports: [SharedImports, TableModule, TagModule, DividerModule],
  templateUrl: './side-screen.html',
  styleUrl: './side-screen.scss'
})
export class SideScreen extends BaseComponent implements OnDestroy {

  constStatuses = SvNhanBangStatuses;

  _svTraoBangService = inject(TraoBangSvService);
  data: IViewSubPlanSideScreen = {
    items: []
  };
  hubConnection: signalR.HubConnection | undefined;
  removing: boolean = false;
  newlyAdded: number | null = null;

  override ngOnInit(): void {
    this.initData();
    this.connectHub();
  }

  initData() {
    this._svTraoBangService.getSvNhanBangKhoa().subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.data = res.data
        } else {
          this.data = {
            items: []
          }
        }
      }
    })
  }

  getHangDoi() {
    this._svTraoBangService.getSvNhanBangKhoa().subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {

          if (this.isUseAnimation(this.data)) {

            this.removing = true;
            setTimeout(() => {
              this.removing = false
              this.data = res.data
            }, 600);
          } else {

            this.data = res.data
          }

        } else {
          this.data = {
            items: []
          }
        }
      }
    })
  }

  isUseAnimation(data: IViewSubPlanSideScreen) {
    const sv1 = (data.items && data.items.length > 0) ? data.items[0] : null;
    const sv2 = (data.items && data.items.length > 1) ? data.items[1] : null;
    return sv1 && sv1.trangThai === SvNhanBangStatuses.DANG_TRAO_BANG && sv2 &&  sv2.trangThai === SvNhanBangStatuses.CHUAN_BI;
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

      // if (!idSubPlan) return;

      this.getHangDoi();
      // this.initData();
    });

    this.hubConnection.on(TraoBangHubConst.ReceiveCheckIn, (...args) => {
      const idSubPlan = args[0];

      // if (!idSubPlan) return;

      this.getHangDoi();
      // this.initData();
    });

    this.hubConnection.on(TraoBangHubConst.ReceiveChonKhoa, (...args) => {
      const idSubPlan = args[0];

      // if (!idSubPlan) return;

      this.getHangDoi();
      // this.initData();
    });

    this.hubConnection.start().then();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop().then();
  }
}
