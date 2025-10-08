import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewRowSvNhanBang {
    id?: number;
    idSubPlan?: number;
    tenSubPlan?: string;
    hoVaTen?: string;
    email?: string;
    emailSinhVien?: string;
    maSoSinhVien?: string;
    lop?: string;
    ngaySinh?: string; // ISO datetime string
    capBang?: string;
    tenNganhDaoTao?: string;
    xepHang?: string;
    thanhTich?: string;
    khoaQuanLy?: string;
    soQuyetDinhTotNghiep?: string;
    ngayQuyetDinh?: string; // ISO datetime string
    note?: string;
    isShow?: boolean;
    order?: string; // string in your data ("1")
    trangThai?: number;
    linkQR?: string;
}

export interface IFindPagingSvNhanBang extends IBaseRequestPaging {
    IdSubPlan?: number
}

export interface ICreateSvNhanBang {
    idSubPlan: number;
    hoVaTen: string;
    email: string;
    emailSinhVien: string;
    maSoSinhVien: string;
    lop: string;
    ngaySinh: string; // ISO datetime string
    capBang: string;
    tenNganhDaoTao: string;
    xepHang: string;
    thanhTich: string;
    khoaQuanLy: string;
    soQuyetDinhTotNghiep: string;
    ngayQuyetDinh: string; // ISO datetime string
    note: string;
    trangThai: number;
    linkQR: string;
}

export interface IUpdateSvNhanBang extends ICreateSvNhanBang {
    id: number;
}

export interface IGetTienDoHangDoi {
    // IdSubPlan: number;
    SoLuong?: number;
}

export interface IViewScanQrCurrentSubPlan {
    soLuongConLai?: number
    soLuongDaTrao?: number
    soLuongThamGia?: number
    soLuongVangMat?: number
    ten?: string
}

export interface IViewScanQrTienDoSv {
    id?: number;
    hoVaTen?: string;
    maSoSinhVien?: string;
    isShow?: boolean;
    order?: number;
    trangThai?: number;
    tenKhoa?: string;
}

export interface IViewScanQrSubPlan {
    id?: number;
    ten?: string;
    tienDo?: string;
    trangThai?: number;
    order?: number;
}

export interface IViewSvDangTraoBang {
    id?: number;
    hoVaTen?: string;
    capBang?: string;
    tenNganhDaoTao?: string;
    thanhTich?: string;
    xepHang?: string;
    maSoSinhVien?: string;
    tenSubPlan?: string;
    note?: string;
    text?: string;
}

export interface IViewTienDoTraoBang {
    tienDo: string
}

export interface IViewSubPlanSideScreen {
    id?: number
    ten?: string
    tienDo?: string
    items?: IViewScanQrTienDoSv[]
}