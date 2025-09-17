using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.DanhBa.Implements;
using thongbao.be.application.DanhBa.Interfaces;
using thongbao.be.application.ToChuc.Dtos;
using thongbao.be.application.ToChuc.Implements;
using thongbao.be.application.ToChuc.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.Controllers.DanhBa;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.ToChuc
{

    [Route("api/core/to-chuc")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ToChucController :BaseController
    {
        private readonly IToChucService _toChucService;
        private readonly ILogger<ToChucController> _logger;
        public ToChucController(ILogger<ToChucController> logger, IToChucService toChucService) : base(logger)
        {
            _toChucService = toChucService;
            _logger = logger;
        }


        [Permission(PermissionKeys.ToChucAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateToChucDto dto)
        {
            try
            {
                _toChucService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.ToChucUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromQuery] int idToChuc,[FromBody] UpdateToChucDto dto)
        {
            try
            {
                _toChucService.Update(idToChuc,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.ToChucDelete)]
        [HttpDelete("")]
        public ApiResponse Delete([FromQuery] int idToChuc)
        {
            try
            {
                _toChucService.Delete(idToChuc);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.ToChucView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingToChucDto dto)
        {
            try
            {
                var data = _toChucService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
        
    

}

