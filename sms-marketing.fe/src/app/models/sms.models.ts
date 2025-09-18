import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models"

export type IViewChienDich = {
  id?: number
  tenChienDich?: string
  moTa?: string
  ngayBatDau?: string
  ngayKetThuc?: string
  noiDung?: string
  isFlashSms?: boolean
  createdDate?: string
}

export type ICreateChienDich = {
  tenChienDich: string
  moTa: string
  ngayBatDau: string
  ngayKetThuc: string
  noiDung: string
  isFlashSms: boolean
  createdDate: string
}

export type IUpdateChienDich = ICreateChienDich & {
  id: number
}

export interface IFindPagingChienDich extends IBaseRequestPaging {}

export interface IViewBrandname {

}