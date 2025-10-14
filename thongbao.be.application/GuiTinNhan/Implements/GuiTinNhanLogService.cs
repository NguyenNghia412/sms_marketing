using AutoMapper;
using ClosedXML.Excel;
using JetBrains.Annotations;
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
                        join db in _smDbContext.DanhBas on clog.IdDanhBa equals db.Id into dbGroup
                        from db in dbGroup.DefaultIfEmpty()
                        where !cd.Deleted && !clog.Deleted && (db == null || !db.Deleted)
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || cd.TenChienDich.Contains(dto.Keyword)
                                  || clog.NoiDung.Contains(dto.Keyword)
                                  || (db != null && db.TenDanhBa.Contains(dto.Keyword)))
                        orderby clog.CreatedDate descending
                        select new ViewChienDichLogDto
                        {
                            IdChienDich = cd.Id,
                            TenChienDich = cd.TenChienDich,
                            TongSoSms = clog.TongSoSms,
                            SmsSentSuccess = clog.SmsSendSuccess,
                            SmsSentFailed = clog.SmsSendFailed,
                            NoiDung = clog.NoiDung,
                            TrangThai = clog.TrangThai,
                            TongChiPhi = clog.TongChiPhi,
                            NgayGui = clog.CreatedDate,
                            danhBa = db != null ? new ViewDanhBaLogDto
                            {
                                IdDanhBa = db.Id,
                                TenDanhBa = db.TenDanhBa
                            } : null
                        };
            var data = query.Paging(dto).ToList();
            return new BaseResponsePagingDto<ViewChienDichLogDto>
            {
                Items = data,
                TotalItems = query.Count()
            };
        }

        public BaseResponsePagingDto<ViewDanhBaSmsLogDto> PagingGuiTinNhanLog(int idChienDich, FindPagingGuiTinNhanLogDto dto)
        {
            _logger.LogInformation($"{nameof(PagingGuiTinNhanLog)} dto={JsonSerializer.Serialize(dto)}");
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ChienDichErrorNotFound));
            }
            // Mode: Danh bạ
            if (dto.idDanhBa.HasValue && dto.idDanhBa.Value > 0)
            {
                var query = from dbs in _smDbContext.DanhBaSms
                            join log in _smDbContext.GuiTinNhanLogChiTiets on dbs.Id equals log.IdDanhBaSms
                            join bn in _smDbContext.BrandName on log.IdBrandName equals bn.Id
                            where !dbs.Deleted && !log.Deleted && !bn.Deleted
                                  && log.IdChienDich == idChienDich
                                  && dbs.IdDanhBa == dto.idDanhBa
                                  && (string.IsNullOrEmpty(dto.Keyword)
                                      || dbs.HoVaTen.Contains(dto.Keyword)
                                      || log.SoDienThoai.Contains(dto.Keyword)
                                      || log.NoiDungChiTiet.Contains(dto.Keyword)
                                      || bn.TenBrandName.Contains(dto.Keyword))
                            orderby log.CreatedDate descending
                            select new ViewDanhBaSmsLogDto
                            {
                                Id = dbs.Id,
                                HoVaTen = dbs.HoVaTen,
                                //SoDienThoai = dbs.SoDienThoai,
                                BrandName = new BrandNameDto
                                {
                                    Id = bn.Id,
                                    TenBrandName = bn.TenBrandName
                                },
                                Log = new ViewGuiTinNhanLogDto
                                {
                                    SoDienThoai = log.SoDienThoai,
                                    NoiDungChiTiet = log.NoiDungChiTiet,
                                    Price = log.Price,
                                    Code = log.Code,
                                    Message = log.Message,
                                    NgayGui = log.CreatedDate,
                                }
                            };
                var data = query.Paging(dto).ToList();
                return new BaseResponsePagingDto<ViewDanhBaSmsLogDto>
                {
                    Items = data,
                    TotalItems = query.Count()
                };
            }
            // Mode: List số điện thoại
            else
            {
                var query = from log in _smDbContext.GuiTinNhanLogChiTiets
                            join bn in _smDbContext.BrandName on log.IdBrandName equals bn.Id
                            where !log.Deleted && !bn.Deleted
                                  && log.IdChienDich == idChienDich
                                  && log.IdDanhBaSms == null
                                  && (string.IsNullOrEmpty(dto.Keyword)
                                      || log.SoDienThoai.Contains(dto.Keyword)
                                      || log.NoiDungChiTiet.Contains(dto.Keyword)
                                      || bn.TenBrandName.Contains(dto.Keyword))
                            orderby log.CreatedDate descending
                            select new ViewDanhBaSmsLogDto
                            {
                                Id = log.Id,
                                HoVaTen = string.Empty,
                                //SoDienThoai = string.Empty,
                                BrandName = new BrandNameDto
                                {
                                    Id = bn.Id,
                                    TenBrandName = bn.TenBrandName
                                },
                                Log = new ViewGuiTinNhanLogDto
                                {
                                    SoDienThoai = log.SoDienThoai,
                                    NoiDungChiTiet = log.NoiDungChiTiet,
                                    Price = log.Price,
                                    Code = log.Code,
                                    Message = log.Message,
                                    NgayGui = log.CreatedDate,
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
        public async Task<byte[]> ExportThongKeTheoChienDich(ExportSmsLogTheoChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(ExportThongKeTheoChienDich)} dto={JsonSerializer.Serialize(dto)}");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Thống Kê");

                int currentRow = 1;


                var titleCell = worksheet.Cell(currentRow, 1);
                titleCell.Value = "THỐNG KÊ CÁC CHIẾN DỊCH GỬI TIN NHẮN";
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(currentRow, 1, currentRow, 8).Merge();
                currentRow += 3;

                foreach (var idChienDich in dto.idChienDichs)
                {
                    var chienDichLogs = await _smDbContext.ChienDichLogTrangThaiGuis
                        .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                        .ToListAsync();

                    if (!chienDichLogs.Any())
                        continue;

                    var chiTietLogs = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                        .Join(_smDbContext.BrandName, log => log.IdBrandName, bn => bn.Id, (log, bn) => new { log, bn })
                        .ToListAsync();

                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

                    if (chienDich == null)
                        continue;

                    var headerRow = currentRow;
                    worksheet.Cell(currentRow, 1).Value = "Stt";
                    worksheet.Cell(currentRow, 2).Value = "Tên Chiến Dịch";
                    worksheet.Cell(currentRow, 3).Value = "Họ và Tên";
                    worksheet.Cell(currentRow, 4).Value = "Số điện thoại";
                    worksheet.Cell(currentRow, 5).Value = "BrandName";
                    worksheet.Cell(currentRow, 6).Value = "Nội Dung Chi Tiết";
                    worksheet.Cell(currentRow, 7).Value = "Trạng Thái";
                    worksheet.Cell(currentRow, 8).Value = "Thời Gian Gửi";

                    for (int col = 1; col <= 8; col++)
                    {
                        var headerCell = worksheet.Cell(headerRow, col);
                        headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    currentRow++;

                    int stt = 1;
                    foreach (var chiTiet in chiTietLogs)
                    {
                        worksheet.Cell(currentRow, 1).Value = stt;
                        worksheet.Cell(currentRow, 2).Value = chienDich.TenChienDich;
                        worksheet.Cell(currentRow, 3).Value = chiTiet.log.IdDanhBaSms.HasValue ?
                            _smDbContext.DanhBaSms.FirstOrDefault(x => x.Id == chiTiet.log.IdDanhBaSms)?.HoVaTen ?? "" : "";
                        worksheet.Cell(currentRow, 4).Value = chiTiet.log.SoDienThoai;
                        worksheet.Cell(currentRow, 5).Value = chiTiet.bn.TenBrandName;
                        worksheet.Cell(currentRow, 6).Value = chiTiet.log.NoiDungChiTiet;
                        worksheet.Cell(currentRow, 7).Value = chiTiet.log.TrangThai;
                        worksheet.Cell(currentRow, 8).Value = chiTiet.log.CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss");

                        currentRow++;
                        stt++;
                    }

                    currentRow++;


                    var totalSms = chienDichLogs.Sum(x => x.TongSoSms);
                    var successSms = chienDichLogs.Sum(x => x.SmsSendSuccess);
                    var failedSms = chienDichLogs.Sum(x => x.SmsSendFailed);
                    var danhBa = chienDichLogs.FirstOrDefault()?.IdDanhBa;
                    var danhBaName = danhBa.HasValue ?
                        _smDbContext.DanhBas.FirstOrDefault(x => x.Id == danhBa)?.TenDanhBa : null;

                    var statsHeaderCell = worksheet.Cell(currentRow, 1);
                    statsHeaderCell.Value = "THỐNG KÊ";
                    statsHeaderCell.Style.Font.Bold = true;
                    statsHeaderCell.Style.Font.FontSize = 14;
                    statsHeaderCell.Style.Fill.BackgroundColor = XLColor.FromArgb(70, 130, 180);
                    statsHeaderCell.Style.Font.FontColor = XLColor.White;
                    statsHeaderCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(currentRow, 1, currentRow, 2).Merge();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Chiến dịch :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = chienDich.TenChienDich;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Danh Bạ :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = danhBaName ?? "NULL";
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = totalSms;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao gửi thành công :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = successSms;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao gửi không thành công :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = failedSms;
                    currentRow += 3;

                }

                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 45;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 45;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 20;

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        public async Task<byte[]> ExportThongKeTheoThang(ExportSmsLogTheoThangDto dto)
        {
            _logger.LogInformation($"{nameof(ExportThongKeTheoThang)} dto={JsonSerializer.Serialize(dto)}");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Thống Kê");

                int currentRow = 1;

                var titleCell = worksheet.Cell(currentRow, 1);
                titleCell.Value = $"THỐNG KÊ CÁC CHIẾN DỊCH GỬI TIN NHẮN THÁNG {dto.Thang} NĂM {dto.Nam}";
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(currentRow, 1, currentRow, 8).Merge();
                currentRow += 3;

                var chienDichLogsTheoThang = await _smDbContext.ChienDichLogTrangThaiGuis
                    .Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Month == dto.Thang && x.CreatedDate.Value.Year == dto.Nam && !x.Deleted)
                    .ToListAsync();

                if (!chienDichLogsTheoThang.Any())
                {
                    var noDataCell = worksheet.Cell(currentRow, 1);
                    noDataCell.Value = "Không có dữ liệu";
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        return memoryStream.ToArray();
                    };
                }

                var groupByChienDich = chienDichLogsTheoThang.GroupBy(x => x.IdChienDich);

                foreach (var chienDichGroup in groupByChienDich)
                {
                    var idChienDich = chienDichGroup.Key;
                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

                    if (chienDich == null)
                        continue;

                    var chiTietLogs = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich && x.CreatedDate.HasValue && x.CreatedDate.Value.Month == dto.Thang && x.CreatedDate.Value.Year == dto.Nam && !x.Deleted)
                        .Join(_smDbContext.BrandName, log => log.IdBrandName, bn => bn.Id, (log, bn) => new { log, bn })
                        .ToListAsync();

                    if (!chiTietLogs.Any())
                        continue;

                    var headerRow = currentRow;
                    worksheet.Cell(currentRow, 1).Value = "Stt";
                    worksheet.Cell(currentRow, 2).Value = "Tên Chiến Dịch";
                    worksheet.Cell(currentRow, 3).Value = "Họ và Tên";
                    worksheet.Cell(currentRow, 4).Value = "Số điện thoại";
                    worksheet.Cell(currentRow, 5).Value = "BrandName";
                    worksheet.Cell(currentRow, 6).Value = "Nội Dung Chi Tiết";
                    worksheet.Cell(currentRow, 7).Value = "Trạng Thái";
                    worksheet.Cell(currentRow, 8).Value = "Thời Gian Gửi";

                    for (int col = 1; col <= 8; col++)
                    {
                        var headerCell = worksheet.Cell(headerRow, col);
                        headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    currentRow++;

                    int stt = 1;
                    foreach (var chiTiet in chiTietLogs)
                    {
                        worksheet.Cell(currentRow, 1).Value = stt;
                        worksheet.Cell(currentRow, 2).Value = chienDich.TenChienDich;
                        worksheet.Cell(currentRow, 3).Value = chiTiet.log.IdDanhBaSms.HasValue ?
                            _smDbContext.DanhBaSms.FirstOrDefault(x => x.Id == chiTiet.log.IdDanhBaSms)?.HoVaTen ?? "" : "";
                        worksheet.Cell(currentRow, 4).Value = chiTiet.log.SoDienThoai;
                        worksheet.Cell(currentRow, 5).Value = chiTiet.bn.TenBrandName;
                        worksheet.Cell(currentRow, 6).Value = chiTiet.log.NoiDungChiTiet;
                        worksheet.Cell(currentRow, 7).Value = chiTiet.log.TrangThai;
                        worksheet.Cell(currentRow, 8).Value = chiTiet.log.CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss");

                        currentRow++;
                        stt++;
                    }

                    currentRow++;

                    var chienDichLogsOfMonth = chienDichGroup.ToList();
                    var totalSms = chienDichLogsOfMonth.Sum(x => x.TongSoSms);
                    var successSms = chienDichLogsOfMonth.Sum(x => x.SmsSendSuccess);
                    var failedSms = chienDichLogsOfMonth.Sum(x => x.SmsSendFailed);
                    var danhBa = chienDichLogsOfMonth.FirstOrDefault()?.IdDanhBa;
                    var danhBaName = danhBa.HasValue ?
                        _smDbContext.DanhBas.FirstOrDefault(x => x.Id == danhBa)?.TenDanhBa : null;

                    var statsHeaderCell = worksheet.Cell(currentRow, 1);
                    statsHeaderCell.Value = "THỐNG KÊ";
                    statsHeaderCell.Style.Font.Bold = true;
                    statsHeaderCell.Style.Font.FontSize = 14;
                    statsHeaderCell.Style.Fill.BackgroundColor = XLColor.FromArgb(70, 130, 180);
                    statsHeaderCell.Style.Font.FontColor = XLColor.White;
                    statsHeaderCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(currentRow, 1, currentRow, 2).Merge();
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Chiến dịch :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = chienDich.TenChienDich;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Danh Bạ :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = danhBaName ?? "NULL";
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = totalSms;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao gửi thành công :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = successSms;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Tổng số thuê bao gửi không thành công :";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = failedSms;
                    currentRow += 3;
                }

                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 45;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 45;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 20;

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}