import { EmailTempalte, ICreateEmailTempalte, IFindPagingEmailTempalte, IUpdateEmailTempalte } from '@/pages/template/models/data-email-template.models';
import { ICreateSMSTempalte, IFindPagingSMSTempalte, IUpdateSMSTempalte, SMSTempalte } from '@/pages/template/models/sms-template.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TemplateEmailService {
    api = '/api/core/mau-noi-dung-email';
    http = inject(HttpClient);

    findPaging(query: IFindPagingEmailTempalte) {
        return this.http.get<IBaseResponsePaging<EmailTempalte>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateEmailTempalte) {
        return this.http.post<IBaseResponseWithData<any>>(this.api, body);
    }

    update(body: IUpdateEmailTempalte) {
        return this.http.put<IBaseResponse>(`${this.api}?id=${body.id}`, body);
    }

    delete(idMauNoiDung: number) {
        return this.http.delete<IBaseResponse>(`${this.api}/${idMauNoiDung}`);
    }
    getById(id:number){
        return this.http.get<IBaseResponseWithData<EmailTempalte>>(`${this.api}/${id}`)
    }
}
