import { BaseComponent } from '@/shared/components/base/base-component';
import { Component } from '@angular/core';

@Component({
    selector: 'app-diem-danh-qr',
    imports: [],
    templateUrl: './diem-danh-qr.html',
    styleUrl: './diem-danh-qr.scss'
})
export class DiemDanhQr extends BaseComponent {
    dotDiemDanh: number = 0;

    override ngOnInit(): void {
        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.dotDiemDanh = Number(params.get('dot-diem-danh'));
            console.log(this.dotDiemDanh);
        });
    }

    onDiemDanh() {}
}
