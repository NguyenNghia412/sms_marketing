using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.TraoBang.Implements
{
    public class PlanService: BaseService,IPlanService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public PlanService(
            SmDbContext smDbContext,
            ILogger<PlanService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }
        public void Create(CreatePlanDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnameNow = GetVietnamTime();
            var plan = new domain.TraoBang.Plan
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                ThoiGianBatDau = dto.ThoiGianBatDau,
                ThoiGianKetThuc = dto.ThoiGianKetThuc,
                CreatedDate = vietnameNow,
                Deleted = false
            };
            _smDbContext.Plans.Add(plan);
            _smDbContext.SaveChanges();


        }
        public void Update(int id, UpdatePlanDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == id && !x.Deleted);
            if (plan == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            }
            var vietnameNow = GetVietnamTime();
            plan.Ten = dto.Ten;
            plan.MoTa = dto.MoTa;
            plan.ThoiGianBatDau = dto.ThoiGianBatDau;
            plan.ThoiGianKetThuc = dto.ThoiGianKetThuc;
            _smDbContext.Plans.Update(plan);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewPlanDto> FindPaging( FindPagingPlanDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)}, dto = {JsonSerializer.Serialize(dto)}");
            var query = from p in _smDbContext.Plans
                        where !p.Deleted
                        orderby p.CreatedDate descending
                        select p;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewPlanDto>>(data);
            return new BaseResponsePagingDto<ViewPlanDto>
            {
                TotalItems = query.Count(),
                Items = items
            };  
        }
        public void Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}, id = {id}");
            var vietnameNow = GetVietnamTime();
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == id && !x.Deleted);
            if (plan == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            }
            plan.DeletedDate = vietnameNow;
            plan.Deleted = true;
            _smDbContext.Plans.Update(plan);
            _smDbContext.SaveChanges();
        }   
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
