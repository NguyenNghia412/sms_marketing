import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn } from '@/shared/models/data-table.models';
import { CommonModule, DatePipe, NgClass, NgComponentOutlet } from '@angular/common';
import { Component, EventEmitter, inject, InjectionToken, Injector, input, OnInit, Output, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Toast } from 'primeng/toast';

export const TBL_CUSTOM_COMP_EMIT = new InjectionToken<EventEmitter<any>>('TBL_CUSTOM_COMP_EMIT');

@Component({
    selector: 'app-data-table',
    imports: [TableModule, PaginatorModule, DatePipe, IconFieldModule, InputIconModule, NgClass, NgComponentOutlet, CommonModule, TagModule],
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
    @Output() onError = new EventEmitter<any>();

    cellViewTypes = CellViewTypes;
    sanitizer = inject(DomSanitizer);
    customInjector!: Injector;

     @ViewChild(Paginator) paginator!: Paginator;

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
        if (col.clickable || col.cellViewType === CellViewTypes.CHECKBOX) {
            this.onCustomComp.emit({
                type: 'cellClick',
                field: col.field,
                data: row 
            });
        }
    }

    isCellClickable(col: IColumn): boolean {
        return col.clickable === true;
    }

    formatVND(value: number | string | null | undefined): string {
        if (value == null || value === '' || value === undefined) return '';
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    }

    getIndexValue( rowIndex:number):number{
        const currentPage = this.pageNumber();
        const pageSize = this.pageSize();
        return (currentPage * pageSize) - (pageSize - rowIndex) + 1;
    }

    goToPageNumber( pageNumber:number):void{
        const totalPages = Math.ceil(this.total()/this.pageSize());
        
        if(pageNumber < 1 || pageNumber > totalPages){
            this.onError.emit(`Số trang không hợp lệ! Vui lòng nhập từ 1 đến ${totalPages}`);
            return;
        }

        const first = (pageNumber - 1)* this.pageSize();

         if (this.paginator) {
            this.paginator.changePage(pageNumber - 1);
        }


        this.onPageChanged.emit({
            first: first,
            rows: this.pageSize(),
            page: pageNumber - 1,
            pageCount : totalPages
        })
    }
}
