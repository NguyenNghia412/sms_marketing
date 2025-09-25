export interface ISaveConfigChienDich {
    idChienDich: number;
    idDanhBa: number;
    idBrandName: number;
    isFlashSms: boolean;
    isAccented: boolean;
    noiDung: string;
}


export interface ISendSms {
  idChienDich: number;
  idDanhBa: number;
  idBrandName: number;
  isFlashSms: boolean;
  isAccented: boolean;
  noiDung: string;
}

export interface IPreviewSendSms {
  idChienDich: number;
  idDanhBa: number;
  idBrandName: number;
  isFlashSms: boolean;
  isAccented: boolean;
  noiDung: string;
  currentIndex: number;
}

export interface IViewPreviewSendSms {
  idDanhBaSms?: number;
  soDienThoai?: string;
  personalizedText?: string;
  smsCount?: number;
}
