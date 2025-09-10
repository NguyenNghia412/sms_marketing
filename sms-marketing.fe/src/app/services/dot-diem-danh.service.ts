import { ICreateDotDiemDanh, IFindPagingDotDiemDanh, IViewRowDotDiemDanh } from '@/models/dot-diem-danh.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class DotDiemDanhService {
    api = '/api/core/hop-truc-tuyen/dot-diem-danh';
    http = inject(HttpClient);

    findPaging(query: IFindPagingDotDiemDanh) {
        return this.http.get<IBaseResponsePaging<IViewRowDotDiemDanh>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateDotDiemDanh) {
        return this.http.post<IBaseResponse>(this.api, body, {
            params: {
                idCuocHop: body.idCuocHop
            }
        });
    }

    delete(idCuocHop: number, idDotDiemDanh: number) {
      return this.http.delete<IBaseResponse>(`${this.api}?idCuocHop=${idCuocHop}&idDotDiemDanh=${idDotDiemDanh}`);
    }
}
