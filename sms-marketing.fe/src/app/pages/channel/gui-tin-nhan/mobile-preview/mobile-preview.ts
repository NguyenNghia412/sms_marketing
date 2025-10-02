import { IListSoDienThoai, IPreviewSendSms, IViewPreviewSendSms } from '@/models/gui-tin-nhan.models';
import { GuiTinNhanService } from '@/services/gui-tin-nhan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, effect, inject, input } from '@angular/core';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';

@Component({
    selector: 'app-mobile-preview',
    imports: [SharedImports, InputGroupModule, InputGroupAddonModule],
    templateUrl: './mobile-preview.html',
    styleUrl: './mobile-preview.scss'
})
export class MobilePreview extends BaseComponent {
    idChienDich = input.required<number>();
    idBrandName = input.required<number | null | undefined>();
    idDanhBa = input.required<number | null | undefined>();
    soDienThoai = input.required<string | null | undefined>();
    nguoiNhanType = input.required<'danhBa' | 'soDienThoai'>();
    noiDung = input.required<string | null | undefined>();
    isAccented = input.required<boolean>();

    private _guiTinNhanService = inject(GuiTinNhanService);

    private debounceTimer: any;
    currentIndex: number = 1;
    previewSms: IViewPreviewSendSms = {};

    constructor() {
        super();
        effect(() => {
            this.noiDung();
            
            clearTimeout(this.debounceTimer);

            if (this.nguoiNhanType() === 'danhBa') {
                if (this.idBrandName() && this.idDanhBa() && this.noiDung()) {
                    this.currentIndex = 1;
                    this.debounceTimer = setTimeout(() => {
                        this.onPreviewSendSms();
                    }, 500);
                } else {
                    this.previewSms = {};
                }
            } else {
                if (this.idBrandName() && this.soDienThoai() && this.noiDung()) {
                    this.currentIndex = 1;
                    this.debounceTimer = setTimeout(() => {
                        this.onPreviewSendSms();
                    }, 2000);
                } else {
                    this.previewSms = {};
                }
            }
        });
    }

    onPreviewSendSms() {
        const body: IPreviewSendSms = {
            idChienDich: this.idChienDich(),
            idBrandName: this.idBrandName() ?? 0,
            currentIndex: this.currentIndex,
            isAccented: this.isAccented(),
            noiDung: this.noiDung() || ''
        };

        if (this.nguoiNhanType() === 'danhBa') {
            body.idDanhBa = this.idDanhBa() ?? undefined;
        } else {
            const soDienThoaiText = this.soDienThoai() || '';
            const phoneNumbers = soDienThoaiText
                .split(/[\n,;\s]+/)
                .map((s: string) => s.trim())
                .filter((s: string) => s.length > 0);
            
            body.danhSachSoDienThoai = phoneNumbers.map((sdt: string) => ({ soDienThoai: sdt }));
        }

        this.loading = true;
        this._guiTinNhanService.previewSendSms(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res)) {
                    this.previewSms = res.data;
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    onChangeIndex(step: number) {
        if (this.currentIndex + step > 0) {
            this.currentIndex += step;
            this.onPreviewSendSms();
        }
    }

    ngOnDestroy() {
        clearTimeout(this.debounceTimer);
    }
}