import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingUserCredits  extends IBaseRequestPaging {}
export interface IFindPagingUserCreditsForUser extends IBaseRequestPaging{}


export interface ICreateUserCredits {
    userId: string,
    hanMucCredit: string,
    thoiGianBatDauApDungHanMuc: Date,
    thoiGianKetThucApDungHanMuc?: Date,

}

export interface IUpdateUserCredits {
    id: number,
    userId: string,
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
    loaiApiCredit?: number | undefined,
    creditDaSuDung?: number,
    creditChuaSuDung?: number,
    creditConSauKhiKetThucThoiGianApDungHanMuc?: number,
    donVi? : string,

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