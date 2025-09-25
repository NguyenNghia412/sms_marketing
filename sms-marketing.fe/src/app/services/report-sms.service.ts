import { IFindPagingChienDichReport, IFindPagingChiTietChienDichReport, IViewChienDichReport, IViewChiTietChienDichReport } from '@/models/report-sms.models';
import { IBaseResponse, IBaseResponseWithData, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ReportSmsService {
    api = '/api/core/report-sms';
    http = inject(HttpClient);
    findPaging(query: IFindPagingChienDichReport) {
            return this.http.get<IBaseResponsePaging<IViewChienDichReport>>(`${this.api}/chien-dich`, {
                params: { ...query }
            });
        }
      findPagingChiTietChienDich(idChienDich: number, idDanhBa: number, query: IFindPagingChiTietChienDichReport) {
            return this.http.get<IBaseResponsePaging<IViewChiTietChienDichReport>>(`${this.api}/chien-dich/${idChienDich}/danh-ba/${idDanhBa}`, {
                params: { ...query }
            });
        }
     
}