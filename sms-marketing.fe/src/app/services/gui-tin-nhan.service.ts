import {  IPreviewSendSms, ISaveConfigChienDich, ISaveConfigChienDichLenLich, ISendSms, ISendSmsLenLich, IVerifySendSms, IVerifySendSmsLenLich, IViewPreviewSendSms, IViewVerifySendSms } from '@/models/gui-tin-nhan.models';
import { IHuyJobSendSms } from '@/models/sms.models';
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

    saveConfigChienDichLenLich(body: ISaveConfigChienDichLenLich) {
        return this.http.post<IBaseResponse>(`${this.api}/save-config-chien-dich-scheduler-job`, body);
    }
    sendSms(body: ISendSms) {
        return this.http.post<IBaseResponse>(`${this.api}/send-sms`, body);
    }

    sendSmsLenLich(body: ISendSmsLenLich) {
        return this.http.post<IBaseResponse>(`${this.api}/send-sms-scheduler-job`, body);
    }

    previewSendSms(body: IPreviewSendSms) {
        return this.http.post<IBaseResponseWithData<IViewPreviewSendSms>>(`${this.api}/preview-send-sms`, body);
    }

    verifySendSms(body: IVerifySendSms) {
        return this.http.post<IBaseResponseWithData<IViewVerifySendSms>>(`${this.api}/verify-send-sms`, body);
    }

    verifySendSmsLenLich(body: IVerifySendSmsLenLich) {
        return this.http.post<IBaseResponseWithData<IViewVerifySendSms>>(`${this.api}/verify-send-sms-scheduler-job`, body);
    }
    huyJobSendSms(body:IHuyJobSendSms){
        return this.http.post<IBaseResponse>(`${this.api}/cancel-send-sms`, body);
    }
   
}


