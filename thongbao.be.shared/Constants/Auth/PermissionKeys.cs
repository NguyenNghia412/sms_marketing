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

        public const string CategoryUser = "QL User";
        public const string UserAdd = Function + "UserAdd";
        public const string UserUpdate = Function + "UserUpdate";
        public const string UserDelete = Function + "UserDelete";
        public const string UserView = Function + "UserView";

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

        public static readonly (string Key, string Name, string Category)[] All =
        {
            (UserAdd, "Thêm user", CategoryUser),
            (UserUpdate, "Cập nhật User" , CategoryUser),
            (UserDelete, "Xoá User" , CategoryUser),
            (UserView, "Xem User" , CategoryUser),

            (RoleAdd, "Thêm Role", CategoryRole),
            (RoleUpdate, "Cập nhật Role", CategoryRole),
            (RoleDelete, "Xoá Role", CategoryRole),
            (RoleView, "Xem Role", CategoryRole),

            (ChienDichAdd, "Thêm Role", CategoryChienDich),
            (ChienDichUpdate, "Cập nhật Role", CategoryChienDich),
            (ChienDichDelete, "Xoá Role", CategoryChienDich),
            (ChienDichView, "Xem Role", CategoryRole),

        };
    }
}
