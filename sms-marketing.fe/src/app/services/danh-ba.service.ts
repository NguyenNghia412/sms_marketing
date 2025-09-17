import { ICreateDanhBa, IFindPagingDanhBa, IUpdateDanhBa, IViewRowDanhBa } from '@/models/danh-ba.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class DanhBaService {
    api = '/api/core/danh-ba';
    http = inject(HttpClient);

    getList() {
        return this.http.get(`${this.api}/list-danh-ba`);
    }

    findPaging(query: IFindPagingDanhBa) {
        return this.http.get<IBaseResponsePaging<IViewRowDanhBa>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateDanhBa) {
        return this.http.post<IBaseResponse>(this.api, body);
    }

    update(body: IUpdateDanhBa) {
        return this.http.put<IBaseResponse>(`${this.api}?idDanhBa=${body.id}`, body);
    }

    delete(id: number) {
        return this.http.delete<IBaseResponse>(`${this.api}?id=${id}`);
    }
}
