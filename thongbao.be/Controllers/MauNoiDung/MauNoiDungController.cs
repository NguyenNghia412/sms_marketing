using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.application.MauNoiDung.Dtos;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.Controllers.ChienDich;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.MauNoiDung
{
    [Route("api/core/mau-noi-dung")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MauNoiDungController:BaseController

    {
        private readonly IMauNoiDungService _mauNoiDungService;

        public MauNoiDungController(ILogger<MauNoiDungController> logger, IMauNoiDungService mauNoiDungService) : base(logger)
        {
            _mauNoiDungService = mauNoiDungService;
        }


        [Permission(PermissionKeys.MauNoiDungAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromQuery] int id ,[FromBody] UpdateMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungService.Update(id,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingMauNoiDungDto dto)
        {
            try
            {
                var data = _mauNoiDungService.Find( dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungDelete)]
        [HttpDelete("")]
        public ApiResponse Delete([FromQuery] int id)
        {
            try
            {
                _mauNoiDungService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.MauNoiDungAdd)]
        [HttpPost("chien-dich")]
        public ApiResponse CreateChienDichByMauNoiDung([FromQuery] int id, [FromBody] CreateChienDichByMauNoiDungDto dto)
        {
            try
            {
                _mauNoiDungService.CreateChienDichByMauNoiDung(id,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungView)]
        [HttpGet("list-mau-noi-dung")]
        public ApiResponse GetListMauNoiDung()
        {
            try
            {
                var data = _mauNoiDungService.GetListMauNoiDung();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
