import { ICreateDanhBa, IUpdateDanhBa } from '@/models/danh-ba.models';
import { ICreateConfigPlan, IUpdateConfigPlan } from '@/models/trao-bang/plan.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { TraoBangPlanService } from '@/services/trao-bang/plan.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DanhBaTypes } from '@/shared/constants/channel.constants';
import { SharedImports } from '@/shared/import.shared';
import { Utils } from '@/shared/utils';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _planService = inject(TraoBangPlanService);

    override form: FormGroup = new FormGroup({
        id: new FormControl(null),
        ten: new FormControl('', [Validators.required]),
        moTa: new FormControl(''),
        time: new FormControl(null, [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        ten: {
            required: 'Không được bỏ trống'
        },
        time: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
        if (this.isUpdate) {
            this.form.setValue({
                id: this._config.data.id,
                ten: this._config.data.ten,
                moTa: this._config.data.moTa,
                time: [new Date(this._config.data.thoiGianBatDau), new Date(this._config.data.thoiGianKetThuc)]
            });
        }
    }

    get isUpdate() {
        return this._config.data?.id;
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

       if (this.isUpdate) {
        this.onSubmitUpdate();
       } else {
        this.onSubmitCreate();
       }
    }

    onSubmitCreate() {
        const from = Utils.formatDateCallApi(this.form.value['time'][0])
        const to = this.form.value['time'][1] ? Utils.formatDateCallApi(this.form.value['time'][1]) : from;

        const body: ICreateConfigPlan = {
            ten: this.form.value['ten'],
            moTa: this.form.value['moTa'],
            thoiGianBatDau: from,
            thoiGianKetThuc: to
        };
        this.loading = true;
        this._planService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm chương trình')) {
                    this._ref?.close(true);
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

    onSubmitUpdate() {
        const from = Utils.formatDateCallApi(this.form.value['time'][0])
        const to = this.form.value['time'][1] ? Utils.formatDateCallApi(this.form.value['time'][1]) : from;

        const body: IUpdateConfigPlan = {
            id: this.form.value['id'],
            ten: this.form.value['ten'],
            moTa: this.form.value['moTa'],
            thoiGianBatDau: from,
            thoiGianKetThuc: to
        };
        this.loading = true;
        this._planService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã lưu')) {
                    this._ref?.close(true);
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
