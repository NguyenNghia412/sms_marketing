import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";
import { IViewDanhBa } from "./danh-ba.models";
export interface IFindPagingDanhBa extends IBaseRequestPaging {}

export interface IViewChienDichReport{
    id?: number
    tenChienDich?: string
    idDanhBa? : number
    danhBas?:IViewDanhBa[]
    smsSentSuccess?: number
    smsSentFailed?: number
    trangThai?: string
    createdDate?: string
}