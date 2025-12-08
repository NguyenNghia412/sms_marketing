using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.Constants.Auth
{
    public static class PermissionKeys
    {
        public const string Menu = "Menu.";
        public const string Function = "Function.";

        public const string MenuMarketing = Menu + "Marketing";
        public const string MenuMarketingSms = MenuMarketing + "_Sms";
        public const string MenuMarketingEmail = MenuMarketing + "_Email";
        public const string MenuMarketingZns = MenuMarketing + "_Zns";

        public const string MenuUserManagement = Menu + "UserManagement";
        public const string MenuUserManagementUser = MenuUserManagement + "_User";
        public const string MenuUserManagementRole = MenuUserManagement + "_User";

        public const string MenuContact = Menu + "Contact";
        public const string MenuTemplate = Menu + "Template";
        public const string MenuReport = Menu + "Report";

        public const string CategoryUser = "QL User";
        public const string UserAdd = Function + "UserAdd";
        public const string UserUpdate = Function + "UserUpdate";
        public const string UserDelete = Function + "UserDelete";
        public const string UserView = Function + "UserView";
        public const string UserSetRoles = Function + "UserSetRoles";

        public const string CategoryRole = "QL Role";
        public const string RoleAdd = Function + "Add";
        public const string RoleUpdate = Function + "Update";
        public const string RoleDelete = Function + "Delete";
        public const string RoleView = Function + "View";

        public const string CategoryChienDich = "QL Chiến dịch";
        public const string ChienDichAdd = Function + "ChienDichAdd";
        public const string ChienDichUpdate = Function + "ChienDichUpdate";
        public const string ChienDichDelete = Function + "ChienDichDelete";
        public const string ChienDichView = Function + "ChienDichView";
        public const string GuiTinNhanReportView = Function + "GuiTinNhanReportView";

        public const string CategoryHopTrucTuyen = "QL Họp trực tuyến";
        public const string HopTrucTuyenAdd = Function + "HopTrucTuyenAdd";
        public const string HopTrucTuyenUpdate = Function + "HopTrucTuyenUpdate";
        public const string HopTrucTuyenDelete = Function + "HopTrucTuyenDelete";
        public const string HopTrucTuyenView = Function + "HopTrucTuyenView";

        public const string CategoryDanhBa = "QL Danh bạ";
        public const string DanhBaAdd = Function + "DanhBaAdd";
        public const string DanhBaUpdate = Function + "DanhBaUpdate";
        public const string DanhBaDelete = Function + "DanhBaDelete";
        public const string DanhBaView = Function + "DanhBaView";
        public const string DanhBaImport = Function + "DanhBaImport";

        public const string CategoryToChuc = "QL Tổ chức";
        public const string ToChucAdd = Function + "ToChucAdd";
        public const string ToChucUpdate = Function + "ToChucUpdate";
        public const string ToChucDelete = Function + "ToChucDelete";
        public const string ToChucView = Function + "ToChucView";

        public const string CategoryMauNoiDungSms = "QL Mẫu Nội Dung Sms";
        public const string MauNoiDungSmsAdd = Function + "MauNoiDungAddSms";
        public const string MauNoiDungSmsUpdate = Function + "MauNoiDungUpdateSms";
        public const string MauNoiDungSmsDelete = Function + "MauNoiDungDeleteSms";
        public const string MauNoiDungSmsView = Function + "MauNoiDungViewSms";


        public const string CategoryMauNoiDungEmail = "QL Mẫu Nội Dung Email";
        public const string MauNoiDungEmailAdd = Function + "MauNoiDungEmailAdd";
        public const string MauNoiDungEmailUpdate = Function + "MauNoiDungEmailUpdate";
        public const string MauNoiDungEmailDelete = Function + "MauNoiDungEmailDelete";
        public const string MauNoiDungEmailView = Function + "MauNoiDungEmailView";


        public const string CategoryGuiTinNhan = "QL Gửi tin nhắn";
        public const string GuiTinNhanAdd = Function + "GuiTinNhanAdd";
        




        public const string CategoryStringeeProfile = "QL Stringee Profile";
        public const string StringeeProfileView = Function + "StringeeProfileView";


        public static readonly (string Key, string Name, string Category)[] All =
        {
            (MenuMarketing, "Menu Marketing", "Menu"),
            (MenuMarketingSms, "Menu Marketing - SMS", "Menu"),
            (MenuMarketingEmail, "Menu Marketing - Email", "Menu"),
            (MenuMarketingZns, "Menu Marketing - ZNS", "Menu"),

            (MenuUserManagement, "Menu Quản lý User", "Menu"),
            (MenuUserManagementUser, "Menu Quản lý User - User", "Menu"),
            (MenuUserManagementRole, "Menu Quản lý User - Role", "Menu"),

            (MenuContact, "Menu Danh bạ", "Menu"),
            (MenuTemplate, "Menu Template", "Menu"),
            (MenuReport, "Menu Báo cáo", "Menu"),


            (UserAdd, "Thêm user", CategoryUser),
            (UserUpdate, "Cập nhật User" , CategoryUser),
            (UserDelete, "Xoá User" , CategoryUser),
            (UserView, "Xem User" , CategoryUser),
            (UserSetRoles, "Gán role cho User" , CategoryUser),

            (RoleAdd, "Thêm Role", CategoryRole),
            (RoleUpdate, "Cập nhật Role", CategoryRole),
            (RoleDelete, "Xoá Role", CategoryRole),
            (RoleView, "Xem Role", CategoryRole),

            (ChienDichAdd, "Thêm Chiến dịch", CategoryChienDich),
            (ChienDichUpdate, "Cập nhật Chiến dịch", CategoryChienDich),
            (ChienDichDelete, "Xoá Chiến dịch", CategoryChienDich),
            (ChienDichView, "Xem Chiến dịch", CategoryChienDich),
            (GuiTinNhanReportView,"Xem báo cáo chiến dịch gửi Sms", CategoryChienDich),

            (HopTrucTuyenAdd, "Thêm Cuộc họp trực tuyến ", CategoryHopTrucTuyen),
            (HopTrucTuyenUpdate, "Cập nhật Cuộc họp trực tuyến", CategoryHopTrucTuyen),
            (HopTrucTuyenDelete, "Xoá Cuộc họp trực tuyến", CategoryHopTrucTuyen),
            (HopTrucTuyenView, "Xem Cuộc họp trực tuyến", CategoryHopTrucTuyen),

            (DanhBaAdd, "Thêm Danh bạ ", CategoryDanhBa),
            (DanhBaUpdate, "Cập nhật Danh bạ", CategoryDanhBa),
            (DanhBaDelete, "Xoá Danh bạ", CategoryDanhBa),
            (DanhBaView, "Xem Danh bạ", CategoryDanhBa),
            (DanhBaImport,"Import Danh Bạ",DanhBaImport),

            (ToChucAdd, "Thêm Tổ chức ", CategoryToChuc),
            (ToChucUpdate, "Cập nhật Tổ chức", CategoryToChuc),
            (ToChucDelete, "Xoá Tổ chức", CategoryToChuc),
            (ToChucView, "Xem Tổ chức", CategoryToChuc),

            (MauNoiDungSmsAdd, "Thêm Mẫu Nội Dung Sms", CategoryMauNoiDungSms),
            (MauNoiDungSmsUpdate, "Cập nhật Mẫu Nội Dung Sms", CategoryMauNoiDungSms),
            (MauNoiDungSmsDelete, "Xoá Mẫu Nội Dung Sms", CategoryMauNoiDungSms),
            (MauNoiDungSmsView, "Xem Mẫu Nội Dung Sms", CategoryMauNoiDungSms),


            (MauNoiDungEmailAdd, "Thêm Mẫu Nội Dung Email ", CategoryMauNoiDungEmail),
            (MauNoiDungEmailUpdate, "Cập nhật Mẫu Nội Dung Email", CategoryMauNoiDungEmail),
            (MauNoiDungEmailDelete, "Xoá Mẫu Nội Dung Email", CategoryMauNoiDungEmail),
            (MauNoiDungEmailView, "Xem Mẫu Nội Dung Email", CategoryMauNoiDungEmail),




            (GuiTinNhanAdd,"Gửi tin nhắn", CategoryGuiTinNhan),

            (StringeeProfileView, "Xem Stringee Profile", CategoryStringeeProfile)


        };
    }
}
