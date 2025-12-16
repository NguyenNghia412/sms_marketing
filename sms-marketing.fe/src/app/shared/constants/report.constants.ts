export class ReportStatus {
    static Success = "Success";
    static Failed = "Failed";
    static Cancelled = "Cancelled";

    static List = [
        { name: 'Thành công', code: ReportStatus.Success },
        { name: 'Thất bại', code: ReportStatus.Failed },
        { name: 'Hủy', code: ReportStatus.Cancelled }
    ];
}


export class OrderByStatus{
    static ASC = "ASC";
    static DES = "DES";

    static List = [
        { name:'Hiển thị thống kê từ STT đầu', code: OrderByStatus.DES},
        { name:'Hiển thị thống kê từ STT cuối', code: OrderByStatus.ASC},
    ]
}