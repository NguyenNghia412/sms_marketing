import { IViewPreviewSendSms } from '@/models/gui-tin-nhan.models';
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

  private _ref = inject(DynamicDialogRef);
  private _config = inject(DynamicDialogConfig);

  sendStatusList = {
    idle: 'idle',
    sending: 'sending',
    success:'success',
    error: 'error'
  }
  data: IViewPreviewSendSms = {};
  showMsg = true;
  sendStatus: string = this.sendStatusList.idle;

  override ngOnInit(): void {
    this.sendStatus = this.sendStatusList.success;
    this.data = {...this._config.data};
  }

  onSubmit() {
    this.loading = true;
    this.sendStatus = this.sendStatusList.sending;
    setTimeout(() => {
      this.loading = false;
      this.sendStatus === this.sendStatusList.success;
    }, 500);
  }
}
