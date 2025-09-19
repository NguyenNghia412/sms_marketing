import { IUploadFileImportDanhBa, IVerifyImportDanhBa } from '@/models/danh-ba.models';
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
        IndexRowStartImport: new FormControl(5, [Validators.required]),
        IndexRowHeader: new FormControl(4, [Validators.required]),
        SheetName: new FormControl('Data', [Validators.required]),
        File: new FormControl(null, [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        IndexRowStartImport: {
            required: 'Không được bỏ trống'
        },
        IndexRowHeader: {
            required: 'Không được bỏ trống'
        },
        SheetName: {
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

        const body: IVerifyImportDanhBa = { ...this.form.value, IdDanhBa: this._config.data?.idDanhBa };

        this.loading = true;
        this._danhBaService.verifyFileImport(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.confirmAction(
                        {
                            header: 'Tiếp tục import?',
                            message: `Số dòng import: ${value.data.totalRowsImported}; Số trường dữ liệu: ${value.data.totalDataImported}`
                        },
                        () => {
                            this.callApiImport();
                        }
                    );
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

    callApiImport() {
        const body: IUploadFileImportDanhBa = { ...this.form.value, IdDanhBa: this._config.data?.idDanhBa };

        this.loading = true;
        this._danhBaService.uploadFileImport(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this._ref.close(true);
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
