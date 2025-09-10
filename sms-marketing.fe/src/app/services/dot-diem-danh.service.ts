import { ICreateDotDiemDanh, IFindPagingDotDiemDanh, IViewRowDotDiemDanh, IXacNhanDiemDanh } from '@/models/dot-diem-danh.models';
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

    downloadQrDiemDanh(idDotDiemDanh: number) {
        const uri = `${this.api}/${idDotDiemDanh}/qr-code/download`;

        return this.http
            .get(uri, {
                responseType: 'blob',
                observe: 'response'
            })
            .subscribe((response) => {
                // Extract filename
                const contentDisposition = response.headers.get('content-disposition');
                let fileName = 'download';

                if (contentDisposition) {
                    const matches = /filename\*?=(?:UTF-8''|)([^;]+)/i.exec(contentDisposition);
                    if (matches?.[1]) {
                        fileName = decodeURIComponent(matches[1].trim().replace(/['"]/g, ''));
                    }
                }

                // Create file blob
                const blob = new Blob([response.body!], { type: response.body!.type });

                // Create link and trigger download
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            });
    }

    generateQrDiemDanh(idDotDiemDanh: number) {
        const uri = `${this.api}/${idDotDiemDanh}/qr-code/`;

        return this.http.post<IBaseResponse>(uri, {});
    }

    xacNhanDiemDanh(body: IXacNhanDiemDanh) {
        const uri = `/api/core/hop-truc-tuyen/xac-nhan-diem-danh`;

        return this.http.post<IBaseResponse>(
            uri,
            {},
            {
                params: {
                    IdDotDiemDanh: body.IdDotDiemDanh,
                    EmailHuce: body.EmailHuce || ''
                }
            }
        );
    }
}
