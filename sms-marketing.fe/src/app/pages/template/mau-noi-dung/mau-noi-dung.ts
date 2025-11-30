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
import { EmailTempalte } from '../models/data-email-template.models';
import { CreateTemplateEmail } from '../create-template-email/create-template-email';

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
                nameTemplate: 'EMAIL Quang Văn Trần King',
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
                                                    text: '<strong>☣ King Of L&aacute;o Thuộc ☣</strong>',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'QtjF_q5dKA',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong><span style="line-height: 6.6px;">Chiếu Chỉ</span></strong>',
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
                                id: 'nKY5rgw26y',
                                cells: [1],
                                columns: [
                                    {
                                        id: 'WkZFBxytZa',
                                        contents: [
                                            {
                                                id: '-hXkntetDx',
                                                type: 'image',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    src: {
                                                        url: 'https://assets.unlayer.com/projects/0/1764485701801-fotor-face-swap-20251130134838.jpg',
                                                        width: 800,
                                                        height: 533,
                                                        filename: 'fotor-face-swap-20251130134838.jpg',
                                                        contentType: 'image/jpeg',
                                                        size: 84252,
                                                        dynamic: true,
                                                        autoWidth: false,
                                                        maxWidth: '73%'
                                                    },
                                                    textAlign: 'center',
                                                    altText: '',
                                                    action: {
                                                        name: 'web',
                                                        values: {
                                                            href: '',
                                                            target: '_blank'
                                                        }
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_image_4',
                                                        htmlClassNames: 'u_content_image'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    pending: false
                                                }
                                            }
                                        ],
                                        values: {
                                            backgroundColor: '',
                                            padding: '0px',
                                            border: {},
                                            borderRadius: '0px',
                                            _meta: {
                                                htmlID: 'u_column_6',
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
                                    _meta: {
                                        htmlID: 'u_row_4',
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
                                                    textJson:
                                                        '{"root":{"children":[{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"Hút thuốc lào nâng cao sĩ diện.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nThơm mồm bổ phổi,diệt trùng lao.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nNâng điếu lên như Triệu Tử cầm đao.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nNhả khói ra như Khổng Minh gọi gió.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nMột thằng hút,bốn thằng say.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nHai thằng châm đóm ngã lăn quay.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nBà già vác củi loay hoay.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nHít phải mùi thuốc lăn quay xuống đồi.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nNgọc Hoàng trông thấy hay hay.\\n","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"},{"children":[{"detail":0,"format":1,"mode":"normal","style":"background-color: ;color: ;","text":"\\r\\nVén mây nhìn xuống cũng say thuốc lào","type":"extended-text","version":1}],"direction":"ltr","format":"","indent":0,"type":"paragraph","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"}],"direction":"ltr","format":"","indent":0,"type":"root","version":1,"textFormat":1,"textStyle":"background-color: ;color: ;"}}',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'RrzF9cOWOK',
                                                type: 'html',
                                                values: {
                                                    html: '<iframe width="500" height="280" src="https://www.youtube.com/embed/3QyGsA2AGR8?si=uVTVfv2L416hfn0C" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>',
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    containerPadding: '0px',
                                                    anchor: '',
                                                    _meta: {
                                                        htmlID: 'u_content_html_1',
                                                        htmlClassNames: 'u_content_html'
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
                                id: 'tUpBp9IREU',
                                cells: [1, 1, 1],
                                columns: [
                                    {
                                        id: '7K6VwMLagq',
                                        contents: [
                                            {
                                                id: 'JPrQBejGn_',
                                                type: 'image',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    src: {
                                                        url: 'https://assets.unlayer.com/projects/0/1764486746714-249cb10e-82d2-4423-9bcb-d47cc20212f9.png',
                                                        width: 1000,
                                                        height: 524,
                                                        filename: '249cb10e-82d2-4423-9bcb-d47cc20212f9.png',
                                                        contentType: 'image/png',
                                                        size: 466836,
                                                        dynamic: true
                                                    },
                                                    textAlign: 'center',
                                                    altText: '',
                                                    action: {
                                                        name: 'web',
                                                        values: {
                                                            href: '',
                                                            target: '_blank'
                                                        }
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_image_6',
                                                        htmlClassNames: 'u_content_image'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    pending: false
                                                }
                                            },
                                            {
                                                id: 'oSoLK7rVcz',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>V&otilde; Nghĩa</strong>',
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontSize: '16px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_7',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
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
                                                htmlID: 'u_column_7',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    },
                                    {
                                        id: 'f9d_eTvRUX',
                                        contents: [
                                            {
                                                id: 'eBjLGhhGzc',
                                                type: 'image',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    src: {
                                                        url: 'https://assets.unlayer.com/projects/0/1764485824737-fotor-face-swap-20251130134533.jpg',
                                                        width: 640,
                                                        height: 360,
                                                        filename: 'fotor-face-swap-20251130134533.jpg',
                                                        contentType: 'image/jpeg',
                                                        size: 30978,
                                                        dynamic: true,
                                                        autoWidth: false,
                                                        maxWidth: '92%'
                                                    },
                                                    textAlign: 'center',
                                                    altText: '',
                                                    action: {
                                                        name: 'web',
                                                        values: {
                                                            href: '',
                                                            target: '_blank'
                                                        }
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_image_5',
                                                        htmlClassNames: 'u_content_image'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    pending: false
                                                }
                                            },
                                            {
                                                id: 'DZTXbXEp8_',
                                                type: 'heading',
                                                values: {
                                                    text: '<span>Gia C&aacute;t Nghĩa</span>',
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontWeight: 700,
                                                    fontSize: '16px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_8',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
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
                                                htmlID: 'u_column_8',
                                                htmlClassNames: 'u_column'
                                            },
                                            deletable: true,
                                            locked: false
                                        }
                                    },
                                    {
                                        id: '_Dlnvya1yK',
                                        contents: [
                                            {
                                                id: 'JkF-UESe0P',
                                                type: 'image',
                                                values: {
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    src: {
                                                        url: 'https://assets.unlayer.com/projects/0/1764486922677-fotor-face-swap-20251130141506.jpg',
                                                        width: 638,
                                                        height: 392,
                                                        filename: 'fotor-face-swap-20251130141506.jpg',
                                                        contentType: 'image/jpeg',
                                                        size: 32975,
                                                        dynamic: true,
                                                        autoWidth: false,
                                                        maxWidth: '83%'
                                                    },
                                                    textAlign: 'center',
                                                    altText: '',
                                                    action: {
                                                        name: 'web',
                                                        values: {
                                                            href: '',
                                                            target: '_blank'
                                                        }
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_image_8',
                                                        htmlClassNames: 'u_content_image'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    pending: false
                                                }
                                            },
                                            {
                                                id: 'iFm7KzJUEP',
                                                type: 'heading',
                                                values: {
                                                    text: '<span>Dương Nghĩa</span>',
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontWeight: 700,
                                                    fontSize: '16px',
                                                    textAlign: 'center',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_9',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
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
                                                htmlID: 'u_column_9',
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
                                    _meta: {
                                        htmlID: 'u_row_5',
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
                                                    text: '<span>@ Tất cả phải thuận theo &yacute; vua</span>',
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
                                                    textJson:
                                                        '{"root":{"children":[{"children":[{"detail":0,"format":0,"mode":"normal","text":"Liên hệ với chúng tôi","type":"extended-text","version":1}],"format":"","indent":0,"type":"paragraph","version":1,"textFormat":0}],"format":"","indent":0,"type":"root","version":1}}',
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
                                                    text: '<span>Thuốc l&agrave;o Ti&ecirc;n L&atilde;ng</span>',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'm4okIl5jXZ',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Thơm mồm</strong>',
                                                    containerPadding: '10px',
                                                    anchor: '',
                                                    headingType: 'h4',
                                                    fontSize: '15px',
                                                    textAlign: 'left',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_19',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    locked: false,
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'i-rdsPicsA',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Bổ Phổi<br></strong>',
                                                    containerPadding: '10px',
                                                    headingType: 'h4',
                                                    fontSize: '15px',
                                                    textAlign: 'left',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_20',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: '6Lt443mrOm',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Diệt Tr&ugrave;ng Lao</strong>',
                                                    containerPadding: '10px',
                                                    headingType: 'h4',
                                                    fontSize: '15px',
                                                    textAlign: 'left',
                                                    lineHeight: '140%',
                                                    linkStyle: {
                                                        inherit: true,
                                                        linkColor: '#0000ee',
                                                        linkHoverColor: '#0000ee',
                                                        linkUnderline: true,
                                                        linkHoverUnderline: true
                                                    },
                                                    displayCondition: null,
                                                    _styleGuide: null,
                                                    _meta: {
                                                        htmlID: 'u_content_heading_21',
                                                        htmlClassNames: 'u_content_heading'
                                                    },
                                                    selectable: true,
                                                    draggable: true,
                                                    duplicatable: true,
                                                    deletable: true,
                                                    hideable: true,
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
                                                    text: 'Trần Văn Quang',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'swIUOMuUvk',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Nguyễn Trọng Nghĩa<br></strong>',
                                                    containerPadding: '10px',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'zaH3bB30k0',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Phạm Ngọc Hiệp<br></strong>',
                                                    containerPadding: '10px',
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
                                                    _languages: {}
                                                }
                                            },
                                            {
                                                id: 'pTUSq9glQb',
                                                type: 'heading',
                                                values: {
                                                    text: '<strong>Nguyễn Việt Cường<br></strong>',
                                                    containerPadding: '10px',
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
