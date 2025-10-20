import { IResponseProfileStringee } from "@/models/profile-stringee.models";
import { IBaseResponse, IBaseResponseWithData } from "@/shared/models/request-paging.base.models";
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";


@Injectable({
    providedIn: 'root'
})
export class ProfileStringeeService {
    api = '/api/core/profile-stringee';
    http = inject(HttpClient);
    getProfileStringee() {
        return this.http.get<IBaseResponseWithData<IResponseProfileStringee>>(`${this.api}`);
    }
}