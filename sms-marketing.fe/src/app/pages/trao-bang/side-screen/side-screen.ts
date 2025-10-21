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
  recentlyCheckedInId: number | null = null;

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
        if (res.data.items && this.data.items) {
          res.data.items.forEach(newSv => {
            const oldSv = this.data.items?.find(x => x.id === newSv.id);
            if (oldSv && oldSv.trangThai === 1 && newSv.trangThai === 2 && newSv.id) {
              this.recentlyCheckedInId = newSv.id;
              setTimeout(() => {
                this.recentlyCheckedInId = null;
              }, 3000);
            }
          });
        }

        if (this.isUseAnimation(this.data, res.data)) {
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
  isUseAnimation(data: IViewSubPlanSideScreen, newData: IViewSubPlanSideScreen) {
    const newSv1 = (newData.items && newData.items.length > 0) ? newData.items[0] : null;
    const oldSv2 = (data.items && data.items.length > 1) ? data.items[1] : null;
    return newSv1 && oldSv2 && newSv1.id === oldSv2.id
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
