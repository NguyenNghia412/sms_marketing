import { IXacNhanDiemDanh } from '@/models/dot-diem-danh.models';
import { DotDiemDanhService } from '@/services/dot-diem-danh.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { Component, inject } from '@angular/core';
import { ButtonDirective } from 'primeng/button';
import { Toast } from "primeng/toast";

@Component({
    selector: 'app-diem-danh-qr',
    imports: [ButtonDirective, Toast],
    templateUrl: './diem-danh-qr.html',
    styleUrl: './diem-danh-qr.scss'
})
export class DiemDanhQr extends BaseComponent {
    _dotDiemDanhService = inject(DotDiemDanhService);

    dotDiemDanh: number = 0;

    override ngOnInit(): void {
        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.dotDiemDanh = Number(params.get('dot-diem-danh'));
        });
    }

    onDiemDanh() {
        const body: IXacNhanDiemDanh = {
            IdDotDiemDanh: this.dotDiemDanh,
        };
        this.loading = true;

        this._dotDiemDanhService
            .xacNhanDiemDanh(body)
            .subscribe(
                (res) => {
                    if (this.isResponseSucceed(res, true, 'Đã điểm danh')) {
                    }
                },
                (err) => {
                    this.messageError(err?.message);
                }
            )
            .add(() => {
                this.loading = false;
            });
    }
}
