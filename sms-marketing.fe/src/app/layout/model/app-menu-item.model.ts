import { MenuItem } from "primeng/api";

export interface IAppMenuItem extends MenuItem {
  heroIcon?: string;
  items?: IAppMenuItem[];
}