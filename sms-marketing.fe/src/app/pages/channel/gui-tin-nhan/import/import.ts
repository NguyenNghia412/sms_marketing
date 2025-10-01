import { IImportCreateDanhBa, IUploadFileImportDanhBa, IVerifyImportCreateDanhBa, IVerifyImportDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileSelectEvent, FileUploadModule } from 'primeng/fileupload';

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
        TenDanhBa : new FormControl(null, [Validators.required]),
        File: new FormControl(null, [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        TenDanhBa:{
            required:'Không được bỏ trống'
        },
        File:{
            required:'Không được bỏ trống'
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

       
    }
}