import { Utils } from "@/shared/utils";
import { HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { environment } from "src/environments/environment";

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {

  const token = Utils.getAccessToken();
  const baseUrl = environment.baseUrl;
  // Clone the request to add the authentication header.
  const newReq = req.clone({
    headers: req.headers.append('Authorization', `Bearer ${token}`),
    url: req.url.startsWith('http') ? req.url : `${baseUrl}${req.url}`
  },);
  return next(newReq);
}