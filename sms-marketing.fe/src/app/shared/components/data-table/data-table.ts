import { CellViewTypes } from '@/shared/constants/data-table.constants';
import { IColumn } from '@/shared/models/data-table.models';
import { CurrencyPipe, DatePipe, NgClass } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';

@Component({
    selector: 'app-data-table',
    imports: [TableModule, PaginatorModule, CurrencyPipe, DatePipe, IconFieldModule, InputIconModule, NgClass],
    templateUrl: './data-table.html',
    styleUrl: './data-table.scss'
})
export class DataTable {
    columns = input.required<IColumn[]>();
    data = input.required<any[]>();
    pageSize = input<number>(10);
    pageNumber = input<number>(1);
    loading = input<boolean>(false);
    
    cellViewTypes = CellViewTypes;
    
    sanitizer = inject(DomSanitizer)

    get<T>(obj: any, path: string): T | undefined {
        return path.split('.').reduce((acc, key) => acc && acc[key], obj);
    }
}
