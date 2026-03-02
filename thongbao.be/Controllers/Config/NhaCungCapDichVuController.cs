using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using thongbao.be.application.Config.Dtos.NhaCungCapDichVu;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.Config
{
    [Route("api/core/nha-cung-cap-dich-vu")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NhaCungCapDichVuController : BaseController
    {
        private readonly INhaCungCapDichVuService _nhaCungCapDichVuService;
        public NhaCungCapDichVuController(ILogger<NhaCungCapDichVuController> logger, INhaCungCapDichVuService nhaCungCapDichVuService) : base(logger)
        {
            _nhaCungCapDichVuService = nhaCungCapDichVuService;
        }


        [Permission(PermissionKeys.NhaCungCapDichVuAdd)]
        [HttpPost("")]
        public ApiResponse AddNhaCungCapDichVu(CreateNhaCungCapDichVuDto dto)
        {
            try
            {
                _nhaCungCapDichVuService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuUpdate)]
        [HttpPut("")]
        public ApiResponse UpdateNhaCungCapDichVu(UpdateNhaCungCapDichVuDto dto)
        {
            try
            {
                _nhaCungCapDichVuService.Update(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.NhaCungCapDichVuDelete)]
        [HttpDelete("{id}")]
        public ApiResponse DeleteNhaCungCapDichVu([FromRoute] int id)
        {
            try
            {
                _nhaCungCapDichVuService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("")]
        public ApiResponse FindPaging(FindPagingNhaCungCapDichVuDto dto)
        {
            try
            {
                var result = _nhaCungCapDichVuService.FindPaging(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("{id}")]
        public ApiResponse GetById([FromRoute] int id)
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetById(id);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("dropdown")]
        public ApiResponse GetDropDownNhaCungCapDichVu()
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetDropDownNhaCungCapDichVu();
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuAdd)]
        [HttpPost("brand-name")]
        public ApiResponse AddBrandNameToNhaCungCapDichVu(AddBrandNameToNhaCungCapDichVuDto dto)
        {
            try
            {
                _nhaCungCapDichVuService.AddBrandNameToNhaCungCapDichVu(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }

        }

        [Permission(PermissionKeys.NhaCungCapDichVuDelete)]
        [HttpDelete("brand-name")]
        public ApiResponse DeleteBrandNameToNhaCungCapDichVu([FromBody] DeleteBrandNameToNhaCungCapDichVuDto dto)
        {
            try
            {
                _nhaCungCapDichVuService.DeleteBrandNameToNhaCungCapDichVu(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuAdd)]
        [HttpPost("user")]
        public async Task<ApiResponse> AddUserToNhaCungCapDichVu(AddUserToNhaCungCapDichVuDto dto)
        {
            try
            {
                await _nhaCungCapDichVuService.AddUserToNhaCungCapDichVu(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuUpdate)]
        [HttpPut("user")]
        public ApiResponse UpdateUserToNhaCungCapDichVu(UpdateUserToNhaCungCapDichVuDto dto)
        {
            try
            {
                _nhaCungCapDichVuService.UpdateUserToNhaCungCapDichVu(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuDelete)]
        [HttpDelete("{idUserNhaCungCapDichVu}/user")]
        public ApiResponse DeleteUserToNhaCungCapDichVu([FromRoute] int idUserNhaCungCapDichVu)
        {
            try
            {
                _nhaCungCapDichVuService.DeleteUserToNhaCungCapDichVu(idUserNhaCungCapDichVu);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("user")]
        public ApiResponse FindPagingUserNhaCungCapDichVu([FromQuery] FindPagingUserToNhaCungCapDichVuDto dto)
        {
            try
            {
                var result = _nhaCungCapDichVuService.FindPagingUserNhaCungCapDichVu(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("{idUserNhaCungCapDichVu}/user")]
        public ApiResponse FindById([FromRoute] int idUserNhaCungCapDichVu)
        {
            try
            {
                var result = _nhaCungCapDichVuService.FindById(idUserNhaCungCapDichVu);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("{idNhaCungCapDichVu}/drop-down-brand-names")]
        public ApiResponse GetListBrandName([FromRoute] int idNhaCungCapDichVu)
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetListBrandName(idNhaCungCapDichVu);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        //[Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("drop-down-brand-names-by-current-user")]
        public ApiResponse GetListBrandNameByCurrentUser()
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetListBrandNameByCurrentUser();
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("{idNhaCungCapDichVu}/drop-down-user")]
        public ApiResponse GetListDropDownUserNhaCungCapDichVu([FromRoute] int idNhaCungCapDichVu)
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetListDropDownUserNhaCungCapDichVu(idNhaCungCapDichVu);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaCungCapDichVuView)]
        [HttpGet("drop-down-user-su-dung-dich-vu")]
        public ApiResponse GetDropDownListUserSuDungDichVu()
        {
            try
            {
                var result = _nhaCungCapDichVuService.GetDropDownListUserSuDungDichVu();
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
