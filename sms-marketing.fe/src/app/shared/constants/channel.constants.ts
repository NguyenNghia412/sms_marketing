export class CampaginStatuses {
    static CHUA_GUI = 1;
    static DA_GUI = 2;
    static List = [
        { name: 'Chưa gửi', code: this.CHUA_GUI },
        { name: 'Đã gửi', code: this.DA_GUI },
    ];
    
}


export class DanhBaTypes {
    static SMS = 1;
    static EMAIL = 2;

    static List = [
        { name: 'SMS', code: this.SMS },
        { name: 'Email', code: this.EMAIL },
    ];
}
