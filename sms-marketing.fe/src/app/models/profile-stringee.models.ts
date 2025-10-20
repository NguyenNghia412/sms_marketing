
export interface IResponseProfileStringee {
    code?: number
    data?: IAccountProfileStringee
}
export interface IAccountProfileStringee {
    id?: number
    firstName?: string
    lastName?: string
    email?: string
    phoneNumber?: string
    countryNumber?: string
    amount?: number
}