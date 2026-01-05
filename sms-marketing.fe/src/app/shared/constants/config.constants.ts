export class LoaiApiCreditStatuses {
    static STRINGEE = 1;
    static VIETTEL = 2;

    static getLabel(status: number): string {
        switch (status) {
            case LoaiApiCreditStatuses.STRINGEE:
                return 'Stringee';
            case LoaiApiCreditStatuses.VIETTEL:
                return 'Viettel';
            default:
                return 'Không xác định';
        }
    }

    static getSeverity(status: number): string {
        switch (status) {
            case LoaiApiCreditStatuses.STRINGEE:
                return 'info';
            case LoaiApiCreditStatuses.VIETTEL:
                return 'success';
            default:
                return 'secondary';
        }
    }
}