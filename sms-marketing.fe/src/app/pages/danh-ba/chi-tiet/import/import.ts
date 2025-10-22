import { IGetExcelInfor, IUploadFileImportDanhBa, IVerifyImportDanhBa, IViewVerifyImportDanhBa } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { IBaseResponseWithData } from '@/shared/models/request-paging.base.models';
import { Component, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileSelectEvent, FileUploadModule } from 'primeng/fileupload';
import { Popover } from 'primeng/popover';
import { ConfirmationService } from 'primeng/api';
import { HttpResponse } from '@angular/common/http';


@Component({
    selector: 'app-import',
    imports: [SharedImports, FileUploadModule, Popover],
    templateUrl: './import.html',
    styleUrl: './import.scss'
})
export class Import extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private confirmService = inject(ConfirmationService);
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
        this.disableDialogDrag();
    }

    disableDialogDrag() {
        setTimeout(() => {
            const dialogs = document.querySelectorAll('.p-dialog-header');
            dialogs.forEach(header => {
                (header as HTMLElement).style.cursor = 'default';
            });
        }, 0);
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
            next: async (response: HttpResponse<Blob>) => {
                const contentType = response.headers.get('content-type');
                
                if (contentType?.includes('application/json')) {
                    const text = await response.body!.text();
                    const jsonData = JSON.parse(text) as IBaseResponseWithData<IViewVerifyImportDanhBa>;
                    
                    if (this.isResponseSucceed(jsonData)) {
                        this.confirmAction(
                            {
                                header: 'Tiếp tục import?',
                                message: `Số dòng import: ${jsonData.data.totalRowsImported}; Số trường dữ liệu: ${jsonData.data.totalDataImported}`
                            },
                            () => {
                                this.callApiImport();
                            }
                        );
                    }
                } else {
                    const contentDisposition = response.headers.get('content-disposition');
                    let fileName = 'file_loi.xlsx';
                    
                    if (contentDisposition) {
                        const matches = /filename\*?=(?:UTF-8''|)([^;]+)/i.exec(contentDisposition);
                        if (matches?.[1]) {
                            fileName = decodeURIComponent(matches[1].trim().replace(/['"]/g, ''));
                        }
                    }
                    
                    this.showErrorFileDialog(response.body!, fileName);
                }
            }
        }).add(() => {
            this.loading = false;
        });
    }

    showErrorFileDialog(fileBlob: Blob, fileName: string) {
        this.confirmService.confirm({
            header: 'File import xuất hiện lỗi',
            message: 'File import xuất hiện lỗi. Vui lòng tải về file và điều chỉnh lại.\n\nVẫn có thể tiếp tục import nhưng chúng tôi sẽ không chịu trách nhiệm nếu xảy ra bất kì lỗi ngoài ý muốn nào.',
            acceptLabel: 'Import',
            rejectLabel: 'Tải về file',
            acceptIcon: 'pi pi-check',
            rejectIcon: 'pi pi-download',
            reject: () => {
                this.downloadErrorFile(fileBlob, fileName);
            },
            accept: () => {
                this.callApiImport();
            }
        });

        setTimeout(() => {
            this.disableDialogDrag();
        }, 0);
    }

    downloadErrorFile(fileBlob: Blob, fileName: string) {
        const url = window.URL.createObjectURL(fileBlob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
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