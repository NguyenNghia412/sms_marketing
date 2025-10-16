export interface ISaveConfigChienDich {
    idChienDich: number;
    idDanhBa?: number | null;
    idBrandName: number;
    isFlashSms?: boolean;
    isAccented: boolean;
    noiDung: string;
}

export interface ISendSms {
    idChienDich: number;
    idDanhBa?: number | null;
    danhSachSoDienThoai?: IListSoDienThoai[];
    idBrandName?: number;
    isFlashSms?: boolean;
    isAccented: boolean;
    noiDung?: string;
}

export interface IPreviewSendSms {
    idChienDich: number;
    idDanhBa?: number | null;
    danhSachSoDienThoai?: IListSoDienThoai[];
    idBrandName: number;
    isFlashSms?: boolean;
    isAccented: boolean;
    noiDung: string;
    currentIndex: number;
}

export interface IVerifySendSms {
    idChienDich: number;
    idDanhBa?: number | null;
    danhSachSoDienThoai?: IListSoDienThoai[];
    idBrandName: number;
    isFlashSms?: boolean;
    isAccented: boolean;
    noiDung: string;
}

export interface IListSoDienThoai {
    soDienThoai: string;
}

export interface IViewPreviewSendSms {
    idDanhBaSms?: number | null;
    brandName?: string;
    soDienThoai?: string;
    personalizedText?: string;
    smsCount?: number;
}

export interface IViewVerifySendSms {
    soLuongNguoiNhan?: number
    tongSoLuongTinNhan?: number
}