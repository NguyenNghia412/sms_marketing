export class CampaginStatuses {
    static CHUA_GUI = 0;
    static DA_GUI = 1;
    static DANG_GUI = 2;
    static LEN_LICH = 3;
    static HUY = 4;
    static List = [
        { name: 'Chưa gửi', code: this.CHUA_GUI, severity: 'secondary' },
        { name: 'Đã gửi', code: this.DA_GUI, severity: 'success' },
        { name: 'Đang gửi', code: this.DANG_GUI, severity:'warn'},
        { name: 'Lên lịch', code: this.LEN_LICH, severity:'infor'},
        { name: 'Hủy', code: this.HUY, severity: 'danger'},
    ];

    static getSeverityByCode(code: number): string {
        const status = this.List.find(s => s.code === code);
        return status ? status.severity : 'default';
    }
    
}


export class DanhBaTypes {
    static SMS = 1;
    static EMAIL = 2;

    static List = [
        { name: 'SMS', code: this.SMS },
        { name: 'Email', code: this.EMAIL },
    ];
}
