import { IImportCreateDanhBa, IUploadFileImportDanhBa, IVerifyImportCreateDanhBa, IVerifyImportDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { FileSelectEvent, FileUploadModule } from 'primeng/fileupload';
import { ChiTietImport } from './chi-tiet/chi-tiet';

@Component({
    selector: 'app-import',
    imports: [SharedImports, FileUploadModule],
    templateUrl: './import.html',
    styleUrl: './import.scss'
})
export class Import extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);

    override form: FormGroup = new FormGroup({
        TenDanhBa: new FormControl(null, [Validators.required]),
        File: new FormControl(null, [Validators.required]),
        Mota: new FormControl(null)
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        TenDanhBa: {
            required: 'Không được bỏ trống'
        },
        File: {
            required: 'Không được bỏ trống'
        }
    };

    downloadImportTemplate() {
        this._danhBaService.downloadImportTemplate();
    }

    onSelect(event: FileSelectEvent) {
        this.form.patchValue({
            File: event.currentFiles[0]
        });
    }

    onSubmit() {
        if (!this.form.value.File) {
            this.messageWarning('Chưa chọn file');
            return;
        }
        if (this.isFormInvalid()) {
            return;
        }

        this.openChiTietDialog();
    }

    openChiTietDialog() {
        const ref = this._dialogService.open(ChiTietImport, {
            header: 'Hoàn thiện thông tin import',
            closable: true,
            modal: true,
            styleClass: 'w-[600px] dialog-import-header',
            focusOnShow: false,
            data: {
                //idDanhBa: this._config.data?.idDanhBa,
                tenDanhBa: this.form.value.TenDanhBa,
                file: this.form.value.File
            }
        });
        ref.onClose.subscribe((result) => {
            if (result) {
                this._ref.close(true);
            }
        });
    }
}