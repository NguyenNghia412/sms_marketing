using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class ChienDichService : BaseService, IChienDichService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public ChienDichService(
            SmDbContext smDbContext, 
            ILogger<ChienDichService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) 
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create(CreateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var tinNhan = new domain.GuiTinNhan.ChienDich
            {
                TenChienDich = dto.TenChienDich,
                NgayBatDau = dto.NgayBatDau ?? vietnamNow,
                NgayKetThuc = dto.NgayKetThuc,
                MoTa = dto.MoTa,
                IsFlashSms = dto.IsFlashSms,
                CreatedDate = vietnamNow,
            };

            _smDbContext.ChienDiches.Add(tinNhan);
            _smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewChienDichDto> Find(FindPagingChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={JsonSerializer.Serialize(dto)}");

            var query = from cd in _smDbContext.ChienDiches
                        where !cd.Deleted
                        orderby cd.CreatedDate descending
                        select cd;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewChienDichDto>>(data);

            return new BaseResponsePagingDto<ViewChienDichDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }
        public void Update(int idChienDich, UpdateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} idChienDich={idChienDich}, dto={JsonSerializer.Serialize(dto)}");
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(404,$"Chiến dịch không tồn tại");
            }
            chienDich.TenChienDich = dto.TenChienDich;
   
            chienDich.NgayKetThuc = dto.NgayKetThuc;
            chienDich.MoTa = dto.MoTa;
            chienDich.IsFlashSms = dto.IsFlashSms;
            _smDbContext.ChienDiches.Update(chienDich);
            _smDbContext.SaveChanges();
        }

        public void Delete(int idChienDich)
        {
            _logger.LogInformation($"{nameof(Delete)} idChienDich={idChienDich}");
            var vietnamNow  = GetVietnamTime();
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(404, $"Chiến dịch không tồn tại");
            }
            chienDich.Deleted = true;
            chienDich.DeletedDate = vietnamNow;
            _smDbContext.ChienDiches.Update(chienDich);
            _smDbContext.SaveChanges();
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
        public void TestSendEmail()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Test send email!"));
        }
    }
}
