import { IFindPagingUser, IViewRowUser } from '@/models/user.models';
import { IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
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
}
