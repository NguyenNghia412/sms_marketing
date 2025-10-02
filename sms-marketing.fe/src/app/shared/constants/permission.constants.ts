export class PermissionConstants {
    static Menu = "Menu.";
    static Function = "Function.";

    static MenuMarketing = this.Menu + "Marketing";
    static MenuMarketingSms = this.MenuMarketing + "_Sms";
    static MenuMarketingEmail = this.MenuMarketing + "_Email";
    static MenuMarketingZns = this.MenuMarketing + "_Zns";

    static MenuUserManagement = this.Menu + "UserManagement";
    static MenuUserManagementUser = this.MenuUserManagement + "_User";
    static MenuUserManagementRole = this.MenuUserManagement + "_User";

    static MenuContact = this.Menu + "Contact";
    static MenuTemplate = this.Menu + "Template";
    static MenuReport = this.Menu + "Report";
    static MenuTraoBang = this.Menu + "TraoBang";

    static CategoryUser = "QL User";
    static UserAdd = this.Function + "UserAdd";
    static UserUpdate = this.Function + "UserUpdate";
    static UserDelete = this.Function + "UserDelete";
    static UserView = this.Function + "UserView";
    static UserSetRoles = this.Function + "UserSetRoles";

    static CategoryRole = "QL Role";
    static RoleAdd = this.Function + "Add";
    static RoleUpdate = this.Function + "Update";
    static RoleDelete = this.Function + "Delete";
    static RoleView = this.Function + "View";

    static CategoryChienDich = "QL Chiến dịch";
    static ChienDichAdd = this.Function + "ChienDichAdd";
    static ChienDichUpdate = this.Function + "ChienDichUpdate";
    static ChienDichDelete = this.Function + "ChienDichDelete";
    static ChienDichView = this.Function + "ChienDichView";


    static CategoryHopTrucTuyen = "QL Họp trực tuyến";
    static HopTrucTuyenAdd = this.Function + "HopTrucTuyenAdd";
    static HopTrucTuyenUpdate = this.Function + "HopTrucTuyenUpdate";
    static HopTrucTuyenDelete = this.Function + "HopTrucTuyenDelete";
    static HopTrucTuyenView = this.Function + "HopTrucTuyenView";

    static CategoryDanhBa = "QL Danh bạ";
    static DanhBaAdd = this.Function + "DanhBaAdd";
    static DanhBaUpdate = this.Function + "DanhBaUpdate";
    static DanhBaDelete = this.Function + "DanhBaDelete";
    static DanhBaView = this.Function + "DanhBaView";
    static DanhBaImport = this.Function + "DanhBaImport";


    static CategoryToChuc = "QL Tổ chức";
    static ToChucAdd = this.Function + "ToChucAdd";
    static ToChucUpdate = this.Function + "ToChucUpdate";
    static ToChucDelete = this.Function + "ToChucDelete";
    static ToChucView = this.Function + "ToChucView";
}