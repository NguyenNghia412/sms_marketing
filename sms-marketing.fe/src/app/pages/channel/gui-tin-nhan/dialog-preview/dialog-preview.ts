import { ISendSms, IViewPreviewSendSms, IViewVerifySendSms } from '@/models/gui-tin-nhan.models';
import { GuiTinNhanService } from '@/services/gui-tin-nhan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, input } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-dialog-preview',
  imports: [SharedImports, MessageModule],
  templateUrl: './dialog-preview.html',
})
export class DialogPreview extends BaseComponent {

  private _guiTinNhanService = inject(GuiTinNhanService);
  private _ref = inject(DynamicDialogRef);
  private _config = inject(DynamicDialogConfig);

  sendStatusList = {
    idle: 'idle',
    sending: 'sending',
    success: 'success',
    error: 'error'
  }
  verifyData: IViewVerifySendSms = {};
  showMsg = true;
  sendStatus: string = this.sendStatusList.idle;

  override ngOnInit(): void {
    this.sendStatus = this.sendStatusList.idle;
    this.verifyData = { ...this._config.data?.verifyData };
  }

  onSendSms() {
    const body: ISendSms = this._config.data?.bodySendSms;

    this.sendStatus = this.sendStatusList.sending;
    this._guiTinNhanService.sendSms(body).subscribe({
      next: (res) => {
        if (this.isResponseSucceed(res, true, 'Đã đặt lệnh gửi')) {
          this.sendStatus = this.sendStatusList.success;
          // this.router.navigate(['/channel/sms']);
        } else {
          this.sendStatus = this.sendStatusList.error;
        }
      },
      error: (err) => {
        this.messageError(err?.message);
        this.sendStatus = this.sendStatusList.error;
      }
    });
  }

  onSubmit() {
    this.onSendSms();
  }
}
