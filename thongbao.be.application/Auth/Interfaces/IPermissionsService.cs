using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Auth.Dtos.Permission;

namespace thongbao.be.application.Auth.Interfaces
{
    public interface IPermissionsService
    {
        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <returns></returns>
        public List<ViewPermissionDto> GetAllPermissions();
    }
}
