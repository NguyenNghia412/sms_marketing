import { IBaseRequestPaging } from "@/shared/models/request-paging.base.models";

export interface IViewRowUser {
  id?: string;
  userName?: string;
  email?: string;
  phoneNumber?: string | null;
  fullName?: string;
  emailConfirmed?: boolean;
  phoneNumberConfirmed?: boolean;
  createdAt?: string; // or Date if you plan to parse it
  roles?: string[];
}

export interface IFindPagingUser extends IBaseRequestPaging {}