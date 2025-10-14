using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("chien-dich/{idChienDich}")]
        public ApiResponse FindGuiTinNhanLog([FromRoute] int idChienDich,[FromQuery] FindPagingGuiTinNhanLogDto dto)
        {
            try
            {
                var data = _guiTinNhanLogService.PagingGuiTinNhanLog(idChienDich,dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("export-thong-ke-theo-chien-dich-excel")]
        public async Task<IActionResult> ExportThongKeTheoChienDich([FromBody] ExportSmsLogTheoChienDichDto dto)
        {
            try
            {
                var excelTemplate = await _guiTinNhanLogService.ExportThongKeTheoChienDich(dto);
                var fileName = $"Thong_Ke_Gui_Tin_Nhan_Theo_Chien_Dich.xlsx";
                return File(
                   excelTemplate,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                 );

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("export-thong-ke-theo-thang-excel")]
        public async Task<IActionResult> ExportThongKeTheoThang([FromBody] ExportSmsLogTheoThangDto dto)
        {
            try
            {
                var excelTemplate = await _guiTinNhanLogService.ExportThongKeTheoThang(dto);
                var fileName = $"Thong_Ke_Gui_Tin_Nhan_Theo_Thang.xlsx";
                return File(
                   excelTemplate,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                 );

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
    }
}
