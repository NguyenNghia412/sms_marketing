using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungSms;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.Controllers.ChienDich;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.MauNoiDung
{
    [Route("api/core/mau-noi-dung-sms")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MauNoiDungSmsController:BaseController

    {
        private readonly IMauNoiDungSmsService _mauNoiDungSmsService;

        public MauNoiDungSmsController(ILogger<MauNoiDungSmsController> logger, IMauNoiDungSmsService mauNoiDungSmsService) : base(logger)
        {
            _mauNoiDungSmsService = mauNoiDungSmsService;
        }


        [Permission(PermissionKeys.MauNoiDungSmsAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungSmsService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungSmsUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromQuery] int id ,[FromBody] UpdateMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungSmsService.Update(id,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungSmsView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingMauNoiDungDto dto)
        {
            try
            {
                var data = _mauNoiDungSmsService.Find( dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungSmsDelete)]
        [HttpDelete("")]
        public ApiResponse Delete([FromQuery] int id)
        {
            try
            {
                _mauNoiDungSmsService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.MauNoiDungSmsAdd)]
        [HttpPost("chien-dich")]
        public ApiResponse CreateChienDichByMauNoiDung([FromQuery] int id, [FromBody] CreateChienDichByMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungSmsService.CreateChienDichByMauNoiDung(id,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungSmsView)]
        [HttpGet("list-mau-noi-dung")]
        public ApiResponse GetListMauNoiDung()
        {
            try
            {
                var data = _mauNoiDungSmsService.GetListMauNoiDung();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
