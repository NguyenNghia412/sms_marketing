import { IViewRowDanhBa } from '@/models/danh-ba.models';
import { ISaveConfigChienDich, ISendSms } from '@/models/gui-tin-nhan.models';
import { IViewBrandname } from '@/models/sms.models';
import { ChienDichService } from '@/services/chien-dich.service';
import { DanhBaService } from '@/services/danh-ba.service';
import { GuiTinNhanService } from '@/services/gui-tin-nhan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { Breadcrumb } from '@/shared/components/breadcrumb/breadcrumb';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MenuItem } from 'primeng/api';
import { MobilePreview } from './mobile-preview/mobile-preview';

@Component({
    selector: 'app-gui-tin-nhan',
    imports: [SharedImports, Breadcrumb, MobilePreview],
    templateUrl: './gui-tin-nhan.html',
    styleUrl: './gui-tin-nhan.scss'
})
export class GuiTinNhan extends BaseComponent {
    private _danhBaService = inject(DanhBaService);
    private _chienDichService = inject(ChienDichService);
    private _guiTinNhanService = inject(GuiTinNhanService);

    items: MenuItem[] = [{ label: 'Danh sách chiến dịch', routerLink: '/channel/sms' }, { label: 'Gửi tin nhắn sms' }];
    home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };
    idChienDich: number = 0;
    listDanhBa: IViewRowDanhBa[] = [];
    listBrandname: IViewBrandname[] = [];

    override form: FormGroup = new FormGroup({
        idBrandName: new FormControl(null, [Validators.required]),
        idDanhBa: new FormControl(null, [Validators.required]),
        tenChienDich: new FormControl('', [Validators.required]),
        ngayBatDau: new FormControl(new Date()),
        ngayKetThuc: new FormControl(new Date()),
        noiDung: new FormControl('', [Validators.required]),
        isFlashSms: new FormControl(true),
        isAccented: new FormControl(true)
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenChienDich: {
            required: 'Không được bỏ trống'
        },
        idDanhBa: {
            required: 'Không được bỏ trống'
        },
        idBrandName: {
            required: 'Không được bỏ trống'
        },
        noiDung: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
        this._danhBaService.getList().subscribe({
            next: (value) => {
                this.listDanhBa = value.data;
            }
        });
        this._chienDichService.getListBrandname().subscribe({
            next: (value) => {
                this.listBrandname = value.data;
            }
        });

        this._activatedRoute.queryParamMap.subscribe((params) => {
            this.idChienDich = Number(params.get('idChienDich'));

            this._chienDichService.getById(this.idChienDich).subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res)) {
                        this.form.setValue({
                            idBrandName: res.data.idBrandName,
                            idDanhBa: null,
                            tenChienDich: res.data.tenChienDich,
                            ngayBatDau: res.data.ngayBatDau,
                            ngayKetThuc: res.data.ngayKetThuc,
                            noiDung: res.data.noiDung,
                            isFlashSms: res.data.isFlashSms
                        });
                    }
                }
            });
        });
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

        const body: ISendSms = { ...this.form.value, idChienDich: this.idChienDich };
        this.loading = true;
        this._guiTinNhanService.sendSms(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã đặt lệnh gửi')) {
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

    onClickSave() {
        if (this.isFormInvalid()) {
            return;
        }

        const body: ISaveConfigChienDich = { ...this.form.value, idChienDich: this.idChienDich };
        this.loading = true;
        this._guiTinNhanService.saveConfigChienDich(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã lưu cấu hình gửi')) {
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
}
