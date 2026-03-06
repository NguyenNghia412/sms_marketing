export interface IGetStatisticsDashBoard {
	tongSoLuongTinNhanGuiThanhCong: number,
	tongSoUserSuDungDichVu: number,
	tongSoChienDichGuiTinNhan: number,
	tongSoNhaCungCapDichVu: number,
}

export interface IGetStatisticsTongSoTinNhanDaGuiTheoNam {
	listThongKeTongSoTinNhanDaGui?: IGetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser[],
}

export interface IGetStatisticsTongSoTinNhanDaGuiTheoNamTheoUser {
	user?: IUser,
	tongSoTinNhanDaGuiThanhCong?: number,
	tongSoTinNhanDaGuiThatBai?: number,
	tongSoTinNhanDaGui?: number,
}

export interface IGetStatisticsUserCreditsTheoNam {
	listUserCredits?: IGetStatisticsUserCreditsTheoNamByUser[],
}

export interface IGetStatisticsUserCreditsTheoNamByUser {
	user?: IUser,
	creditDaSuDung?: string,
	donVi?: string,
}

export type IUser = {
	userId?: string,
	fullName?: string,
}

export interface IGetStatisticsUserCreditsTheoThangByUser{
	userCreditsTheoThangByUsers?: IGetStatisticsUserCreditsByUser[],
}

export interface IGetStatisticsUserCreditsByUser{
	tuNgay : Date,
	denNgay: Date,
	user?: IUser,
	creditDaSuDung?: string,
	hanMucCredit?: string,
	donVi?: string,
}
