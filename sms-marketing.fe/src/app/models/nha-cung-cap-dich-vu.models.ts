import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingNhaCungCapDichVu extends IBaseRequestPaging {}


export interface ICreateBrandNameDto {
    tenBrandName: string;
    thoiGianBatDauHoatDong: string | null;
    thoiGianKetThucHoatDong: string | null;
}

export interface ICreateNhaCungCapDichVu {
    name: string;
    apiKey: string;
    apiSecret: string;
    baseUrl: string;
    isConfigAuthReq: boolean;
    brandNames?: ICreateBrandNameDto[];
}

export interface IUpdateBrandNameDto {
    idBrandName: number;
    tenBrandName: string;
    thoiGianBatDauHoatDong?: Date;
    thoiGianKetThucHoatDong?: Date;
}

export interface IUpdateNhaCungCapDichVu {
    id: number;
    name: string;
    apiKey: string;
    apiSecret: string;
    baseUrl: string;
    isConfigAuthReq: boolean;
    brandNames?: IUpdateBrandNameDto[];
}
export type IViewBrandNameDto = {
    id?: number;
    tenBrandName?: string;
}

export type IViewNhaCungCapDichVu = {
    id?: number;
    name?: string;
    apiKey?: string;
    apiSecret?: string;
    baseUrl?: string;
    isConfigAuthReq?: boolean;
    createdBy?: string;
    createdDate?: Date;
    modifiedBy?: string;
    modifiedDate?: Date;
    deletedDate?: Date;
    deleted?: boolean;
    deletedBy?: string;
    brandNames?: IViewBrandNameDto[];
}

export interface IViewNhaCungCapDichVuById {
    id: number;
    name: string;
    apiKey: string;
    apiSecret: string;
    baseUrl: string;
    isConfigAuthReq: boolean;
    brandNames: IViewBrandNameDto[];
}

export interface IDropDownNhaCungCapDichVu {
    id: number;
    name: string;
}

export interface IAddBrandNameToNhaCungCapDichVu {
    idNhaCungCapDichVu: number;
    tenBrandName: string;
    thoiGianBatDauHoatDong?: Date | null;
    thoiGianKetThucHoatDong?: Date | null;
}
export interface IAddUserToNhaCungCapDichVuDto {
    idNhaCungCapDichVu: number;
    idUser: string;
    idBrandName: number[];
    thoiGianBatDauSuDungDichVu?: Date | null;
    thoiGianKetThucSuDungDichVu?: Date | null;
}
export interface IDeleteBrandNameToNhaCungCapDichVuDto {
    idNhaCungCapDichVu: number;
    idBrandName: number;
}
export interface IFindPagingUserToNhaCungCapDichVuDto extends IBaseRequestPaging {
    idNhaCungCapDichVu: number;
}

export interface IGetListBrandNameResponseDto {
    id: number;
    tenBrandName: string;
    moTa: string;
}
export interface IUpdateUserToNhaCungCapDichVuDto {
    idUserNhaCungCapDichVu: number;
    thoiGianBatDauSuDungDichVu?: Date | null;
    thoiGianKetThucSuDungDichVu?: Date | null;
}
export type IViewUserNhaCungCapDto = {
    id?: number;
    user?: IViewUserNhaCungCapWithDetailsDto;
    nhaCungCapDichVu?: IDropDownNhaCungCapDichVu;
    brandName?: IViewBrandNameDto;
}

export interface IViewUserNhaCungCapWithDetailsDto {
    idUser: string;
    fullName: string;
    userName: string;
}

export interface IViewUserToNhaCungCapDichVuByIdDto {
    id: number;
    user: IViewUserNhaCungCapWithDetailsDto;
    nhaCungCapDichVu: IViewNhaCungCapDichVu;
    brandName: IViewBrandNameDto;
}

export interface IGetListDropDownUserNhaCungCapDichVuDto {
    idUser: string;
    fullName: string;
}