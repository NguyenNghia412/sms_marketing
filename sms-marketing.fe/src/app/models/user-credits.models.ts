import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingUserCredits  extends IBaseRequestPaging {}
export interface IFindPagingUserCreditsForUser extends IBaseRequestPaging{
    userId?: string;
}


export interface ICreateUserCredits {
    userId: string,
    idNhaCungCapDichVu: number,
    hanMucCredit: string,
    thoiGianBatDauApDungHanMuc: Date,
    thoiGianKetThucApDungHanMuc?: Date,

}

export interface IUpdateUserCredits {
    id: number,
    //userId: string,
    //idNhaCungCapDichVu: number,
    hanMucCredit: string,
    thoiGianBatDauApDungHanMuc: Date,
    thoiGianKetThucApDungHanMuc?: Date,
}

export type IViewUserCredits = {
    id?: number,
    user?: IUser,
    hanMucCredit?: string,
    thoiGianBatDauApDungHanMuc?: Date,
    thoiGianKetThucApDungHanMuc?: Date,
    nhaCungCapDichVus?: IViewNhaCungCapDichVuByUserCredits[],
    creditDaSuDung?: number,
    creditChuaSuDung?: number,
    creditConSauKhiKetThucThoiGianApDungHanMuc?: number,
    donVi? : string,
    _tenNhaCungCapDichVu?: string,
    _tenBrandName?: string,
}

export type IUser = {
    userId?: string,
    userName?: string,
    email?: string,
    fullName?: string,
}

export interface IGetDonVi{
    id?: number,
    donVi?: string,
}

export type IViewNhaCungCapDichVuByUserCredits = {
    idNhaCungCapDichVu: number,
    tenNhaCungCapDichVu: string,
    brandNames?: IBrandName[],
}

export type IBrandName = {
    id?: number,
    tenBrandName?: string,
}

export interface IViewUserCreditsByUser {
    userCredit: IViewUserCreditDto;
    hanMucCredit: string;
    creditDaSuDung?: string;
    creditChuaSuDung?: string;
    donVi: string;
}

export interface IViewUserCreditDto {
    userId: string;
    userName: string;
    fullName: string;
    email: string;
}

export interface IUpdateToiDaHanMucCreditsGiaHan {
    id: number;
    toiDaHanMucCreditGiaHan: string;
    donVi: string;
}