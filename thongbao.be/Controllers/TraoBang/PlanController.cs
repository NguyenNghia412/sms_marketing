using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.Controllers.MauNoiDung;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.TraoBang
{
    [Route("api/core/trao-bang/plan")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlanController:BaseController
    {
        private readonly IPlanService _planService;

        public PlanController(ILogger<PlanController> logger, IPlanService planService) : base(logger)
        {
            _planService = planService;
        }

        [Permission(PermissionKeys.PlanAdd)]
        [HttpPost("")]
        public ApiResponse Create (CreatePlanDto dto)
        {
            try
            {
                _planService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.PlanUpdate)]
        [HttpPut("{id}")]
        public ApiResponse Update ([FromRoute] int id,[FromBody] UpdatePlanDto dto)
        {
            try
            {
                _planService.Update(id, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.PlanView)]
        [HttpGet("")]
        public ApiResponse FindPaging([FromQuery] FindPagingPlanDto dto)
        {
            try
            {
                var result = _planService.FindPaging(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.PlanDelete)]
        [HttpDelete("{id}")]
        public ApiResponse Delete([FromRoute] int id)
        {
            try
            {
                _planService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
