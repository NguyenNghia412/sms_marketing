import { ICreateMauNoiDung, IFindPagingMauNoiDung, IUpdateMauNoiDung, IViewRowMauNoiDung } from '@/models/template.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TemplateService {
    api = '/api/core/mau-noi-dung';
    http = inject(HttpClient);

    findPaging(query: IFindPagingMauNoiDung) {
        return this.http.get<IBaseResponsePaging<IViewRowMauNoiDung>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateMauNoiDung) {
        return this.http.post<IBaseResponse>(this.api, body);
    }

    update(body: IUpdateMauNoiDung) {
        return this.http.put<IBaseResponse>(`${this.api}?idMauNoiDung=${body.idMauNoiDung}`, body);
    }

    delete(idMauNoiDung: number) {
        return this.http.delete<IBaseResponse>(`${this.api}?idMauNoiDung=${idMauNoiDung}`);
    }
}
