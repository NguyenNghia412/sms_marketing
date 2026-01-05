using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.Controllers.ChienDich;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.Config
{
    [Route("api/core/nha-mang")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NhaMangController : BaseController
    {
        private readonly INhaMangService _nhaMangService;

        public NhaMangController(ILogger<NhaMangController> logger, INhaMangService nhaMangService) : base(logger)
        {
            _nhaMangService = nhaMangService;
        }

        [Permission(PermissionKeys.NhaMangAdd)]
        [HttpPost("")]
        public ApiResponse AddNhaMang(AddNhaMangDto dto)
        {
            try
            {
                _nhaMangService.AddNhaMang(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.NhaMangUpdate)]
        [HttpPut("")]
        public ApiResponse UpdateNhaMang(UpdateNhaMangDto dto)
        {
            try
            {
                _nhaMangService.UpdateNhaMang(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaMangDelete)]
        [HttpDelete("{id}")]
        public ApiResponse DeleteNhaMang([FromRoute] int id)
        {
            try
            {
                _nhaMangService.DeleteNhaMang(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaMangView)]
        [HttpGet("")]
        public ApiResponse FindPaging([FromQuery] FindPagingNhaMangDto dto)
        {
            try
            {
                var data = _nhaMangService.FindPaging(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.NhaMangView)]
        [HttpGet("by-id/{id}")]
        public ApiResponse GetById([FromRoute] int id)
        {
            try
            {
                var data = _nhaMangService.GetById(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
