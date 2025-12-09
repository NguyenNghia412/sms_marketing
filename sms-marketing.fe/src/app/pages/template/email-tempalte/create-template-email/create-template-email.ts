import { EmailTempalte, ICreateEmailTempalte } from './../../models/data-email-template.models';
import { SharedImports } from '@/shared/import.shared';
import { Component, ViewChild, OnInit, inject } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Breadcrumb } from '@/shared/components/breadcrumb/breadcrumb';
import { BaseComponent } from '@/shared/components/base/base-component';
import { EmailEditorComponent, EmailEditorModule } from 'angular-email-editor';
import { ToolsConfig, UnlayerOptions } from 'node_modules/angular-email-editor/types';
import { TemplateEmailService } from '@/services/template-email.service';
import { DrawerModule } from 'primeng/drawer';
import { FormControl, FormGroup, Validators } from '@angular/forms';
declare var unlayer: any;

@Component({
    selector: 'app-create-template-email',
    imports: [SharedImports, Breadcrumb, EmailEditorModule, DrawerModule],
    templateUrl: './create-template-email.html',
    styleUrl: './create-template-email.scss'
})
export class CreateTemplateEmail extends BaseComponent {
    private _templateEmailService = inject(TemplateEmailService);
    @ViewChild(EmailEditorComponent)
    editor!: EmailEditorComponent;
    visible: boolean = false
    showSelectModal = false;
    selectedPerson: any = null;
    templateEmail!: EmailTempalte
    design: any = null;
    idTemplate!: number;


    override form: FormGroup = new FormGroup({
        tenMauNoiDung: new FormControl('', [Validators.required]),
    });


    override ngOnInit() {
        this._activatedRoute.queryParamMap.subscribe((params) => {
            const designParam = params.get('design');
            if (designParam) {
                this.design = JSON.parse(decodeURIComponent(designParam));
            }
        });
    }

    variables = [
        { label: 'Tên khách hàng', value: '{{name}}', key: 'name' },
        { label: 'Email khách hàng', value: '{{email}}', key: 'email' },
        { label: 'Số điện thoại', value: '{{sdt}}', key: 'sdt' }
    ];

    data: any[] = [
        {
            id: 1,
            name: 'Phạm Ngọc Hiệp',
            email: 'hieppn@huce.edu.vn',
            sdt: '0904179061'
        },
        {
            id: 2,
            name: 'Nguyễn Trọng Nghĩa',
            email: 'nghiant@huce.edu.vn',
            sdt: '0904179061'
        },
        { id: 3, name: 'Nguyễn Việt Cường', email: 'cuongnv@huce.edu.vn', sdt: '0904179061' },
        {
            id: 4,
            name: 'Trần Văn Quang',
            email: 'quangvt@huce.edu.vn',
            sdt: '0904179061'
        }
    ];

    options: UnlayerOptions = {
        version: 'latest',
        features: {
            textEditor: {
                tables: true
            },
            colorPicker: {},
            imageEditor: {
                enabled: true
            },
            undoRedo: true,
            audit: true,
            ai: true,
            blocks: true
        },
        // Cấu hình merge tags cho editor
        mergeTags: this.getMergeTagsConfig()
    };

    items: MenuItem[] = [{ label: 'Danh sách template', routerLink: '/channel/email' }, { label: 'Tempale email' }];
    home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

    onLoad() { }

    onReady(event: any) {
        if (this.design) {
            this.editor.loadDesign(this.design);
        }
    }

    getTemplateById() {
        this.loading = true;
        this._templateEmailService.getById(this.idTemplate).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, false)) {
                    this.templateEmail = res.data
                }
            }
        })
    }

    // Cấu hình merge tags cho Unlayer
    getMergeTagsConfig() {
        const config: { [key: string]: any } = {};
        this.variables.forEach((variable) => {
            config[variable.key] = {
                name: variable.label,
                value: variable.value
            };
        });
        return config;
    }

    // Export template without data replacement
    exportTemplate() {
        if (!this.editor) {
            return;
        }

        this.editor.exportHtml((templateData: any) => {
            this.showPreview(templateData.html);
        });
    }

    selectData(event: any) {
        const selectedId = event.value;
        this.selectedPerson = this.data.find((person) => person.id === selectedId);
    }

    previewWithData() {
        if (!this.editor || !this.selectedPerson) {
            return;
        }

        this.editor.exportHtml((templateData: any) => {
            let html = templateData.html;

            this.variables.forEach((variable) => {
                const regex = new RegExp(variable.value.replace(/[{}]/g, '\\$&'), 'g');
                html = html.replace(regex, this.selectedPerson[variable.key]);
            });

            this.showPreview(html);
        });
    }

    exportWithRealData(realData: { [key: string]: string }) {
        if (!this.editor) return;

        this.editor.exportHtml((data: any) => {
            let html = data.html;

            Object.keys(realData).forEach((key) => {
                const regex = new RegExp(`{{${key}}}`, 'g');
                html = html.replace(regex, realData[key]);
            });

            return html;
        });
    }

    saveTemplate() {
         if (this.isFormInvalid()) {
            return;
        }
        this.loading = true;
        this.editor.exportHtml((templateData: any) => {
            const body: ICreateEmailTempalte = {
                tenMauNoiDung: this.form.get("tenMauNoiDung")?.value,
                thietKe: this.designToString(templateData.design)
            }
            this._templateEmailService.create(body).subscribe({
                next: (res) => {
                    if (this.isResponseSucceed(res, true, 'Tạo template thành công')) {
                        this.ngOnInit()
                    }
                },
                error: (err) => {
                    this.messageError(err?.message);
                },
                complete: () => {
                    this.loading = false;
                }
            })
        });
    }

    private showPreview(html: string) {
        const previewWindow = window.open('', '_blank', 'width=800,height=600');

        if (previewWindow) {
            previewWindow.document.write(`
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Email Preview</title>
                    <meta charset="utf-8">
                </head>
                <body>
                    ${html}
                </body>
                </html>
            `);
            previewWindow.document.close();
        }
    }
}
