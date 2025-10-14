import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";
import { IViewDanhBa } from "./danh-ba.models";
export interface IFindPagingDanhBa extends IBaseRequestPaging {}

export interface IViewChienDichReport{
    idChienDich?: number
    tenChienDich?: string
    idDanhBa? : number
    tenDanhBa? :string
    danhBa?: {
        idDanhBa?: number;
        tenDanhBa?: string;
    };
    tongSoSms?: number
    smsSentSuccess?: number
    smsSentFailed?: number
    noiDung?: string
    trangThai?: string
    tongChiPhi?: number
    ngayGui?: string
}

export interface IBrandName {
    id?: number;
    tenBrandName?: string;
}

export interface ILogReport {
    soDienThoai?: string;
    noiDungChiTiet?: string;
    price?: number;
    code?: number;
    message?: string;
    ngayGui?: string;
}

export interface IViewChiTietChienDichReport {
    id?: number;
    hoVaTen?: string;
    //maSoNguoiDung?: string;
    soDienThoai?: string;
    brandName?: IBrandName;
    log?: ILogReport;
}
export interface IExportThongKeTheoChienDich{
    idChienDichs?: number[];
}
export interface IExportThongKeTheoThang{
    thang?: number;
    nam?: number;
}
export interface IFindPagingChienDichReport extends IBaseRequestPaging {}

export interface IFindPagingChiTietChienDichReport extends IBaseRequestPaging{
    idDanhBa?: number;
}

