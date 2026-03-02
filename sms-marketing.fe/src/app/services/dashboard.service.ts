import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { IGetStatisticsDashBoard, IGetStatisticsTongSoTinNhanDaGuiTheoNam, IGetStatisticsUserCreditsTheoNam, IGetStatisticsUserCreditsTheoThangByUser } from "@/models/dash-board.models";
import { IBaseResponseWithData } from "@/shared/models/request-paging.base.models";

@Injectable({
    providedIn: 'root'
})
export class DashBoardService {
    api = '/api/core/dash-board';
    http = inject(HttpClient);

    getStatisticsDashBoard(){
        return this.http.get<IBaseResponseWithData<IGetStatisticsDashBoard>>(`${this.api}/thong-ke-tong-quan`);
    }

    getStatisticsTongSoTinNhanDaGuiTheoNam(tuNgay: Date, denNgay: Date){
        const params: any = {
            tuNgay: tuNgay.toISOString(),
            denNgay: denNgay.toISOString()
        };

        return this.http.get<IBaseResponseWithData<IGetStatisticsTongSoTinNhanDaGuiTheoNam>>(
            `${this.api}/thong-ke-tong-so-tin-nhan-da-gui-theo-nam`,
            { params }
        );
    }

    getStatisticsUserCreditsTheoNam(tuNgay: Date, denNgay: Date){
        const params: any = {
            tuNgay: tuNgay.toISOString(),
            denNgay: denNgay.toISOString()
        };

        return this.http.get<IBaseResponseWithData<IGetStatisticsUserCreditsTheoNam>>(
            `${this.api}/thong-ke-credits-theo-nam`,
            { params }
        );
    }

    getStatisticsUserCreditsTheoThangByUser(userId: string, nam: number){
        const params: any = { userId, nam };
        return this.http.get<IBaseResponseWithData<IGetStatisticsUserCreditsTheoThangByUser>>(
            `${this.api}/thong-ke-credits-theo-thang-by-user`,
            { params }
        );
    }
}