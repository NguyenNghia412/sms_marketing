import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingNhaMang extends IBaseRequestPaging {}

export interface ICreateNhaMang {
    tenNhaMang: string,
    prefix: string,
    idBrandName: number,
    donGia: number,
    thoiHan?: Date,
}

export interface IUpdateNhaMang {
    id: number,
    tenNhaMang: string,
    prefix: string,
    idBrandName: number,
    donGia: number,
    thoiHan?: Date,
}


export type IViewNhaMang ={
    id?: number,
    tenNhaMang?: string,
    prefix?: string,
    donGia?: IDonGia,
    brandName?: IBrandName,
}

export type IDonGia = {
    id? : number,
    //idBrandName: number,
    //idNhaMang: number,
    donGia?: number,
    thoiHan?: Date,
}

export type IBrandName = {
    id?: number,
    tenBrandName?: string,
}
