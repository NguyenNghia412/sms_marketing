import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ICreateSMSTempalte, IUpdateSMSTempalte, SMSTempalte } from '../../models/sms-template.models';
import { TextareaModule } from 'primeng/textarea';
import { TemplateService } from '@/services/template.service';

@Component({
    selector: 'app-create-template-sms',
    imports: [SharedImports, TextareaModule],
    templateUrl: './create-template-sms.html',
    styleUrl: './create-template-sms.scss'
})
export class CreateTemplateSms extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _templateService = inject(TemplateService);

    data: any | undefined;

    override form: FormGroup = new FormGroup({
        tenMauNoiDung: new FormControl('', [Validators.required]),
        mauNoiDung: new FormControl('', [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        tenMauNoiDung: {
            required: 'Không được bỏ trống'
        },
        mauNoiDung: {
            required: 'Không được bỏ trống'
        }
    };

    get isUpdate() {
        console.log(this._config.data?.id)
        return this._config.data?.id;
    }

    override ngOnInit(): void {
        this.data = this._config.data;
        console.log( this.data)
        if (this.isUpdate) {
            this.initOnUpdate();
        } else {
            this.initOnCreate();
        }
    }

    initOnCreate() {
        // this._roleService.getList().subscribe({
        //     next: (res) => {
        //         if (this.isResponseSucceed(res)) {
        //             this.listRole = res.data;
        //         }
        //     }
        // });
    }

    initOnUpdate() {
        this.form.setValue({
            tenMauNoiDung: this.data?.tenMauNoiDung,
            mauNoiDung: this.data?.noiDung
        });
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

    onSubmitUpdate() {
        const body: any = {
            id: this._config.data?.id,
            noiDung:this.form.get("mauNoiDung")?.value,
            ...this.form.value
        };
        this.loading = true;
        this._templateService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật')) {
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

    onSubmitCreate() {
        const body: ICreateSMSTempalte = {
            ...this.form.value
        };
        this.loading = true;
        this._templateService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Tạo template thành công')) {
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
