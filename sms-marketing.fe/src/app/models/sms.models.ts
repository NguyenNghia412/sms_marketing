import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models"
import { IViewMauNoiDung } from "./template.models"
import { IViewDanhBa } from "./danh-ba.models"

export type IViewRowChienDich = {
  id?: number
  tenChienDich?: string
  moTa?: string
  ngayBatDau?: string
  ngayKetThuc?: string
  noiDung?: string
  mauNoiDung?: string
  idBrandName?: number
  idDanhBa?: number
  mauNoiDungs?: IViewMauNoiDung[]
  danhBas?: IViewDanhBa[]
  isFlashSms?: boolean
  trangThai?: boolean
  soLuongThueBao?: number
  soLuongSmsDaGuiThanhCong?: number
  soLuongSmsDaGuiThatBai ?: number
  createdDate?: string
}
export type IViewChienDichCreatedBy = {
  id?: number
  fullName?: string
}
export type IViewChienDich = {
  id?: number
  tenChienDich?: string
  moTa?: string
  ngayBatDau?: string
  ngayKetThuc?: string
  noiDung?: string
  idBrandName?: number
  idDanhBa?: number
  danhBas?: IViewDanhBa[]
  users?: IViewChienDichCreatedBy
  isFlashSms?: boolean
  isAccented?: boolean
  createdDate?: string
}

export type ICreateChienDich = {
  tenChienDich: string
  moTa: string
  ngayBatDau: string
  ngayKetThuc: string
  noiDung: string
  isFlashSms: boolean
  idBrandName: number
  idDanhBa: number
  createdDate: string
}

export type IUpdateChienDich = ICreateChienDich & {
  id: number
}

export interface IFindPagingChienDich extends IBaseRequestPaging {}

export interface IViewBrandname {

}