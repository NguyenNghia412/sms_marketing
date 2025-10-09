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

  override ngOnInit(): void {
    this.getHangDoi();
    this.connectHub();
  }

  getHangDoi() {
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
