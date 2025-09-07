export interface IBaseRequestPaging {
  pageSize: number
  pageNumber: number
  keyword?: string
}

export type IBaseResponse = {
  status: number
  code: number
  message: string
}

export type IBaseResponsePaging<T> = IBaseResponse & {
  data: T
}