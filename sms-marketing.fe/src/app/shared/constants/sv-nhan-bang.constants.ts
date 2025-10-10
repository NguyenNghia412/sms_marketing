import { environment } from "src/environments/environment";

export class SvNhanBangStatuses {
    static XEP_HANG = 1;
    static CHUAN_BI = 2;
    static DANG_TRAO_BANG = 3;
    static DA_TRAO_BANG = 4;
    static THAM_GIA_TRAO_BANG = 5;
    static VANG_MAT = 6;

    static List = [
        { name: 'Xếp hàng', code: this.XEP_HANG, severity: 'secondary' },
        { name: 'Chuẩn bị', code: this.CHUAN_BI, severity: 'info' },
        { name: 'Đang trao bằng', code: this.DANG_TRAO_BANG, severity: 'warn' },
        { name: 'Đã trao bằng', code: this.DA_TRAO_BANG, severity: 'success' },
        { name: 'Tham gia trao bằng', code: this.THAM_GIA_TRAO_BANG, severity: 'danger' },
        { name: 'Vắng mặt', code: this.VANG_MAT, severity: 'danger' }
    ];

    static getSeverity(code: number) {
        const found = this.List.find(x => x.code === code);
        return typeof found != 'undefined' ? found.severity : '';
    }

    static getName(code: number) {
        const found = this.List.find(x => x.code === code);
        return typeof found != 'undefined' ? found.name : '';
    }

}

export class SubPlanStatuses {
    static XEP_HANG = 1;
    static CHUAN_BI = 2;
    static DANG_TRAO_BANG = 3;
    static DA_TRAO_BANG = 4;
    static THAM_GIA_TRAO_BANG = 5;
    static VANG_MAT = 6;

    static List = [
        { name: 'Xếp hàng', code: this.XEP_HANG, severity: 'danger' },
        { name: 'Chuẩn bị', code: this.CHUAN_BI, severity: 'info' },
        { name: 'Đang trao bằng', code: this.DANG_TRAO_BANG, severity: 'warn' },
        { name: 'Đã trao bằng', code: this.DA_TRAO_BANG, severity: 'success' },
        { name: 'Tham gia trao bằng', code: this.THAM_GIA_TRAO_BANG, severity: 'contrast' },
        { name: 'Vắng mặt', code: this.VANG_MAT, severity: 'danger' }
    ];

    static getSeverity(code: number) {
        const found = this.List.find(x => x.code === code);
        return typeof found != 'undefined' ? found.severity : '';
    }

    static getName(code: number) {
        const found = this.List.find(x => x.code === code);
        return typeof found != 'undefined' ? found.name : '';
    }

}

export class TraoBangHubConst {
    static HUB = environment.baseUrl + '/hub/trao-bang'
    static ReceiveSinhVienDangTrao = 'ReceiveSinhVienDangTrao'
    static ReceiveChonKhoa = 'ReceiveChonKhoa'
    // static ReceiveChonKhoa = 'ReceiveChonKhoa'
    static ReceiveCheckIn = 'ReceiveCheckIn'
}