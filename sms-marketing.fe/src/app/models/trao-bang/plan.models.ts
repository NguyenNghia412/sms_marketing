import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewRowConfigPlan {
    id?: number;
    ten?: string;
    moTa?: string;
    thoiGianBatDau?: string;
    thoiGianKetThuc?: string;
    createdDate?: string;
}

export interface IFindPagingConfigPlan extends IBaseRequestPaging {}

export interface ICreateConfigPlan {
    ten: string;
    moTa?: string;
    thoiGianBatDau?: string;
    thoiGianKetThuc?: string;
}

export interface IUpdateConfigPlan extends ICreateConfigPlan {
    id: number
}