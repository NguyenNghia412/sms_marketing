import { ICreateConfigPlan, IFindPagingConfigPlan, IUpdateConfigPlan, IViewRowConfigPlan } from '@/models/trao-bang/plan.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class SystemTraoBangService {
    api = '/api/core/trao-bang/sub-plan';
    http = inject(HttpClient);

    restart() {
        return this.http.post<IBaseResponse>(`${this.api}/restart`, null);
    }

}
