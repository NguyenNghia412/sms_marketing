import { ICreateConfigPlan, IFindPagingConfigPlan, IUpdateConfigPlan, IViewRowConfigPlan } from '@/models/trao-bang/plan.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TraoBangPlanService {
    api = '/api/core/trao-bang/plan';
    http = inject(HttpClient);

    findPaging(query: IFindPagingConfigPlan) {
        return this.http.get<IBaseResponsePaging<IViewRowConfigPlan>>(this.api, {
            params: { ...query }
        });
    }

    getById(id: string) {
        return this.http.get<IBaseResponseWithData<IViewRowConfigPlan>>(`${this.api}/${id}`);
    }

    getList() {
        return this.http.get<IBaseResponseWithData<IViewRowConfigPlan[]>>(`${this.api}/list`);
    }

    create(body: ICreateConfigPlan) {
        return this.http.post<IBaseResponse>(`${this.api}`, body);
    }

    update(body: IUpdateConfigPlan) {
        return this.http.put<IBaseResponse>(`${this.api}/${body.id}`, body);
    }
    
    delete(id: number) {
        return this.http.delete<IBaseResponse>(`${this.api}/${id}`);
    }

}
