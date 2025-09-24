using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
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

        public void Create( CreateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto={JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
          

            var chienDich = new domain.GuiTinNhan.ChienDich
            {
                TenChienDich = dto.TenChienDich,
                NgayBatDau = dto.NgayBatDau ?? vietnamNow,
                NgayKetThuc = dto.NgayKetThuc,
                CreatedDate = vietnamNow,
            };

            _smDbContext.ChienDiches.Add(chienDich);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewChienDichDto> Find(FindPagingChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={JsonSerializer.Serialize(dto)}");
            var query = from cd in _smDbContext.ChienDiches
                        join bn in _smDbContext.BrandName on cd.IdBrandName equals bn.Id into brandJoin
                        from brand in brandJoin.DefaultIfEmpty()
                        join mnd in _smDbContext.MauNoiDungs on cd.IdMauNoiDung equals mnd.Id into mauNoiDungJoin
                        from mauNoiDung in mauNoiDungJoin.DefaultIfEmpty()
                        where !cd.Deleted
                        orderby cd.CreatedDate descending
                        select new ViewChienDichDto
                        {
                            Id = cd.Id,
                            TenChienDich = cd.TenChienDich,
                            MoTa = cd.MoTa,
                            NoiDung = cd.NoiDung,
                            NgayBatDau = cd.NgayBatDau,
                            NgayKetThuc = cd.NgayKetThuc,
                            IdMauNoiDung = cd.IdMauNoiDung,
                            TenMauNoiDung = mauNoiDung != null && !mauNoiDung.Deleted ? mauNoiDung.TenMauNoiDung : null,
                            NoiDungMauNoiDung  = mauNoiDung != null && !mauNoiDung.Deleted ? mauNoiDung.NoiDung : null,
                            IdBrandName = cd.IdBrandName,
                            TenBrandName = brand != null ? brand.TenBrandName : string.Empty,
                            IsFlashSms = cd.IsFlashSms,
                            CreatedBy = cd.CreatedBy,
                            CreatedDate = cd.CreatedDate,
                            DanhBas = (from cddb in _smDbContext.ChienDichDanhBa
                                       join db in _smDbContext.DanhBas on cddb.IdDanhBa equals db.Id
                                       where cddb.IdChienDich == cd.Id
                                             && !cddb.Deleted
                                             && !db.Deleted
                                       orderby cddb.CreatedDate
                                       select new ChienDichDanhBaDto
                                       {
                                           IdDanhBa = db.Id,
                                           TenDanhBa = db.TenDanhBa,
                                           CreatedDate = db.CreatedDate
                                       }).ToList(),
                        };
            var data = query.Paging(dto).ToList();
            return new BaseResponsePagingDto<ViewChienDichDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
        }
        public void Update(int idChienDich, UpdateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} idChienDich={idChienDich}, dto={JsonSerializer.Serialize(dto)}");
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }
            chienDich.TenChienDich = dto.TenChienDich;
   
            chienDich.NgayKetThuc = dto.NgayKetThuc;
            chienDich.MoTa = dto.MoTa;
            //chienDich.NoiDung = dto.NoiDung;
            chienDich.IsFlashSms = dto.IsFlashSms;
            _smDbContext.ChienDiches.Update(chienDich);
            _smDbContext.SaveChanges();
        }

        public void Delete(int idChienDich)
        {
            _logger.LogInformation($"{nameof(Delete)} idChienDich={idChienDich}");
            var vietnamNow = GetVietnamTime();
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }

            var chienDichLogTrangThaiGuiList = _smDbContext.ChienDichLogTrangThaiGuis
                .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                .ToList();

            foreach (var logTrangThaiGui in chienDichLogTrangThaiGuiList)
            {
                logTrangThaiGui.Deleted = true;
                logTrangThaiGui.DeletedDate = vietnamNow;
            }

            var chienDichDanhBaList = _smDbContext.ChienDichDanhBa
                .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                .ToList();

            foreach (var chienDichDanhBa in chienDichDanhBaList)
            {
                chienDichDanhBa.Deleted = true;
                chienDichDanhBa.DeletedDate = vietnamNow;
            }

      

            chienDich.Deleted = true;
            chienDich.DeletedDate = vietnamNow;

            _smDbContext.SaveChanges();
        }
        public ViewChienDichByIdDto GetChienDichById(int idChienDich, int idDanhBa)
        {
            _logger.LogInformation($"{nameof(GetChienDichById)}");

            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);

            var brandName = _smDbContext.BrandName.FirstOrDefault(x => x.Id == chienDich.IdBrandName && !x.Deleted);
            

            return new ViewChienDichByIdDto
            {
                TenChienDich = chienDich.TenChienDich,
                TenDanhBa = danhBa.TenDanhBa,
                IdBrandName = chienDich.IdBrandName,
                TenBrandName = brandName?.TenBrandName ?? string.Empty,
                IsFlashSms = chienDich.IsFlashSms,
                IsAccented = chienDich.IsAccented,
                NoiDung = chienDich.NoiDung ?? string.Empty,
                NgayBatDau = chienDich.NgayBatDau ?? DateTime.MinValue,
                NgayKetThuc = chienDich.NgayKetThuc ?? DateTime.MinValue
            };
        }

        public void AddDanhBaChienDich (int idChienDich, int idDanhBa)
        {
            _logger.LogInformation($"{nameof(AddDanhBaChienDich)} ");
            var vietnam = GetVietnamTime();
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted);
            if (danhBa == null)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            }
            var chienDichDanhBa = new domain.GuiTinNhan.ChienDichDanhBa
            {
                IdChienDich = idChienDich,
                IdDanhBa = idDanhBa,
            };
            _smDbContext.ChienDichDanhBa.Add(chienDichDanhBa);
            _smDbContext.SaveChanges();
            
        }

        public List<GetListBrandNameResponseDto> GetListBrandName()
        {
            _logger.LogInformation($"{nameof(GetListBrandName)}");

            var query = from bn in _smDbContext.BrandName
                        where !bn.Deleted
                        orderby bn.CreatedDate descending
                        select bn;

            var data = query.ToList();
            var result = _mapper.Map<List<GetListBrandNameResponseDto>>(data);

            return result;
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
