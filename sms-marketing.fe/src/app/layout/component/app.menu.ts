import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppMenuitem } from './app.menuitem';
import { IAppMenuItem } from '../model/app-menu-item.model';
import { SharedService } from '@/services/shared.service';
import { PermissionConstants } from '@/shared/constants/permission.constants';

@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, AppMenuitem, RouterModule],
    template: `<ul class="layout-menu">
        <ng-container *ngFor="let item of model; let i = index">
            <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true"></li>
            <li *ngIf="item.separator" class="menu-separator"></li>
        </ng-container>
    </ul> `
})
export class AppMenu {

    _sharedService = inject(SharedService);

    model: IAppMenuItem[] = [];

    ngOnInit() {
        console.log(this._sharedService.roles)
        this.model = [
            // {
            //     items:[
            //         {
            //             label: 'Dashboard',
            //             routerLink: ['/']
            //         }
            //     ]
            // },
            {
                items: [
                    {
                        label: 'Kênh marketing',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuMarketing),
                        items: [
                            { label: 'SMS', heroIcon: 'heroChatBubbleBottomCenterText', routerLink: ['/channel/sms'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingSms) },
                            { label: 'Email', heroIcon: 'heroEnvelope', routerLink: ['/channel/email'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingEmail) },
                            { label: 'ZNS', heroIcon: 'heroSquare3Stack3d', routerLink: ['/channel/zns'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingZns) }
                        ],
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuMarketing),
            },
            {
                items: [
                    {
                        label: 'Danh bạ',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuContact),
                        routerLink: ['/danh-ba/ds']
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuContact),
            },
            {
                items: [
                    {
                        label: 'Templates',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuTemplate),
                        routerLink: ['/template/mau-nd']
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuTemplate),

            },
            {
                items: [
                    {
                        label: 'Thống kê',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuReport),
                        items: [
                            { label: 'SMS', heroIcon: 'heroChatBubbleBottomCenterText', routerLink: ['/report/chien-dich-report'] },
                        ]
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuReport),

            },
            {
                items: [
                    {
                        label: 'Trao bằng',
                        visible: true,
                        items: [
                            {
                                label: 'Cấu hình',
                                visible: true,
                                items: [
                                    {
                                        label: 'Kế hoạch',
                                        visible: true,
                                        routerLink: ['/trao-bang/config/plan']
                                    },
                                    {
                                        label: 'Khoa',
                                        visible: true,
                                        routerLink: ['/trao-bang/config/sub-plan']
                                    },
                                    {
                                        label: 'SV nhận bằng',
                                        visible: true,
                                        routerLink: ['/trao-bang/config/sv']
                                    },
                                ]
                            },
                        ]
                    }
                ],
                visible: true,
            },
            {
                items: [
                    {
                        label: 'QL Tài khoản',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagement),
                        items: [
                            {
                                label: 'Người dùng',
                                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagementUser),
                                heroIcon: 'heroUser',
                                routerLink: ['/user-management/user']
                            },
                            {
                                label: 'Vai trò',
                                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagementRole),
                                heroIcon: 'heroUserGroup',
                                routerLink: ['/user-management/role']
                            }
                        ]
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagement),
            },
            // {
            //     items: [
            //         {
            //             label: 'MS Teams',
            //             items: [
            //                 { label: 'Họp trực tuyến', heroIcon: 'heroRadio', routerLink: ['/meeting/hop-truc-tuyen'] },
            //             ]
            //         }
            //     ]
            // },
            // {
            //     items: [
            //         {
            //             label: 'Tài khoản',
            //             routerLink: ['/contacts']
            //         }
            //     ]
            // },
            // {
            //     items: [
            //         {
            //             label: 'Audit log',
            //             routerLink: ['/contacts']
            //         }
            //     ]
            // },
        ];
    }
}
