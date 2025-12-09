using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungEmail;
using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungSms;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.MauNoiDung.Implements
{
    public class MauNoiDungEmailService : BaseService, IMauNoiDungEmailService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public MauNoiDungEmailService(
            SmDbContext smDbContext,
            ILogger<MauNoiDungEmailService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }
        public void Create(CreateMauNoiDungEmailDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = new domain.MauNoiDung.MauNoiDungEmail
            {
                TenMauNoiDung = dto.TenMauNoiDung,
                ThietKe = JsonSerializer.Serialize(dto.ThietKe),
                CreatedDate = vietnamNow,
                CreatedBy = currentUserId,
                Deleted = false

            };
            _smDbContext.MauNoiDungEmail.Add(mauNoiDung);
            _smDbContext.SaveChanges();
        }

        public void Update( UpdateMauNoiDungEmailDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = _smDbContext.MauNoiDungEmail.FirstOrDefault(x => x.Id == dto.Id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);
            mauNoiDung.TenMauNoiDung = dto.TenMauNoiDung;
            mauNoiDung.ThietKe = JsonSerializer.Serialize(dto.ThietKe);
            _smDbContext.MauNoiDungEmail.Update(mauNoiDung);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewMauNoiDungEmailDto> Find(FindPagingMauNoiDungEmailDto dto)
        {
            _logger.LogInformation($"{nameof(Find)}, dto = {JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from mnd in _smDbContext.MauNoiDungEmail
                        where !mnd.Deleted && (isSuperAdmin || mnd.CreatedBy == currentUserId)
                        orderby mnd.CreatedDate descending
                        select mnd;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewMauNoiDungEmailDto>>(data);
            return new BaseResponsePagingDto<ViewMauNoiDungEmailDto>
            {
                Items = items,
                TotalItems = query.Count()
            };

        }

        public List<GetListMauNoiDungEmailResponseDto> GetListMauNoiDung()
        {
            _logger.LogInformation($"{nameof(GetListMauNoiDung)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from mnd in _smDbContext.MauNoiDungEmail
                        where !mnd.Deleted && (isSuperAdmin || mnd.CreatedBy == currentUserId)
                        orderby mnd.CreatedDate descending
                        select mnd;
            var data = query.ToList();
            var result = _mapper.Map<List<GetListMauNoiDungEmailResponseDto>>(data);
            return result;

        }

        public void Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = _smDbContext.MauNoiDungEmail.FirstOrDefault(x => x.Id == id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);
            mauNoiDung.Deleted = true;
            mauNoiDung.DeletedDate = vietnamNow;
            mauNoiDung.DeletedBy = currentUserId;
      
            _smDbContext.SaveChanges();
        }
        public ViewMauNoiDungEmailByIdDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}, id = {id}");

            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();

            var mauNoiDung = _smDbContext.MauNoiDungEmail
                .FirstOrDefault(x => x.Id == id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);

            return new ViewMauNoiDungEmailByIdDto
            {
                ThietKe = mauNoiDung.ThietKe  
            };
        }
    }
}
