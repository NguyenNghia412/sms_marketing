using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.Auth.Dtos.User;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace thongbao.be.Controllers.Auth
{
    [Route("api/app/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;
        public UsersController(ILogger<BaseController> logger, IUsersService usersService) : base(logger)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Tạo user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Permission(PermissionKeys.UserAdd)]
        public async Task<ApiResponse> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                await _usersService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("")]
        [Permission(PermissionKeys.UserUpdate)]
        public async Task<ApiResponse> UpdateUser([FromBody] UpdateUserDto dto)
        {
            try
            {
                await _usersService.Update(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Tìm user phân trang
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet("")]
        [Permission(PermissionKeys.UserView)]
        public async Task<ApiResponse> Find([FromQuery] FindPagingUserDto dto)
        {
            try
            {
                var data = await _usersService.FindPaging(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Tìm user theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Permission(PermissionKeys.UserView)]
        public async Task<ApiResponse> GetById([FromRoute] string id)
        {
            try
            {
                var data = await _usersService.FindById(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Gán role cho user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("set-to-role")]
        [Permission(PermissionKeys.UserSetRoles)]
        public async Task<ApiResponse> SetToRole([FromBody] SetRoleForUserDto dto)
        {
            try
            {
                await _usersService.SetRoleForUser(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
