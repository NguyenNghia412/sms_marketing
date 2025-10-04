using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.infrastructure.data;
using thongbao.be.infrastructure.data.Migrations;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.TraoBang.Implements
{
    public class SubPlanService:BaseService, ISubPlanService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public SubPlanService(
            SmDbContext smDbContext,
            ILogger<SubPlanService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }
        public void Create (int idPlan, CreateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = new domain.TraoBang.SubPlan
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                TruongKhoa = dto.TruongKhoa,
                MoBai = dto.MoBai ?? "",
                KetBai = dto.KetBai ?? "",
                Note = dto.Note,
                IsShow = true,
                Order = dto.Order,
                IdPlan = idPlan,
                CreatedDate = GetVietnamTime(),
                Deleted = false
            };
            _smDbContext.SubPlans.Add(subplan);
            _smDbContext.SaveChanges();
        }
        public void Update (UpdateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            subplan.Ten = dto.Ten;
            subplan.MoTa = dto.MoTa;
            subplan.TruongKhoa = dto.TruongKhoa;
            subplan.MoBai = dto.MoBai ?? "";
            subplan.KetBai = dto.KetBai ?? "";
            subplan.Note = dto.Note;
            //subplan.Order = dto.Order;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewSubPlanDto> FindPaging (FindPagingSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)}, dto = {JsonSerializer.Serialize(dto)}");
            var query = from sp in _smDbContext.SubPlans
                        where !sp.Deleted
                        orderby sp.Order ascending
                        select sp;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewSubPlanDto>>(data);
            return new BaseResponsePagingDto<ViewSubPlanDto>
            {
                TotalItems = query.Count(),
                Items = items
            };
        }
        public void UpdateIsShow (UpdateSubPlanIsShowDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateIsShow)}, dto= {JsonSerializer.Serialize(dto)} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            subplan.IsShow = dto.IsShow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public void Delete (int idPlan, int idSubPlan)
        {
            _logger.LogInformation($"{nameof(Delete)}, idPlan= {idPlan}, idSubPlan= {idSubPlan} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var vietnamNow = GetVietnamTime();
            subplan.Deleted = true;
            subplan.DeletedDate = vietnamNow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
