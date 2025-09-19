import { ICreateDanhBa, IFindPagingDanhBa, IFindPagingNguoiNhan, IUpdateDanhBa, IUploadFileImportDanhBa, IVerifyImportDanhBa, IViewRowDanhBa, IViewRowNguoiNhan, IViewVerifyImportDanhBa } from '@/models/danh-ba.models';
import { IBaseResponse, IBaseResponseWithData, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class DanhBaService {
    api = '/api/core/danh-ba';
    http = inject(HttpClient);

    getList() {
        return this.http.get<IBaseResponseWithData<IViewRowDanhBa[]>>(`${this.api}/list-danh-ba`);
    }

    findPaging(query: IFindPagingDanhBa) {
        return this.http.get<IBaseResponsePaging<IViewRowDanhBa>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateDanhBa) {
        return this.http.post<IBaseResponse>(this.api, body);
    }

    update(body: IUpdateDanhBa) {
        return this.http.put<IBaseResponse>(`${this.api}?idDanhBa=${body.id}`, body);
    }

    delete(id: number) {
        return this.http.delete<IBaseResponse>(`${this.api}?id=${id}`);
    }

    downloadImportTemplate() {
        return this.http
            .post(`${this.api}/export-danh-ba-chi-tiet-template-excel`, null, {
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

    verifyFileImport(body: IVerifyImportDanhBa) {
        const uri = `${this.api}/verify-import-danh-ba-chien-dich`;

        const formData = new FormData();
        formData.append('File', body.File, body.File.name);
        formData.append('IndexRowStartImport', body.IndexRowStartImport.toString());
        formData.append('IndexRowHeader', body.IndexRowHeader.toString());
        formData.append('SheetName', body.SheetName);
        formData.append('IdDanhBa', body.IdDanhBa.toString());

        return this.http.post<IBaseResponseWithData<IViewVerifyImportDanhBa>>(uri, formData);
    }

    uploadFileImport(body: IUploadFileImportDanhBa) {
        const uri = `${this.api}/import-danh-ba-chien-dich-append`;

        const formData = new FormData();
        formData.append('File', body.File, body.File.name);
        formData.append('IndexRowStartImport', body.IndexRowStartImport.toString());
        formData.append('IndexRowHeader', body.IndexRowHeader.toString());
        formData.append('SheetName', body.SheetName);
        formData.append('IdDanhBa', body.IdDanhBa.toString());

        return this.http.post<IBaseResponse>(uri, formData);
    }

    findPagingNguoiNhan(query: IFindPagingNguoiNhan) {
        const uri = `${this.api}/paging-danh-ba-chi-tiet`;

        return this.http.get<IBaseResponsePaging<IViewRowNguoiNhan>>(uri, {
            params: { ...query }
        });
    }
}
