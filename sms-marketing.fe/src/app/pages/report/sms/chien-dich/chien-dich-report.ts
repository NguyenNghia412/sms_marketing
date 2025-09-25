import { DataTable } from "@/shared/components/data-table/data-table";
import { SharedImports } from "@/shared/import.shared";
import { Component } from "@angular/core";
import { BaseComponent } from "primeng/basecomponent";

@Component({
    selector: 'app-thong-ke-chien-dich',
    imports: [...SharedImports, DataTable],
    templateUrl: './chien-dich-report.html',
    styleUrl: './chien-dich-report.scss'

})
export class ChienDichReport extends BaseComponent{
    
}