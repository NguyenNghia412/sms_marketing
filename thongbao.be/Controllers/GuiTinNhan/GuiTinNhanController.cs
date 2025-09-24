using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.GuiTinNhan
{
    [Route("api/core/gui-tin-nhan")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GuiTinNhanController : BaseController
    {
        private readonly IGuiTinNhanJobService _guiTinNhanJobService;
        private readonly ILogger<GuiTinNhanController> _logger;
        private readonly ISendSmsService _sendSmsService;

        public GuiTinNhanController(
            ILogger<GuiTinNhanController> logger,
            IGuiTinNhanJobService guiTinNhanJobService,
            ISendSmsService sendSmsService) : base(logger)
        {
            _guiTinNhanJobService = guiTinNhanJobService;
            _sendSmsService = sendSmsService;
            _logger = logger;
        }


        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("save-config-chien-dich")]
        public async Task<ApiResponse> SaveThongTinChienDich([FromBody] GuiTinNhanDto dto)
        {
            try
            {
                await _guiTinNhanJobService.SaveThongTinChienDich(
                    dto.IdChienDich,
                    dto.IdDanhBa,
                    dto.IdBrandName,
                    dto.IsFlashSms,
                    dto.IsAccented,
                    dto.NoiDung
                 
                );

              

                return new ();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("send-sms")]
        public async Task<ApiResponse> SendSms([FromBody] GuiTinNhanDto dto)
        {
            try
            {
                var smsMessages = await _guiTinNhanJobService.StartGuiTinNhanJob(
                    dto.IdChienDich,
                    dto.IdDanhBa,
                    dto.IsFlashSms,
                    dto.IdBrandName,
                    dto.IsAccented,
                    dto.NoiDung);

                var result = await _sendSmsService.SendSmsAsync(smsMessages);
                await _guiTinNhanJobService.SendSmsLog(result, dto.IdChienDich, dto.IdDanhBa, dto.IdBrandName, dto.IsAccented, dto.NoiDung);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("preview-send-sms")]
        public async Task<ApiResponse>PreviewSendSms([FromBody] GetPreviewSmsDto dto)
        {
            try
            {
                var preview = await _guiTinNhanJobService.GetPreviewMessage(
                     dto.IdChienDich,
                    dto.IdDanhBa,
                    dto.IsFlashSms,
                    dto.IdBrandName,
                    dto.IsAccented,
                    dto.NoiDung,
                    dto.CurrentDanhBaSmsId
                    );
                return new(preview);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.GuiTinNhanAdd)]
        [HttpPost("du-tru-chi-phi")]
        public async Task<ApiResponse> GetChiPhiDuTruChienDich([FromBody] GuiTinNhanDto dto)
        {
            try
            {
                var cost = await _guiTinNhanJobService.GetChiPhiDuTruChienDich(
                    dto.IdChienDich,
                    dto.IdDanhBa,
                    dto.IdBrandName,
                    dto.IsFlashSms,
                    dto.IsAccented,
                    dto.NoiDung
                    );
                return new(cost);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}