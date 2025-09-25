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
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.domain.DanhBa;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class GuiTinNhanLogService : BaseService, IGuiTinNhanLogService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public GuiTinNhanLogService(
             SmDbContext smDbContext,
            ILogger<GuiTinNhanLogService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }
        public BaseResponsePagingDto<ViewChienDichLogDto> PagingChienDichLog(FindPagingChienDichLogDto dto)
        {
            _logger.LogInformation($"{nameof(PagingChienDichLog)} dto={JsonSerializer.Serialize(dto)}");

            var query = from clog in _smDbContext.ChienDichLogTrangThaiGuis
                        join cd in _smDbContext.ChienDiches on clog.IdChienDich equals cd.Id
                        join db in _smDbContext.DanhBas on clog.IdDanhBa equals db.Id
                        where !cd.Deleted && !db.Deleted && !clog.Deleted
                        orderby clog.CreatedDate descending
                        select new ViewChienDichLogDto
                        {
                            IdChienDich = cd.Id,
                            TenChienDich = cd.TenChienDich,
                            SmsSentSuccess = clog.SmsSendSuccess,
                            SmsSentFailed = clog.SmsSendFailed,
                            TrangThai = clog.TrangThai,
                            TongChiPhi = clog.TongChiPhi,
                            NgayGui = clog.CreatedDate,
                            danhBa = new ViewDanhBaLogDto
                            {
                                IdDanhBa = db.Id,
                                TenDanhBa = db.TenDanhBa
                            }
                        };

            var data = query.Paging(dto).ToList();

            return new BaseResponsePagingDto<ViewChienDichLogDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
        }
        public BaseResponsePagingDto<ViewDanhBaSmsLogDto> PagingGuiTinNhanLog(int idChienDich,int idDanhBa,FindPagingGuiTinNhanLogDto dto)
        {
            _logger.LogInformation($"{nameof(PagingGuiTinNhanLog)} dto={JsonSerializer.Serialize(dto)}");

            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }


            var existingDanhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));

            var query = from dbs in _smDbContext.DanhBaSms
                        join log in _smDbContext.GuiTinNhanLogChiTiets on dbs.Id equals log.IdDanhBaSms
                        join bn in _smDbContext.BrandName on log.IdBrandName equals bn.Id
                        where !dbs.Deleted && !log.Deleted && !bn.Deleted
                              && log.IdChienDich == idChienDich
                              && dbs.IdDanhBa == idDanhBa
                        orderby log.CreatedDate descending
                        select new ViewDanhBaSmsLogDto
                        {
                            Id = dbs.Id,
                            HoVaTen = dbs.HoVaTen,
                            MaSoNguoiDung = dbs.MaSoNguoiDung,
                            SoDienThoai = dbs.SoDienThoai,
                            BrandName = new BrandNameDto
                            {
                                Id = bn.Id,
                                TenBrandName = bn.TenBrandName
                            },
                            Log = new ViewGuiTinNhanLogDto
                            {
                                Price = log.Price,
                                Code = log.Code,
                                Message = log.Message,
                                TrangThai = log.TrangThai,
                            }
                        };

            var data = query.Paging(dto).ToList();

            return new BaseResponsePagingDto<ViewDanhBaSmsLogDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
        }



    }
}
