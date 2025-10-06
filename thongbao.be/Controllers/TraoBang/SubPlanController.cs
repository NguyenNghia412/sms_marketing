using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.Functions.ImTan;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
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
    public class SubPlanController : BaseController
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
        public async Task<ApiResponse> Update([FromBody] UpdateSubPlanDto dto)
        {
            try
            {
                await _subPlanService.Update(dto);
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
        public ApiResponse Delete([FromRoute] int idPlan, [FromRoute] int idSubPlan)
        {
            try
            {
                _subPlanService.Delete(idPlan, idSubPlan);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanUpdate)]
        [HttpPut("sort")]
        public ApiResponse Sort([FromBody] MoveOrderSubPlanDto dto)
        {
            try
            {
                var data = _subPlanService.MoveOrder(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("plan/{idPlan}/list")]
        public async Task<ApiResponse> ListSubPlan([FromRoute] int idPlan)
        {
            try
            {
                var result = await _subPlanService.GetListSubPlan(idPlan);
                return new(result);

            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPost("import-ggsheet")]
        public async Task<ApiResponse> ImportSubPlanFromGgSheet([FromBody] ImportDanhSachSinhVienNhanBangDto dto)
        {
            try
            {
                var data = await _subPlanService.ImportDanhSachNhanBang(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("paging-danh-sach-sinh-vien-nhan-bang")]
        public ApiResponse PagingSinhVienNhanBang([FromQuery] FindPagingSinhVienNhanBangDto dto)
        {
            try
            {
                var data = _subPlanService.PagingSinhVienNhanBang(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPost("sinh-vien-nhan-bang")]
        public ApiResponse CreateSinhVienNhanBang([FromBody] CreateSinhVienNhanBangDto dto)
        {
            try
            {
                _subPlanService.CreateSinhVienNhanBang(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanUpdate)]
        [HttpPut("sinh-vien-nhan-bang")]
        public ApiResponse UpdateSinhVienNhanBang([FromBody] UpdateSinhVienNhanBangDto dto)
        {
            try
            {
                _subPlanService.UpdateSinhVienNhanBang(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanDelete)]
        [HttpDelete("{idSubPlan}/sinh-vien-nhan-bang/{id}")]
        public ApiResponse DeleteSinhVienNhanBang([FromRoute] int idSubPlan, [FromRoute] int id)
        {
            try
            {
                _subPlanService.DeleteSinhVienNhanBang(idSubPlan, id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [AllowAnonymous]
        [HttpGet("sinh-vien-nhan-bang/{mssv}")]
        public async Task<ApiResponse> GetByMssv([FromRoute] string mssv)
        {
            try
            {
                var data = await _subPlanService.ShowSinhVienNhanBangInfor(mssv);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [AllowAnonymous]
        [HttpGet("sinh-vien-nhan-bang/{mssv}/next")]
        public async Task<ApiResponse> GetNextByMssv([FromRoute] string mssv)
        {
            try
            {
                var data = await _subPlanService.NextSinhVienNhanBang(mssv);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [AllowAnonymous]
        [HttpGet("sinh-vien-nhan-bang/{mssv}/prev")]
        public async Task<ApiResponse> GetPrevMssv([FromRoute] string mssv)
        {
            try
            {
                var data = await _subPlanService.PreviousSinhVienNhanBang(mssv);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPost("sinh-vien-nhan-bang/hang-doi")]
        public ApiResponse DiemDanhNhanBang([FromQuery] string mssv)
        {
            try
            {
                var data = _subPlanService.DiemDanhNhanBang(mssv);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("sinh-vien-nhan-bang/tien-do")]
        public async Task<ApiResponse> GetTienDoNhanBang([FromQuery] ViewTienDoNhanBangRequestDto dto)
        {
            try
            {
                var data = await _subPlanService.GetTienDoNhanBang(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("{idSubPlan}/thong-tin-subplan")]
        public async Task<ApiResponse> GetThongTinSubPlan([FromRoute] int idSubPlan)
        {
            try
            {
                var data = await _subPlanService.GetInforSubPlan(idSubPlan);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanUpdate)]
        [HttpPut("{id}/trang-thai-sub-plan")]
        public ApiResponse UpdateTrangThaiSubPlan([FromRoute] int id)
        {
            try
            {
                _subPlanService.UpdateTrangThaiSubPlan(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }

        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPost("{idSubPlan}/next-sub-plan")]
        public ApiResponse TaoSubPlanTiepTheo([FromRoute] int idSubPlan)
        {
            try
            {
                _subPlanService.NextSubPlan(idSubPlan);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanView)]
        [HttpGet("plan/{idPlan}/list-sub-plan-infor")]
        public async Task<ApiResponse> GetListSubPlanInfor([FromRoute] int idPlan)
        {
            try
            {
                var data = await _subPlanService.GetListSubPlanInfor(idPlan);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.SubPlanAdd)]
        [HttpPut("{idSubPlan}/sinh-vien-nhan-bang/{id}/trang-thai")]
        public ApiResponse UpdateTrangThaiSinhVienNhanBang([FromRoute] int idSubPlan,[FromRoute] int id)
        {
            try
            {
                _subPlanService.UpdateTrangThaiSinhVienNhanBang(idSubPlan,id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
