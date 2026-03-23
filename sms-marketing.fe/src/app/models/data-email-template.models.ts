import { IBaseRequestPaging } from '@/shared/models/request-paging.base.models';

export interface EmailTempalte {
    id?: number;
    tenMauNoiDung?: String;
    thietKe?: any;
    createdDate?: Date;
}

export interface ICreateEmailTempalte {
    tenMauNoiDung?: String;
    thietKe?: string;
}

export interface IUpdateEmailTempalte extends ICreateEmailTempalte {
    id: number;
}

export interface IFindPagingEmailTempalte extends IBaseRequestPaging {}

export type JSONTemplate = {
    counters: Record<string, number>;
    body: {
        id: string | undefined;
        rows: any[];
        headers: any[];
        footers: any[];
        values: {};
    };
    schemaVersion?: number;
};
