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
        public const string MenuTraoBang = Menu + "TraoBang";

        public const string MenuTraoBangCauHinh = MenuTraoBang + "_CauHinh";
        public const string MenuTraoBangCauHinhChuongTrinh = MenuTraoBangCauHinh + "_ChuongTrinh";
        public const string MenuTraoBangCauHinhKhoa = MenuTraoBangCauHinh + "_Khoa";
        public const string MenuTraoBangCauHinhSinhVienNhanBang = MenuTraoBangCauHinh + "_SinhVienNhanBang";
        public const string MenuTraoBangQuetQr = MenuTraoBang + "_QuetQr";
        public const string MenuTraoBangMc = MenuTraoBang + "_Mc";

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

        public const string CategoryMauNoiDung = "QL Mẫu Nội Dung";
        public const string MauNoiDungAdd = Function + "MauNoiDungAdd";
        public const string MauNoiDungUpdate = Function + "MauNoiDungUpdate";
        public const string MauNoiDungDelete = Function + "MauNoiDungDelete";
        public const string MauNoiDungView = Function + "MauNoiDungView";

        public const string GuiTinNhanAdd = Function + "GuiTinNhanAdd";

        public const string CategoryPlan = "QL Plan";
        public const string PlanAdd = Function + "PlanAdd";
        public const string PlanUpdate = Function + "PlanUpdate";
        public const string PlanDelete = Function + "PlanDelete";
        public const string PlanView = Function + "PlanView";

        public const string CategorySubPlan = "QL SubPlan";
        public const string SubPlanAdd = Function + "SubPlanAdd";
        public const string SubPlanUpdate = Function + "SubPlanUpdate";
        public const string SubPlanDelete = Function + "SubPlanDelete";
        public const string SubPlanView = Function + "SubPlanView";


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
            (MenuTraoBang, "Menu Trao Bằng", "Menu"),

            (MenuTraoBangCauHinh, "Menu Trao Bằng Cấu hình", "Menu"),
            (MenuTraoBangCauHinhChuongTrinh, "Menu Trao Bằng Cấu hình Chương trình", "Menu"),
            (MenuTraoBangCauHinhKhoa, "Menu Trao Bằng Cấu hình Khoa", "Menu"),
            (MenuTraoBangCauHinhSinhVienNhanBang, "Menu Trao Bằng Cấu hình Sinh viên", "Menu"),
            (MenuTraoBangQuetQr, "Menu Trao Bằng Quét QR", "Menu"),
            (MenuTraoBangMc, "Menu Trao Bằng Điều khiển", "Menu"),

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

            (MauNoiDungAdd, "Thêm Mẫu Nội Dung ", CategoryMauNoiDung),
            (MauNoiDungUpdate, "Cập nhật Mẫu Nội Dung", CategoryMauNoiDung),
            (MauNoiDungDelete, "Xoá Mẫu Nội Dung", CategoryMauNoiDung),
            (MauNoiDungView, "Xem Mẫu Nội Dung", CategoryMauNoiDung),

            (PlanAdd, "Thêm Plan ", CategoryPlan),
            (PlanUpdate, "Cập nhật Plan", CategoryPlan),
            (PlanDelete, "Xoá Plan", CategoryPlan),
            (PlanView, "Xem Plan", CategoryPlan),

            (SubPlanAdd, "Thêm SubPlan ", CategorySubPlan),
            (SubPlanUpdate, "Cập nhật SubPlan", CategorySubPlan),
            (SubPlanDelete, "Xoá SubPlan", CategorySubPlan),
            (SubPlanView, "Xem SubPlan", CategorySubPlan),

        };
    }
}
