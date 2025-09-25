import { IPreviewSendSms, IViewPreviewSendSms } from '@/models/gui-tin-nhan.models';
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
    noiDung = input.required<string | null | undefined>();
    isFlashSms = input.required<boolean>();
    isAccented = input.required<boolean>();

    private _guiTinNhanService = inject(GuiTinNhanService);

    private debounceTimer: any;
    currentIndex: number = 1;
    previewSms: IViewPreviewSendSms = {};

    constructor() {
        super();
        effect(() => {
            clearTimeout(this.debounceTimer);

            if (this.idBrandName() && this.idDanhBa() && this.noiDung()) {
                this.debounceTimer = setTimeout(() => {
                    this.onPreviewSendSms();
                }, 500); // debounce 0.5s
            } else {
                this.previewSms = {};
            }
        });
    }

    onPreviewSendSms() {
        const body: IPreviewSendSms = {
            idChienDich: this.idChienDich(),
            idBrandName: this.idBrandName() ?? 0,
            idDanhBa: this.idDanhBa() ?? 0,
            currentIndex: this.currentIndex,
            isAccented: this.isAccented(),
            isFlashSms: this.isFlashSms(),
            noiDung: this.noiDung() || ''
        };

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
