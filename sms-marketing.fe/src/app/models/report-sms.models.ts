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
    smsSentSuccess?: number
    smsSentFailed?: number
    trangThai?: string
    tongChiPhi?: number
    ngayGui?: string
}

export interface IBrandName {
    id?: number;
    tenBrandName?: string;
}

export interface ILogReport {
    price?: number;
    code?: number;
    message?: string;
    ngayGui?: string;
}

export interface IViewChiTietChienDichReport {
    id?: number;
    hoVaTen?: string;
    maSoNguoiDung?: string;
    soDienThoai?: string;
    brandName?: IBrandName;
    log?: ILogReport;
}

export interface IFindPagingChienDichReport extends IBaseRequestPaging {}

export interface IFindPagingChiTietChienDichReport extends IBaseRequestPaging{}
