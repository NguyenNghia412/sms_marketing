import { ICreateHopTrucTuyen, IFindPagingHopTrucTuyen, IViewRowHopTrucTuyen } from '@/models/hopTrucTuyen.models';
import { IBaseResponse, IBaseResponsePaging } from '@/shared/models/request-paging.base.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HopTrucTuyenService {
  
  api = '/api/core/hop-truc-tuyen'
  http = inject(HttpClient)

  findPaging(query: IFindPagingHopTrucTuyen) {
    return this.http.get<IBaseResponsePaging<IViewRowHopTrucTuyen>>(this.api, {
      params: {...query}
    })
  }

  create(body: ICreateHopTrucTuyen) {
    return this.http.post<IBaseResponse>(this.api, body);
  }

}
