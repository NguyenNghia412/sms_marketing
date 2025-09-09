using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.Auth
{
    [Route("api/app/permissions")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PermissionController : BaseController
    {
        private readonly IPermissionsService _permissionsService;
        public PermissionController(ILogger<BaseController> logger, IPermissionsService permissionsService) : base(logger)
        {
            _permissionsService = permissionsService;
        }

        /// <summary>
        /// Lấy toàn bộ permission
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Permission(PermissionKeys.RoleView)]
        public ApiResponse GetAll()
        {
            try
            {
                var data = _permissionsService.GetAllPermissions();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
