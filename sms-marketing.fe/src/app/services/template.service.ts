import { ICreateSMSTempalte, IFindPagingSMSTempalte, IUpdateSMSTempalte, SMSTempalte } from '@/pages/template/models/sms-template.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TemplateService {
    api = '/api/core/mau-noi-dung';
    http = inject(HttpClient);

    findPaging(query: IFindPagingSMSTempalte) {
        return this.http.get<IBaseResponsePaging<SMSTempalte>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateSMSTempalte) {
        return this.http.post<IBaseResponse>(this.api, body);
    }

    update(body: IUpdateSMSTempalte) {
        return this.http.put<IBaseResponse>(`${this.api}?id=${body.id}`, body);
    }

    delete(idMauNoiDung: number) {
        return this.http.delete<IBaseResponse>(`${this.api}?id=${idMauNoiDung}`);
    }
}
