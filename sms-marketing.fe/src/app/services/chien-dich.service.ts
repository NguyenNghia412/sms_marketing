import { IFindPagingChienDich } from '@/models/sms.models';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ChienDichService {
  
  api = '/api/core/chien-dich'
  http = inject(HttpClient)

  findPaging(query: IFindPagingChienDich) {
    return this.http.get(this.api, {
      params: {...query}
    })
  }
}
