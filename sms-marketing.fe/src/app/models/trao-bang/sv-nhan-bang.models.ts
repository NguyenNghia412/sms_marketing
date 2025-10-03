import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewRowSvNhanBang {
    id?: number;
}

export interface IFindPagingSvNhanBang extends IBaseRequestPaging {}

export interface ICreateSvNhanBang {

}

export interface IUpdateSvNhanBang extends ICreateSvNhanBang {
    id: number;
}