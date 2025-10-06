import { ICreateConfigSubPlan, IFindPagingConfigSubPlan, IUpdateConfigSubPlan, IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TraoBangSubPlanService {
    api = '/api/core/trao-bang/sub-plan';
    http = inject(HttpClient);

    findPaging(query: IFindPagingConfigSubPlan) {
        return this.http.get<IBaseResponsePaging<IViewRowConfigSubPlan>>(this.api, {
            params: { ...query }
        });
    }

    getById(id: string) {
        return this.http.get<IBaseResponseWithData<IViewRowConfigSubPlan>>(`${this.api}/${id}`);
    }

    getList(idPlan: number) {
        return this.http.get<IBaseResponseWithData<IViewRowConfigSubPlan[]>>(`${this.api}/plan/${idPlan}/list`);
    }

    create(body: ICreateConfigSubPlan) {
        return this.http.post<IBaseResponse>(`${this.api}/${body.idPlan}`, body);
    }

    update(body: IUpdateConfigSubPlan) {
        return this.http.put<IBaseResponse>(`${this.api}`, body);
    }

    delete(id: number, idPlan: number) {
        return this.http.delete<IBaseResponse>(`${this.api}/${id}/plan/${idPlan}`);
    }

}
