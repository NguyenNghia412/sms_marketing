import { IFindPagingMauNoiDung, IViewRowMauNoiDung } from '@/models/template.models';
import { TemplateService } from '@/services/template.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { DataTable } from '@/shared/components/data-table/data-table';
import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { SharedImports } from '@/shared/import.shared';
import { IColumn } from '@/shared/models/data-table.models';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PaginatorState } from 'primeng/paginator';
import { TblAction, TblActionTypes } from './tbl-action/tbl-action';
import { EmailTempalte } from '../../models/data-email-template.models';

@Component({
    selector: 'app-mau-noi-dung',
    imports: [SharedImports, DataTable],
    templateUrl: './mau-noi-dung.html',
    styleUrl: './mau-noi-dung.scss'
})
export class MauNoiDung extends BaseComponent {
    _templateService = inject(TemplateService);

    searchForm: FormGroup = new FormGroup({
        search: new FormControl('')
    });

    listEmailTempalte: EmailTempalte[] = [];
    columns: IColumn[] = [
        { header: 'STT', cellViewType: CellViewTypes.INDEX, headerContainerStyle: 'width: 6rem' },
        { header: 'Tiêu đề template', field: 'nameTemplate', headerContainerStyle: 'min-width: 10rem' },
        { header: 'Thời gian tạo', field: 'createdDate', headerContainerStyle: 'width: 10rem', cellViewType: CellViewTypes.DATE, dateFormat: 'dd/MM/yyyy HH:mm:ss' },
        { header: 'Thao tác', headerContainerStyle: 'width: 12rem', cellViewType: CellViewTypes.CUSTOM_COMP, customComponent: TblAction }
    ];

    data: EmailTempalte[] = [];
    query: IFindPagingMauNoiDung = {
        pageNumber: 1,
        pageSize: this.MAX_PAGE_SIZE
    };

    override ngOnInit(): void {
        this.getData();
    }

    onSearch() {
        this.getData();
    }

    onPageChanged($event: PaginatorState) {
        this.query.pageNumber = ($event.page ?? 0) + 1;
        this.getData();
    }

    getData() {
        this.loading = true;
        this.loading = false;
        this.data = [
            {
                id: 1,
                nameTemplate: 'EMAIL Template 1 ',
                design: {
                    counters: {
                        u_column: 17,
                        u_row: 8,
                        u_content_paragraph: 4,
                        u_content_heading: 21,
                        u_content_html: 1,
                        u_content_image: 8,
                        u_content_button: 1,
                        u_content_social: 2
                    },
                    body: {
                        id: '6brvs6Z7lo',
                        rows: [
                            {
                                id: 'bNpfPq98lP',
                                cells: [1],
                                columns: [
                                    {
                                        id: 'liP3PJTHSt',
                                        contents: [
                                            {
                                                id: 'AxQiXL2N2q',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h1',
                                                    fontSize: '33px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_2',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<strong>TITLE OF EMAIL</strong>',
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'QtjF_q5dKA',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h1',
                                                    fontSize: '22px',
                                                    textAlign: 'center',
                                                    lineHeight: '30%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_1',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<span>THANKS {{name}}</span>',
                                                    _languages: {}
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_1',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    }
                                ],
                                values: {
                                    displayCondition: null,
                                    columns: false,
                                    _styleGuide: null,
                                    backgroundColor: '',
                                    columnsBackgroundColor: '',
                                    backgroundImage: {
                                        url: '',
                                        fullWidth: true,
                                        repeat: 'no-repeat',
                                        size: 'custom',
                                        position: 'center',
                                        customPosition: ['50%', '50%']
                                    },
                                    padding: '0px',
                                    anchor: '',
                                    hideDesktop: false,
                                    _meta: {
                                        htmlID: 'u_row_1',
                                        htmlClassNames: 'u_row'
                                    },
                                    selectable: true,
                                    draggable: true,
                                    duplicatable: true,
                                    deletable: true,
                                    hideable: true,
                                    locked: false
                                }
                            },
                            {
                                id: 'ovE7Ap-iQR',
                                cells: [1],
                                columns: [
                                    {
                                        id: '2vvQi8uge3',
                                        contents: [
                                            {
                                                id: '9czBVq-mJF',
                                                type: 'paragraph',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    fontSize: '15px',
                                                    textAlign: 'center',
                                                    lineHeight: '190%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_paragraph_2',
                                                        htmlClassNames: 'u_content_paragraph'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    textJson:
                                                        '{"root":{"children":[{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Smoking pipe tobacco boosts one’s pride.","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\nFragrant breath, lungs fortified, chasing all the germs aside.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Raise the pipe like Zhao Yun lifting his spear high.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Blow the smoke like Kongming summoning the winds from the sky.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\nOne man smokes — four men get dizzy.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Two light the ember — both fall down easy.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"An old granny carrying firewood, all busy,\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Breathes in a whiff — tumbles down the hill all woozy.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\nJade Emperor sees it and laughs away,","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\nPulls apart the clouds — even he gets smoked today.","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"}],"direction":"ltr","format":"","indent":0,"type":"root","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"}}',
                                                    _languages: {}
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_2',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    }
                                ],
                                values: {
                                    displayCondition: null,
                                    columns: false,
                                    _styleGuide: null,
                                    backgroundColor: '',
                                    columnsBackgroundColor: '',
                                    backgroundImage: {
                                        url: '',
                                        fullWidth: true,
                                        repeat: 'no-repeat',
                                        size: 'custom',
                                        position: 'center'
                                    },
                                    padding: '0px',
                                    anchor: '',
                                    hideDesktop: false,
                                    _meta: {
                                        htmlID: 'u_row_2',
                                        htmlClassNames: 'u_row'
                                    },
                                    selectable: true,
                                    draggable: true,
                                    duplicatable: true,
                                    deletable: true,
                                    hideable: true,
                                    locked: false
                                }
                            },
                            {
                                id: 'qs75Fnvz_1',
                                cells: [1],
                                columns: [
                                    {
                                        id: '_Gky3OomoP',
                                        contents: [
                                            {
                                                id: 'RM7NwPBMtA',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h2',
                                                    fontWeight: 700,
                                                    fontSize: '20px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_10',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<span>@ Về ch&uacute;ng t&ocirc;i</span>',
                                                    _languages: {}
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_10',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    }
                                ],
                                values: {
                                    displayCondition: null,
                                    columns: false,
                                    _styleGuide: null,
                                    backgroundColor: '',
                                    columnsBackgroundColor: '',
                                    backgroundImage: {
                                        url: '',
                                        fullWidth: true,
                                        repeat: 'no-repeat',
                                        size: 'custom',
                                        position: 'center'
                                    },
                                    padding: '0px',
                                    anchor: '',
                                    hideDesktop: false,
                                    _meta: {
                                        htmlID: 'u_row_6',
                                        htmlClassNames: 'u_row'
                                    },
                                    selectable: true,
                                    draggable: true,
                                    duplicatable: true,
                                    deletable: true,
                                    hideable: true,
                                    locked: false
                                }
                            },
                            {
                                id: 'Ih4NO_Dhy8',
                                cells: [1, 1, 1],
                                columns: [
                                    {
                                        id: '6-jV9IAboZ',
                                        contents: [
                                            {
                                                id: 'iDUMkDzifk',
                                                type: 'paragraph',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    fontWeight: 700,
                                                    fontSize: '14px',
                                                    textAlign: 'left',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_paragraph_3',
                                                        htmlClassNames: 'u_content_paragraph'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    textJson:
                                                        '{"root":{"children":[{"children":[{"detail":0,"format":0,"mode":"normal","text":"Liên hệ với chúng tôi","type":"extended-text","version":1}],"format":"","indent":0,"type":"paragraph","version":1,"textFormat":0}],"format":"","indent":0,"type":"root","version":1}}',
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: '8exIcPIbIZ',
                                                type: 'social',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    icons: {
                                                        iconType: 'circle',
                                                        icons: [
                                                            {
                                                                name: 'Facebook',
                                                                url: 'https://facebook.com/'
                                                            },
                                                            {
                                                                name: 'Messenger',
                                                                url: 'https://messenger.com/'
                                                            },
                                                            {
                                                                name: 'Instagram',
                                                                url: 'https://instagram.com/'
                                                            }
                                                        ]
                                                    },
                                                    align: 'center',
                                                    iconSize: 32,
                                                    spacing: 5,
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_social_2',
                                                        htmlClassNames: 'u_content_social'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_11',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    },
                                    {
                                        id: 'pxrc40V7WR',
                                        contents: [
                                            {
                                                id: '08DsnBgxOv',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontWeight: 700,
                                                    fontSize: '14px',
                                                    textAlign: 'left',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_12',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<span>Email</span>',
                                                    _languages: {}
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_12',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    },
                                    {
                                        id: 'zQzf-7EtoF',
                                        contents: [
                                            {
                                                id: 'DpK6StAAPk',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontWeight: 700,
                                                    fontSize: '15px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_13',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: 'Trần Văn Quang',
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'swIUOMuUvk',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontSize: '14px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_16',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<strong>Nguyễn Trọng Nghĩa<br></strong>',
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'zaH3bB30k0',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontSize: '14px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_17',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<strong>Phạm Ngọc Hiệp<br></strong>',
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'pTUSq9glQb',
                                                type: 'heading',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontSize: '14px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    hideDesktop: false,
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_18',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    text: '<strong>Nguyễn Việt Cường<br></strong>',
                                                    _languages: {}
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_17',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    }
                                ],
                                values: {
                                    displayCondition: null,
                                    columns: false,
                                    _styleGuide: null,
                                    backgroundColor: '',
                                    columnsBackgroundColor: '',
                                    backgroundImage: {
                                        url: '',
                                        fullWidth: true,
                                        repeat: 'no-repeat',
                                        size: 'custom',
                                        position: 'center'
                                    },
                                    padding: '0px',
                                    anchor: '',
                                    hideDesktop: false,
                                    _meta: {
                                        htmlID: 'u_row_7',
                                        htmlClassNames: 'u_row'
                                    },
                                    selectable: true,
                                    draggable: true,
                                    duplicatable: true,
                                    deletable: true,
                                    hideable: true,
                                    locked: false
                                }
                            }
                        ],
                        headers: [],
                        footers: [],
                        values: {
                            _styleGuide: null,
                            popupPosition: 'center',
                            popupDisplayDelay: 0,
                            popupWidth: '600px',
                            popupHeight: 'auto',
                            borderRadius: '10px',
                            contentAlign: 'center',
                            contentVerticalAlign: 'center',
                            contentWidth: '500px',
                            fontFamily: {
                                label: 'Arial',
                                value: 'arial,helvetica,sans-serif'
                            },
                            textColor: '#000000',
                            popupBackgroundColor: '#FFFFFF',
                            popupBackgroundImage: {
                                url: '',
                                fullWidth: true,
                                repeat: 'no-repeat',
                                size: 'cover',
                                position: 'center',
                                customPosition: ['50%', '50%']
                            },
                            popupOverlay_backgroundColor: 'rgba(0, 0, 0, 0.1)',
                            popupCloseButton_position: 'top-right',
                            popupCloseButton_backgroundColor: '#DDDDDD',
                            popupCloseButton_iconColor: '#000000',
                            popupCloseButton_borderRadius: '0px',
                            popupCloseButton_margin: '0px',
                            popupCloseButton_action: {
                                name: 'close_popup',
                                attrs: {
                                    onClick: "document.querySelector('.u-popup-container').style.display = 'none';"
                                }
                            },
                            language: {},
                            backgroundColor: '#F7F8F9',
                            preheaderText: '',
                            linkStyle: {
                                body: true,
                                linkColor: '#0000ee',
                                linkHoverColor: '#0000ee',
                                linkUnderline: true,
                                linkHoverUnderline: true
                            },
                            backgroundImage: {
                                url: '',
                                fullWidth: true,
                                repeat: 'no-repeat',
                                size: 'custom',
                                position: 'center',
                                customPosition: ['50%', '50%']
                            },
                            accessibilityTitle: '',
                            _meta: {
                                htmlID: 'u_body',
                                htmlClassNames: 'u_body'
                            }
                        }
                    },
                    schemaVersion: 21
                }
            }
        ];
        // this._templateService
        //     .findPaging({ ...this.query, keyword: this.searchForm.get('search')?.value })
        //     .subscribe({
        //         next: (res) => {
        //             if (this.isResponseSucceed(res, false)) {
        //                 this.data = res.data.items;
        //                 this.totalRecords = res.data.totalItems;
        //             }
        //         }
        //     })
        //     .add(() => {
        //         this.loading = false;
        //     });
    }

    onOpenCreate() {
        this.router.navigate(['template/mau-nd/create-template-email'], {});
    }

    onOpenUpdate(data: any) {
        this.router.navigate(['template/mau-nd/create-template-email'], {
            queryParams: {
                design: encodeURIComponent(JSON.stringify(data.design))
            }
        });
    }

    onCustomEmit(data: { type: string; data: IViewRowMauNoiDung }) {
        if (data.type === TblActionTypes.use) {
            // const uri = '/danh-ba/chi-tiet';
            // this.router.navigate([uri], {
            //     queryParams: {
            //         id: data.data.id
            //     }
            // });
        } else if (data.type === TblActionTypes.delete) {
            this.onDelete(data.data);
        } else if (data.type === TblActionTypes.update) {
            this.onOpenUpdate(data.data);
        }
    }

    onDelete(data: IViewRowMauNoiDung) {
        this.confirmDelete(
            {
                header: 'Bạn chắc chắn muốn xóa template?',
                message: 'Không thể khôi phục sau khi xóa'
            },
            () => {
                this._templateService.delete(data.idMauNoiDung || 0).subscribe(
                    (res) => {
                        if (this.isResponseSucceed(res, true, 'Đã xóa')) {
                            this.getData();
                        }
                    },
                    (err) => {
                        this.messageError(err?.message);
                    }
                );
            }
        );
    }
}
