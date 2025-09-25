import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IFindPagingDanhBa extends IBaseRequestPaging {}

export interface ICreateDanhBa {
  tenDanhBa: string,
  mota: string,
  ghiChu: string
  type: number,
}

export interface IUpdateDanhBa {
  id: number,
  tenDanhBa: string,
  mota: string,
  ghiChu: string,
  type: number,
}

export interface IViewRowDanhBa {
  id?: number,
  tenDanhBa?: string,
  mota?: string,
  ghiChu?: string,
  type?: number,
  truongData?: IViewCustonField[]
}

export interface IViewCustonField {
  id?: number
  tenTruong?: string,
}

export interface IViewDanhBa {
  id: number,
  idDanhBa: number,
  tenDanhBa: string,
  type: number,
}

export interface IVerifyImportDanhBa {
  File: File,
  IndexRowStartImport: number,
  IndexRowHeader: number,
  SheetName: string,
  IdDanhBa: number
}

export interface IUploadFileImportDanhBa extends IVerifyImportDanhBa {}

export interface IViewVerifyImportDanhBa {
  totalRowsImported: number,
  totalDataImported: number,
}

export interface IFindPagingNguoiNhan extends IBaseRequestPaging {
  idDanhBa: number,
}

export interface IViewRowNguoiNhan {
  id?: number,
  emailHuce?: string,
  hoVaTen?: string,
  maSoNguoiDung?: string,
  soDienThoai?: string,
}