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
  IdDanhBa: number,
  IndexColumnHoTen : number,
  IndexColumnSoDienThoai: number,
}

export interface IVerifyImportCreateDanhBa {
  TenDanhBa: string,
  Mota: string,
  Type:number,
  File: File,
  IndexColumnHoTen : number,
  IndexColumnSoDienThoai: number,
  IndexRowStartImport: number,
  IndexRowHeader: number,
  SheetName: string,
}
export interface IImportCreateDanhBa extends IVerifyImportCreateDanhBa {}
export interface IUploadFileImportDanhBa extends IVerifyImportDanhBa {}

export interface IViewVerifyImportDanhBa {
  //fileFailed?: File,
  hasError?: boolean,
  fileKey?: string,
  data: ISoLuongLoi[],
  totalRowsImported: number,
  totalDataImported: number,
}
export interface IVerifyImportResponse {
    //isFile: boolean;
    data?: IViewVerifyImportDanhBa;
    //file?: Blob;
    fileName?: string;
}
export interface ISoLuongLoi{
  soLuongLoi?: number,
  nguyenNhanLoi?: string,
}
export interface IFileFailedImportCache{
  stream?: Blob,
  fileName?: string,
  contentType?: string,
}
export interface IFindPagingNguoiNhan extends IBaseRequestPaging {
  idDanhBa: number,
  items?: IListFieldIsHidden[],
}

export interface IListFieldIsHidden{
  idDanhBaTruongData: number
}

export type IViewRowNguoiNhan = {
  id?: number,
  //emailHuce?: string,
  hoVaTen?: string,
  //moTa?: string,
  //maSoNguoiDung?: string,
  //soLuongNguoiNhan: number,
  soDienThoai?: string,
  items?: IDataNguoiNhan[],
}

export interface IDataItem{
  id?: number,
  data?: string,
}

export interface IDataNguoiNhan{
  id?: number,
  tenTruong?: string,
  data:IDataItem,
}
export interface ICreateDanhBaChienDichQuick{
  tenDanhBa? : string, 
  type? : number,
  truong: string[],
  data:string[],
}
export interface ICreateDanhBaSmsQuick{
  indexTruongHoTen?: number,
  indexTruongSoDienThoai?: number,
  data:string[],
}
export interface IGetExcelInfor{
  File:File
}
export interface SheetInfoDto {
  sheetName: string;
  headers: string[];
}

export interface GetFileExcelInforResponseDto {
  sheets: SheetInfoDto[];
}
export interface TruongDataItem{
  id?: number,
  tenTruong?: string,
}
export interface GetTruongDataDanhBaSmsResponse{
  truongData: TruongDataItem[]
}

export interface IViewChiTietThueBaoNguoiNhan {
  items: IViewChiTietThueBaoNguoiNhanDataById[];
}

export interface IViewChiTietThueBaoNguoiNhanDataById {
  idTruong: number;
  tenTruong: string;
  idData: number;
  data: string;
}

export interface IUpdateDataChiTietThueBaoRequest {
  idDanhBa: number;
  idThueBao: number;
  items: IDataChiTietThueBao[];
}

export interface IDataChiTietThueBao {
  idData: number;
  data: string;
}

export interface IViewChiTietDanhBaSms {
  hoVaTen: string;
  soDienThoai: string;
}

export interface IUpdateDanhBaSmsRequest {
  idDanhBa: number;
  id: number;
  hoVaTen: string;
  soDienThoai: string;
}

export interface ICreateDanhBaFromTinNhanError {
  idChienDich: number,
  tenDanhBa: string,
  items: IListTinNhanError[], 
}

export interface IListTinNhanError {
  idDanhBa: number,
  idDanhBaSms: number,
}
