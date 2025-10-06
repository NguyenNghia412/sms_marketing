import { IViewGuestSvNhanBang } from '@/models/trao-bang/guest-sv-nhan-bang.models';
import { ICreateConfigSubPlan, IFindPagingConfigSubPlan, IUpdateConfigSubPlan, IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { IBaseResponse, IBaseResponsePaging, IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class GuestSvNhanBangService {
    api = '/api/core/trao-bang/sub-plan/sinh-vien-nhan-bang';
    http = inject(HttpClient);

    getByMssv(mssv: string) {
        return this.http.get<IBaseResponseWithData<IViewGuestSvNhanBang>>(`${this.api}/${mssv}`);
    }

    downloadQr(imageUrl: string) {
        this.http.get(imageUrl, { responseType: 'blob' }).subscribe((blob) => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'image.jpg'; // file name
            a.click();
            window.URL.revokeObjectURL(url);
        });
    }

}
