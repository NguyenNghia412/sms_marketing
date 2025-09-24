using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Implements;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.GuiTinNhan
{
    [Route("api/core/report-sms")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GuiTinNhanLogController:BaseController
    {
        private readonly IGuiTinNhanLogService _guiTinNhanLogService;
        private readonly ILogger<GuiTinNhanLogController> _logger;

        public GuiTinNhanLogController(
          ILogger<GuiTinNhanLogController> logger,
          IGuiTinNhanLogService guiTinNhanLogService) : base(logger)
        {
            _guiTinNhanLogService = guiTinNhanLogService;
            _logger = logger;
        }

        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpGet("chien-dich")]
        public ApiResponse FindChienDichLog([FromQuery] FindPagingChienDichLogDto dto)
        {
            try
            {
                var data = _guiTinNhanLogService.PagingChienDichLog(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpGet("")]
        public ApiResponse FindGuiTinNhanLog([FromQuery] int idChienDich, [FromQuery] int idDanhBa,[FromQuery] FindPagingGuiTinNhanLogDto dto)
        {
            try
            {
                var data = _guiTinNhanLogService.PagingGuiTinNhanLog(idChienDich,idDanhBa,dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
