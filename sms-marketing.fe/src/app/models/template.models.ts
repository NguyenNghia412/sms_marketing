import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewMauNoiDung {
  idMauNoiDung: number,
  noiDung: string,
}

export interface IViewRowMauNoiDung {
  idMauNoiDung: number,
  noiDung: string,
}

export interface ICreateMauNoiDung {
  noiDung: string,
}

export interface IUpdateMauNoiDung extends ICreateMauNoiDung {
  idMauNoiDung: number,
}

export interface IFindPagingMauNoiDung extends IBaseRequestPaging {}