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
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    if (value.data.fileKey) {
                        this.showErrorFileDialog(value.data);
                    } else {

                        this.confirmService.confirm({
                        header: 'Tiếp tục import?',
                        message: `Số dòng import: ${value.data.totalRowsImported}; Số trường dữ liệu: ${value.data.totalDataImported}`,
                        acceptLabel: 'Import',
                        rejectLabel: 'Hủy',
                        acceptIcon: 'pi pi-check',
                        rejectIcon: 'pi pi-times',
                        accept: () => {
                          this.callApiImport();
                        }
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

  showErrorFileDialog(verifyData: IViewVerifyImportDanhBa) {
    const errorList = verifyData.data.map(item => 
        `• <strong>${item.nguyenNhanLoi}</strong>: ${item.soLuongLoi} bản ghi`
    ).join('<br>');
    
    let shouldDownload = false;

    const styleId = 'dialog-center-fix';
    if (!document.getElementById(styleId)) {
        const style = document.createElement('style');
        style.id = styleId;
        style.textContent = `
            .p-dialog {
                position: fixed !important;
                top: 50% !important;
                left: 50% !important;
                transform: translate(-50%, -50%) !important;
                margin: 0 !important;
            }
        `;
        document.head.appendChild(style);
    }
    
    this.confirmService.confirm({
        header: 'WARNING',
        message: `File import xuất hiện lỗi:<br><br>${errorList}<br><br>Vui lòng tải về file và điều chỉnh lại.<br><br>Vẫn có thể tiếp tục import nhưng chúng tôi sẽ không chịu trách nhiệm nếu xảy ra bất kì lỗi ngoài ý muốn nào.`,
        acceptLabel: 'Import',
        rejectLabel: 'Tải về file',
        acceptIcon: 'pi pi-check',
        rejectIcon: 'pi pi-download',
        reject: () => {
            if (shouldDownload && verifyData.fileKey) {
                this.downloadErrorFile(verifyData.fileKey);
            }
        },
        accept: () => {
            this.callApiImport();
        }
    });

    setTimeout(() => {
        const allDialogs = document.querySelectorAll('.p-dialog');
        const confirmDialog = allDialogs[allDialogs.length - 1] as HTMLElement;
        
        if (!confirmDialog) return;
        
        const header = confirmDialog.querySelector('.p-dialog-header');
        if (header?.textContent?.trim() !== 'WARNING') return;

        const headerElement = confirmDialog.querySelector('.p-dialog-header') as HTMLElement;
        if (headerElement) {
            headerElement.style.cursor = 'default';
            headerElement.onmousedown = (e) => {
                e.preventDefault();
                e.stopPropagation();
                return false;
            };
        }

        const buttons = confirmDialog.querySelectorAll('button');
        buttons.forEach(button => {
            const buttonText = button.textContent?.trim();
            if (buttonText === 'Tải về file') {
                button.addEventListener('click', () => { shouldDownload = true; }, true);
            }
            if (button.classList.contains('p-dialog-header-close') || button.querySelector('.pi-times')) {
                button.addEventListener('click', () => { shouldDownload = false; }, true);
            }
        });
    }, 0);
}
    downloadErrorFile(fileKey: string) {
        this.loading = true;
        this._danhBaService.downloadFileFailedCache(fileKey).subscribe({
            next: (response) => {
                const contentDisposition = response.headers.get('content-disposition');
                let fileName = 'file_loi.xlsx';

                if (contentDisposition) {
                    const matches = /filename\*?=(?:UTF-8''|)([^;]+)/i.exec(contentDisposition);
                    if (matches?.[1]) {
                        fileName = decodeURIComponent(matches[1].trim().replace(/['"]/g, ''));
                    }
                }

                const blob = response.body!;
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
                
            },
            error: (err) => {
                this.messageError(err?.message || 'Không thể tải file');
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
                if (this.isResponseSucceed(value, true, 'Import thành công')) {
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