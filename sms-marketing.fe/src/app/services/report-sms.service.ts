import { IExportThongKeTheoChienDich, IExportThongKeTheoThang, IFindPagingChienDichReport, IFindPagingChiTietChienDichReport, IViewChienDichReport, IViewChiTietChienDichReport } from '@/models/report-sms.models';
import { IBaseResponse, IBaseResponseWithData, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

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
            return this.http.get<IBaseResponsePaging<IViewChiTietChienDichReport>>(`${this.api}/chien-dich/${idChienDich}`, {
                params: { ...query, idDanhBa: idDanhBa }
            });
        }
    exportThongKeTheoChienDich(dto:  IExportThongKeTheoChienDich): Observable<Blob> {
        return this.http.post(`${this.api}/export-theo-chien-dich`, dto, {
            responseType: 'blob'
        });
        }
    exportThongKeTheoThang(dto: IExportThongKeTheoThang): Observable<Blob> {
        return this.http.post(`${this.api}/export-theo-thang`, dto, {
            responseType: 'blob'
        });
        }

    downloadThongKeTheoChienDich(body: IExportThongKeTheoChienDich): Observable<void> {
        return this.http.post(`${this.api}/export-thong-ke-theo-chien-dich-excel`, body, {
            responseType: 'blob',
            observe: 'response'
        }).pipe(
            tap((response: any) => {
                const contentDisposition = response.headers.get('content-disposition');
                let fileName = 'thong-ke-chien-dich.xlsx';

                if (contentDisposition) {
                    const matches = /filename\*?=(?:UTF-8''|)([^;]+)/i.exec(contentDisposition);
                    if (matches?.[1]) {
                        fileName = decodeURIComponent(matches[1].trim().replace(/['"]/g, ''));
                    }
                }

                const blob = new Blob([response.body], { type: response.body.type });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            })
        ) as Observable<void>;
    }
    downloadThongKeTheoThang(body: IExportThongKeTheoThang): Observable<void> {
        return this.http.post(`${this.api}/export-thong-ke-theo-thang-excel`, body, {
            responseType: 'blob',
            observe: 'response'
        }).pipe(
            tap((response: any) => {
                const contentDisposition = response.headers.get('content-disposition');
                let fileName = 'thong-ke-thang.xlsx';

                if (contentDisposition) {
                    const matches = /filename\*?=(?:UTF-8''|)([^;]+)/i.exec(contentDisposition);
                    if (matches?.[1]) {
                        fileName = decodeURIComponent(matches[1].trim().replace(/['"]/g, ''));
                    }
                }

                const blob = new Blob([response.body], { type: response.body.type });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            })
        ) as Observable<void>;
    }
}
