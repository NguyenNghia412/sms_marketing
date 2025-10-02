import { IGetExcelInfor, IUploadFileImportDanhBa, IVerifyImportDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileSelectEvent, FileUploadModule } from 'primeng/fileupload';
import { Popover } from 'primeng/popover';


@Component({
    selector: 'app-import',
    imports: [SharedImports, FileUploadModule, Popover],
    templateUrl: './import.html',
    styleUrl: './import.scss'
})
export class Import extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);
    sheetNameOptions: Array<{ label: string; value: string }> = [];

    @ViewChild('op') popover!: Popover;

    override form: FormGroup = new FormGroup({
        IndexRowStartImport: new FormControl(2, [Validators.required]),
        IndexRowHeader: new FormControl(1, [Validators.required]),
        SheetName: new FormControl(null, [Validators.required]),
        File: new FormControl(null, [Validators.required]),
        IndexColumnHoTen: new FormControl(null, [Validators.required]),
        IndexColumnSoDienThoai: new FormControl(null, [Validators.required])
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
        },
        File: {
            required: 'Không được bỏ trống'
        },
        IndexColumnHoTen: {
            required: 'Không được bỏ trống'
        },
        IndexColumnSoDienThoai: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
    }

    onToggleAdvanced(event: Event) {
        if (this.popover) {
            this.popover.toggle(event);
        }
    }

    loadExcelInfo(file: File) {
        if (!file) {
            this.messageError('Không tìm thấy file');
            return;
        }

        const body: IGetExcelInfor = {
            File: file
        };

        this.loading = true;
        this._danhBaService.getExcelInfor(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.sheetNameOptions = value.data.sheets.map(sheet => ({
                        label: sheet.sheetName,
                        value: sheet.sheetName
                    }));
                    if (this.sheetNameOptions.length > 0) {
                        this.form.patchValue({
                            SheetName: this.sheetNameOptions[0].value
                        });
                    }
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

    downloadImportTemplate() {
        this._danhBaService.downloadImportTemplate();
    }

    onSelect(event: FileSelectEvent) {
        const file = event.currentFiles[0];
        this.form.patchValue({
            File: file
        });
        if (file) {
            this.loadExcelInfo(file);
        }
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