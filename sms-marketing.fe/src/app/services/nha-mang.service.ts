import { ICreateNhaMang, IFindPagingNhaMang, IUpdateNhaMang } from "@/models/nha-mang.models";
import { IBaseResponseWithData } from "@/shared/models/request-paging.base.models";
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class NhaMangService {
    api = '/api/core/nha-mang';
    http = inject(HttpClient);

    findPaging(query: IFindPagingNhaMang){
        const params: any = {
            pageNumber: query.pageNumber,
            pageSize: query.pageSize,
            keyword: query.keyword || ''
        };

        return this.http.get<IBaseResponseWithData<any>>(`${this.api}`, {
            params
        });
    }

    createNhaMang(body: ICreateNhaMang)
    {
        return this.http.post<IBaseResponseWithData<any>>(`${this.api}`, body);
    }

    updateNhaMang( body: IUpdateNhaMang)
    {
        return this.http.put<IBaseResponseWithData<any>>(`${this.api}`, body);
    }

    deleteNhaMang(id: number)
    {
        return this.http.delete<IBaseResponseWithData<any>>(`${this.api}/${id}`);
    }

    getById(id: number)
    {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/by-id/${id}`);
    }
}