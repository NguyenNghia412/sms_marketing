import { IPreviewSendSms, ISaveConfigChienDich, ISendSms, IVerifySendSms, IViewPreviewSendSms, IViewVerifySendSms } from '@/models/gui-tin-nhan.models';
import { IBaseResponse, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class GuiTinNhanService {
    api = '/api/core/gui-tin-nhan';
    http = inject(HttpClient);

    saveConfigChienDich(body: ISaveConfigChienDich) {
        return this.http.post<IBaseResponse>(`${this.api}/save-config-chien-dich`, body);
    }

    sendSms(body: ISendSms) {
        return this.http.post<IBaseResponse>(`${this.api}/send-sms`, body);
    }

    previewSendSms(body: IPreviewSendSms) {
        return this.http.post<IBaseResponseWithData<IViewPreviewSendSms>>(`${this.api}/preview-send-sms`, body);
    }

    verifySendSms(body: IVerifySendSms) {
        return this.http.post<IBaseResponseWithData<IViewVerifySendSms>>(`${this.api}/verify-send-sms`, body);
    }

}
