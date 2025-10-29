export class CampaginStatuses {
    static CHUA_GUI = 1;
    static DA_GUI = 2;
    static List = [
        { name: 'Chưa gửi', code: this.CHUA_GUI, severity: 'secondary' },
        { name: 'Đã gửi', code: this.DA_GUI, severity: 'success' },
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
