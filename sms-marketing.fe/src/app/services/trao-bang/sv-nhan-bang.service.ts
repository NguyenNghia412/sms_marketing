import { ICreateSvNhanBang, IFindPagingSvNhanBang, IGetTienDoHangDoi as IGetTienDoHangDoi, IUpdateSvNhanBang, IViewRowSvNhanBang, IViewScanQrCurrentSubPlan, IViewScanQrTienDoSv } from '@/models/trao-bang/sv-nhan-bang.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TraoBangSvService {
    api = '/api/core/trao-bang/sub-plan';
    http = inject(HttpClient);

    findPaging(query: IFindPagingSvNhanBang) {
        return this.http.get<IBaseResponsePaging<IViewRowSvNhanBang>>(`${this.api}/paging-danh-sach-sinh-vien-nhan-bang`, {
            params: { ...query }
        });
    }

    getById(id: string) {
        return this.http.get<IBaseResponseWithData<IViewRowSvNhanBang>>(`${this.api}/${id}`);
    }

    getList() {
        return this.http.get<IBaseResponseWithData<IViewRowSvNhanBang[]>>(`${this.api}/list`);
    }

    create(body: ICreateSvNhanBang) {
        return this.http.post<IBaseResponse>(`${this.api}/sinh-vien-nhan-bang`, body);
    }

    update(body: IUpdateSvNhanBang) {
        return this.http.put<IBaseResponse>(`${this.api}/sinh-vien-nhan-bang`, body);
    }
    
    delete(id: number) {
        return this.http.delete<IBaseResponse>(`${this.api}/${id}`);
    }
    
    pushHangDoi(mssv: string) {
        return this.http.post<IBaseResponseWithData<IViewScanQrTienDoSv>>(`${this.api}/sinh-vien-nhan-bang/hang-doi?mssv=${mssv}`, null);
    }
    
    getHangDoi(query: IGetTienDoHangDoi) {
        if (!query.SoLuong) {
            query.SoLuong = 5;
        }
        return this.http.get<IBaseResponseWithData<IViewScanQrTienDoSv[]>>(`${this.api}/sinh-vien-nhan-bang/tien-do`, {
            params: {...query}
        });
    }

    getCurrentSubPlanById(id: number) {
        return this.http.get<IBaseResponseWithData<IViewScanQrCurrentSubPlan>>(`${this.api}/${id}/thong-tin-subplan`);
    }

}
