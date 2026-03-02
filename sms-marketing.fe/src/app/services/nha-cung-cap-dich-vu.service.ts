import { IAddBrandNameToNhaCungCapDichVu, IAddUserToNhaCungCapDichVuDto, ICreateNhaCungCapDichVu, IDeleteBrandNameToNhaCungCapDichVuDto, IDropDownNhaCungCapDichVu, IUpdateNhaCungCapDichVu, IUpdateUserToNhaCungCapDichVuDto, IViewNhaCungCapDichVuById } from "@/models/nha-cung-cap-dich-vu.models";
import { IBaseResponseWithData } from "@/shared/models/request-paging.base.models";
import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class NhaCungCapDichVuService {
    api = '/api/core/nha-cung-cap-dich-vu';
    http = inject(HttpClient);

    findPaging(query: any) {
        const params: any = {
            pageNumber: query.pageNumber,
            pageSize: query.pageSize,
            keyword: query.keyword || ''
        };

        return this.http.get<any>(`${this.api}`, { params });
    }

    create(body: ICreateNhaCungCapDichVu) {
        return this.http.post<IBaseResponseWithData<ICreateNhaCungCapDichVu>>(`${this.api}`, body);
    }

    update(body: IUpdateNhaCungCapDichVu) {
        return this.http.put<IBaseResponseWithData<IUpdateNhaCungCapDichVu>>(`${this.api}`, body);
    }

    delete(id: number) {
        return this.http.delete<IBaseResponseWithData<any>>(`${this.api}/${id}`);
    }

    getById(id: number) {
        return this.http.get<IBaseResponseWithData<IViewNhaCungCapDichVuById>>(`${this.api}/${id}`);
    }

    getDropdown() {
        return this.http.get<IBaseResponseWithData<IDropDownNhaCungCapDichVu[]>>(`${this.api}/dropdown`);
    }
    addBrandNameToNhaCungCapDichVu(body: IAddBrandNameToNhaCungCapDichVu) {
        return this.http.post<IBaseResponseWithData<any>>(`${this.api}/brand-name`, body);
    }

    deleteBrandNameToNhaCungCapDichVu(body: IDeleteBrandNameToNhaCungCapDichVuDto) {
        return this.http.delete<IBaseResponseWithData<any>>(`${this.api}/brand-name`, { body });
    }

    addUserToNhaCungCapDichVu(body: IAddUserToNhaCungCapDichVuDto) {
        return this.http.post<IBaseResponseWithData<any>>(`${this.api}/user`, body);
    }

    updateUserToNhaCungCapDichVu(body: IUpdateUserToNhaCungCapDichVuDto) {
        return this.http.put<IBaseResponseWithData<any>>(`${this.api}/user`, body);
    }

    deleteUserToNhaCungCapDichVu(idUserNhaCungCapDichVu: number) {
        return this.http.delete<IBaseResponseWithData<any>>(`${this.api}/${idUserNhaCungCapDichVu}/user`);
    }

    findPagingUserNhaCungCapDichVu(query: any) {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/user`, { params: query });
    }

    findById(idUserNhaCungCapDichVu: number) {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/${idUserNhaCungCapDichVu}/user`);
    }

    getListBrandName(idNhaCungCapDichVu: number) {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/${idNhaCungCapDichVu}/drop-down-brand-names`);
    }
    getListDropDownUserNhaCungCapDichVu(idNhaCungCapDichVu: number) {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/${idNhaCungCapDichVu}/drop-down-user`);
    }
    getListBrandNameByCurrentUser() {
        return this.http.get<IBaseResponseWithData<any>>(`${this.api}/drop-down-brand-names-by-current-user`);
    }

}