import { ICreateChienDich, IFindPagingChienDich, IUpdateChienDich, IViewBrandname, IViewChienDich } from '@/models/sms.models';
import { IBaseResponse, IBaseResponseList, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ChienDichService {
    api = '/api/core/chien-dich';
    http = inject(HttpClient);

    findPaging(query: IFindPagingChienDich) {
        return this.http.get<IBaseResponsePaging<IViewChienDich>>(this.api, {
            params: { ...query }
        });
    }

    getListBrandname() {
        return this.http.get<IBaseResponseList<IViewBrandname[]>>(`${this.api}/list-brand-name`);
    }

    create(body: ICreateChienDich) {
        return this.http.post<IBaseResponse>(this.api, body);
    }

    update(body: IUpdateChienDich) {
        return this.http.post<IBaseResponse>(`${this.api}?idChienDich=${body.id}`, body);
    }

    delete(id: number) {
        return this.http.delete<IBaseResponse>(`${this.api}?idChienDich=${id}`);
    }
}
