import { MenuItem } from "primeng/api";

export interface IAppMenuItem extends MenuItem {
  heroIcon?: string;
  permission?: string;
  items?: IAppMenuItem[];
}