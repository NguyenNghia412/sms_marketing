import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingHopTrucTuyen extends IBaseRequestPaging {}

export type ICreateHopTrucTuyen = {
  tenCuocHop: string,
  moTa?: string,
  linkCuocHop?: string,
  thoiGianBatDau: string,
  thoiGianKetThuc: string
}

export type IViewRowHopTrucTuyen = {
  id?: number
  tenCuocHop?: string,
  linkCuocHop?: string,
  moTa?: string,
  thoiGianBatDau?: string,
  thoiGianKetThuc?: string
}