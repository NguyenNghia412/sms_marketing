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