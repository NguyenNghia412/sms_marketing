import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingDotDiemDanh extends IBaseRequestPaging {
  idCuocHop: number
}

export type ICreateDotDiemDanh = {
  idCuocHop: number
  tenDotDiemDanh: string;
  tenMonHoc: string;
  maMonHoc: string;
  thoiGianBatDau: string;   // ISO date string
  thoiGianKetThuc: string;  // ISO date string
  ghiChu: string;
}

export type IViewRowDotDiemDanh = {
  id?: number
  tenDotDiemDanh?: string;
  tenMonHoc?: string;
  maMonHoc?: string;
  thoiGianBatDau?: string;   // ISO date string
  thoiGianKetThuc?: string;  // ISO date string
  ghiChu?: string;
}