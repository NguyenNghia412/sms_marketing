import { IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { TraoBangConst } from '@/shared/constants/sv-nhan-bang.constants';
import { Component, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-main-screen',
  imports: [],
  templateUrl: './main-screen.html',
  styleUrl: './main-screen.scss'
})
export class MainScreen extends BaseComponent {

  _svTraoBangService = inject(TraoBangSvService);
  svDangTrao: IViewSvDangTraoBang = {};
  hubConnection: signalR.HubConnection | undefined;

  override ngOnInit(): void {
    this.getSvDangTrao();
    this.connectHub();
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
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on('PayBill', (...args) => {
      // const data = args[0]; // Vì backend chỉ truyền 1 object
      // if (!data) return;

      // const { id, paymentId, trangThai } = data;

      // if (
      //   id === this.data.item.idHoSo &&
      //   paymentId === this.data.item.id &&
      //   trangThai === CandidatePaymentStatus.Paid
      // ) {
      //   this.hubConnection.stop().then();
      //   // this.messageSuccess('Thanh toán thành công');
      //   this.closeDialog(true);
      // }
    });

    this.hubConnection.start().then();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop().then();
  }
}
