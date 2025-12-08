import { IBaseRequestPaging } from '@/shared/models/request-paging.base.models';

export interface SMSTempalte {
    id: number;
    tenMauNoiDung: string;
    mauNoiDung: string;
}

export interface ICreateSMSTempalte {
    noiDung: string;
}

export interface IUpdateSMSTempalte extends ICreateSMSTempalte {
    id: number;
}

export interface IFindPagingSMSTempalte extends IBaseRequestPaging {}
