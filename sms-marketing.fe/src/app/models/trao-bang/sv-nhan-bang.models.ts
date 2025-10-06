import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewRowSvNhanBang {
    id?: number;
    idSubPlan?: number;
    tenSubPlan?: string;
    hoVaTen?: string;
    email?: string;
    emailSinhVien?: string;
    maSoSinhVien?: string;
    lop?: string;
    ngaySinh?: string; // ISO datetime string
    capBang?: string;
    tenNganhDaoTao?: string;
    xepHang?: string;
    thanhTich?: string;
    khoaQuanLy?: string;
    soQuyetDinhTotNghiep?: string;
    ngayQuyetDinh?: string; // ISO datetime string
    note?: string;
    isShow?: boolean;
    order?: string; // string in your data ("1")
    trangThai?: number;
    linkQR?: string;
}

export interface IFindPagingSvNhanBang extends IBaseRequestPaging {
    IdSubPlan?: number
}

export interface ICreateSvNhanBang {
  idSubPlan: number;
  hoVaTen: string;
  email: string;
  emailSinhVien: string;
  maSoSinhVien: string;
  lop: string;
  ngaySinh: string; // ISO datetime string
  capBang: string;
  tenNganhDaoTao: string;
  xepHang: string;
  thanhTich: string;
  khoaQuanLy: string;
  soQuyetDinhTotNghiep: string;
  ngayQuyetDinh: string; // ISO datetime string
  note: string;
  trangThai: number;
  linkQR: string;
}

export interface IUpdateSvNhanBang extends ICreateSvNhanBang {
    id: number;
}