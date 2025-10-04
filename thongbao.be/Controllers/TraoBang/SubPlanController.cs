using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.TraoBang
{
    [Route("api/core/trao-bang/sub-plan")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SubPlanController:BaseController
    {
        private readonly ISubPlanService _subPlanService;

        public SubPlanController(ILogger<SubPlanController> logger, ISubPlanService subPlanService) : base(logger)
        {
            _subPlanService = subPlanService;
        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPost("{idPlan}")]
        public ApiResponse Create([FromRoute] int idPlan, [FromBody] CreateSubPlanDto dto)
        {
            try
            {
                _subPlanService.Create(idPlan, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanUpdate)]
        [HttpPut("")]
        public ApiResponse Update( [FromBody] UpdateSubPlanDto dto)
        {
            try
            {
                _subPlanService.Update( dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("")]
        public ApiResponse FindPaging([FromQuery] FindPagingSubPlanDto dto)
        {
            try
            {
                var result = _subPlanService.FindPaging(dto);
                return new()
                {
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.SubPlanUpdate)]
        [HttpPut("is-show")]
        public ApiResponse UpdateIsShow([FromBody] UpdateSubPlanIsShowDto dto)
        {
            try
            {
                _subPlanService.UpdateIsShow(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanDelete)]
        [HttpDelete("{idSubPlan}/plan/{idPlan}")]
        public ApiResponse Delete([FromRoute] int idSubPlan, [FromRoute] int idPlan)
        {
            try
            {
                _subPlanService.Delete(idSubPlan, idPlan);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
}
