import { ICreateUserCredits, IFindPagingUserCredits, IFindPagingUserCreditsForUser, IUpdateUserCredits, IViewUserCreditsByUser } from "@/models/user-credits.models";
import { IBaseResponseWithData } from "@/shared/models/request-paging.base.models";
import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class UserCreditsService {
    api = '/api/core/user-credits';
    http = inject(HttpClient);

    findPaging(query: IFindPagingUserCredits){
        const params: any ={
            pageNumber: query.pageNumber,
            pageSize: query.pageSize,
            keyword: query.keyword || ''
        };

        return this.http.get<IBaseResponseWithData<any>>(`${this.api}`, {
            params
        });
    }

    createUserCredits(body: ICreateUserCredits){
        return this.http.post<IBaseResponseWithData<any>>(`${this.api}`, body);
    }

    updateUserCredits(body: IUpdateUserCredits){
        return this.http.put<IBaseResponseWithData<any>>(`${this.api}`, body);
    }

    deleteUserCredits(id: number)
    {
        return this.http.delete<IBaseResponseWithData<any>>(`${this.api}/${id}`);
    }

    getById(id: number)
    {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/by-id/${id}`);
    }

    getDonVi(id: number)
    {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/${id}/don-vi`);
    }

    getByUserId(query: IFindPagingUserCreditsForUser)
    {
        const params: any ={
            pageNumber: query.pageNumber,
            pageSize: query.pageSize,
            keyword: query.keyword || '',
            ...(query.userId ? { userId: query.userId } : {})
        }
        return this.http.get<IBaseResponseWithData<any>> (`${this.api}/user`, {params});
    }
    getCurrentUserCredits()
    {
        return this.http.get<IBaseResponseWithData<IViewUserCreditsByUser>>(`${this.api}/user-credits`);
    }


    
}