using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungEmail;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.MauNoiDung
{
    [Route("api/core/mau-noi-dung-email")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MauNoiDungEmailController : BaseController

    {
        private readonly IMauNoiDungEmailService _mauNoiDungEmailService;

        public MauNoiDungEmailController(ILogger<MauNoiDungEmailController> logger, IMauNoiDungEmailService mauNoiDungEmailService) : base(logger)
        {
            _mauNoiDungEmailService = mauNoiDungEmailService;
        }

        [Permission(PermissionKeys.MauNoiDungEmailAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateMauNoiDungEmailDto dto)
        {
            try
            {
                return new(_mauNoiDungEmailService.Create(dto));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungEmailUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromBody] UpdateMauNoiDungEmailDto dto)
        {
            try
            {
                _mauNoiDungEmailService.Update(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungEmailView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingMauNoiDungEmailDto dto)
        {
            try
            {
                var data = _mauNoiDungEmailService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.MauNoiDungEmailDelete)]
        [HttpDelete("{id}")]
        public ApiResponse Delete([FromRoute] int id)
        {
            try
            {
                _mauNoiDungEmailService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.MauNoiDungEmailView)]
        [HttpGet("list-mau-noi-dung")]
        public ApiResponse GetListMauNoiDung()
        {
            try
            {
                var data = _mauNoiDungEmailService.GetListMauNoiDung();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.MauNoiDungEmailView)]
        [HttpGet("{idMnd}")]
        public ApiResponse FindById([FromRoute] int idMnd)
        {
            try
            {
                var data = _mauNoiDungEmailService.FindById(idMnd);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
