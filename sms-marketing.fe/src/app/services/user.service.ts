import { ICreateUser, IFindPagingUser, IUpdateUser, IViewRowUser } from '@/models/user.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    api = '/api/app/users';
    http = inject(HttpClient);

    findPaging(query: IFindPagingUser) {
        return this.http.get<IBaseResponsePaging<IViewRowUser>>(this.api, {
            params: { ...query }
        });
    }

    create(body: ICreateUser) {
        return this.http.post<IBaseResponse>(`${this.api}`, body);
    }

    update(body: IUpdateUser) {
        return this.http.put<IBaseResponse>(`${this.api}`, body);
    }
}
