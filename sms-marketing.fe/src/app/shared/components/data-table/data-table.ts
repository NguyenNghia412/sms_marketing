import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn, ICustomEmit } from '@/shared/models/data-table.models';
import { CommonModule, CurrencyPipe, DatePipe, NgClass, NgComponentOutlet } from '@angular/common';
import { Component, EventEmitter, inject, InjectionToken, Injector, input, OnInit, Output } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';

export const TBL_CUSTOM_COMP_EMIT = new InjectionToken<EventEmitter<any>>('TBL_CUSTOM_COMP_EMIT');

@Component({
    selector: 'app-data-table',
    imports: [TableModule, PaginatorModule, CurrencyPipe, DatePipe, IconFieldModule, InputIconModule, NgClass, NgComponentOutlet, CommonModule],
    templateUrl: './data-table.html',
    styleUrl: './data-table.scss'
})
export class DataTable implements OnInit {
    injector = inject(Injector);

    columns = input.required<IColumn[]>();
    data = input.required<any[]>();
    pageSize = input<number>(10);
    pageNumber = input<number>(1);
    total = input<number>(100);
    loading = input<boolean>(false);

    @Output() onPageChanged = new EventEmitter<any>();
    @Output() customEmit = new EventEmitter<any>();
    @Output() onCustomComp = new EventEmitter<any>();

    cellViewTypes = CellViewTypes;
    sanitizer = inject(DomSanitizer);
    customInjector!: Injector;

    ngOnInit(): void {
        this.customInjector = Injector.create({
            providers: [
                {
                    provide: TBL_CUSTOM_COMP_EMIT,
                    useValue: this.customEmit
                }
            ],
            parent: this.injector
        });
        
        this.customEmit.subscribe((data) => this.onTblCustomCompEmit(data));
    }

    onTblCustomCompEmit(data: any) {
        this.onCustomComp.emit(data);
    }

    get<T>(obj: any, path: string): T | undefined {
        return path.split('.').reduce((acc, key) => acc && acc[key], obj);
    }

    onPage($event: any) {
        this.onPageChanged.emit($event);
    }
    
     onCellClick(col: IColumn, row: any) {
        if (col.clickable) {
            this.onCustomComp.emit({
                type: 'cellClick',
                field: col.field,
                data: row // Đổi từ rowData thành data để consistent với các emit khác
            });
        }
    }
    isCellClickable(col: IColumn): boolean {
        return col.clickable === true;
    }

}