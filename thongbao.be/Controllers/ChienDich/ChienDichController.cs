using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.ChienDich
{
    [Route("api/core/chien-dich")]
    [ApiController]
    [Authorize]
    public class ChienDichController : BaseController
    {
        private readonly IChienDichService _chienDichService;

        public ChienDichController(ILogger<ChienDichController> logger, IChienDichService chienDichService) : base(logger)
        {
            _chienDichService = chienDichService;
        }

        [Permission(PermissionKeys.ChienDichView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingChienDichDto dto)
        {
            try
            {
                var data = _chienDichService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.ChienDichAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateChienDichDto dto)
        {
            try
            {
                _chienDichService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.ChienDichUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromQuery] int idChienDich, [FromBody] UpdateChienDichDto dto)
        {
            try
            {
                _chienDichService.Update(idChienDich, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.ChienDichDelete)]
        [HttpDelete("")]
        public ApiResponse Delete([FromQuery] int idChienDich)
        {
            try
            {
                _chienDichService.Delete(idChienDich);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("test")]
        public ApiResponse TestSendEmail()
        {
            try
            {
                _chienDichService.TestSendEmail();
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
