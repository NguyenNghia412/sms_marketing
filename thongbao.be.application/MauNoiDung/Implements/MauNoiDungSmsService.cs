using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.application.GuiTinNhan.Implements;
using thongbao.be.application.MauNoiDung.Dtos.MauNoiDungSms;
using thongbao.be.application.MauNoiDung.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using Volo.Abp.Users;

namespace thongbao.be.application.MauNoiDung.Implements
{
    public class MauNoiDungSmsService: BaseService, IMauNoiDungSmsService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public MauNoiDungSmsService(
            SmDbContext smDbContext,
            ILogger<MauNoiDungSmsService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create (CreateMauNoiDungDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = new domain.MauNoiDung.MauNoiDungSms
            {
                TenMauNoiDung = dto.TenMauNoiDung,
                NoiDung = dto.MauNoiDung,
                CreatedDate = vietnamNow,
                CreatedBy = currentUserId,
                Deleted = false

            };
            _smDbContext.MauNoiDungSms.Add( mauNoiDung );
            _smDbContext.SaveChanges(); 
        }

        public void Update (int id,UpdateMauNoiDungDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = _smDbContext.MauNoiDungSms.FirstOrDefault(x => x.Id == id &&( isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);
            mauNoiDung.TenMauNoiDung = dto.TenMauNoiDung;
            mauNoiDung.NoiDung = dto.NoiDung;
            _smDbContext.MauNoiDungSms.Update(mauNoiDung);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewMauNoiDungDto> Find (FindPagingMauNoiDungDto dto)
        {
            _logger.LogInformation($"{nameof(Find)}, dto = {JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from mnd in _smDbContext.MauNoiDungSms
                        where !mnd.Deleted && (isSuperAdmin || mnd.CreatedBy == currentUserId)
                        orderby mnd.CreatedDate descending
                        select mnd;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewMauNoiDungDto>>(data);
            return new BaseResponsePagingDto<ViewMauNoiDungDto>
            {
                Items = items,
                TotalItems = query.Count()
            };

        }
        public List<GetListMauNoiDungResponseDto> GetListMauNoiDung()
        {
            _logger.LogInformation($"{nameof(GetListMauNoiDung)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from mnd in _smDbContext.MauNoiDungSms
                        where !mnd.Deleted && (isSuperAdmin || mnd.CreatedBy == currentUserId)
                        orderby mnd.CreatedDate descending
                        select mnd;
            var data = query.ToList();
            var result = _mapper.Map<List<GetListMauNoiDungResponseDto>>(data);
            return result;

        }
        public void Delete (int id)
        {
            _logger.LogInformation($"{nameof(Delete)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = _smDbContext.MauNoiDungSms.FirstOrDefault(x => x.Id == id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);
            mauNoiDung.Deleted = true;
            mauNoiDung.DeletedDate = vietnamNow;
            mauNoiDung.DeletedBy = currentUserId;
            var chienDichs = _smDbContext.ChienDiches.Where(x => x.IdMauNoiDung == id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted).ToList();
            foreach (var chienDich in chienDichs)
            {
                chienDich.IdMauNoiDung = null;
            }
            _smDbContext.SaveChanges();
        }
        public void CreateChienDichByMauNoiDung(int id, CreateChienDichByMauNoiDungDto dto)
        {
            _logger.LogInformation($"{nameof(CreateChienDichByMauNoiDung)}, id = {id}, dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var mauNoiDung = _smDbContext.MauNoiDungSms.FirstOrDefault(x => x.Id == id && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.MauNoiDungErrorNotFound);

            var chienDich = new domain.GuiTinNhan.ChienDich
            {
                IdMauNoiDung = id,
                TenChienDich = dto.TenChienDich,
                MoTa = dto.MoTa,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                NoiDung = dto.MauNoiDung,
                IsFlashSms = dto.IsFlashSms,
                CreatedDate = vietnamNow,
                CreatedBy = currentUserId,
            };

            _smDbContext.ChienDiches.Add(chienDich);
            _smDbContext.SaveChanges();
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
