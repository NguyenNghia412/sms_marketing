using AutoMapper;
using ClosedXML.Excel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
using thongbao.be.domain.Auth;
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
        private readonly UserManager<AppUser> _userManager;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public GuiTinNhanLogService(
             SmDbContext smDbContext,
            ILogger<GuiTinNhanLogService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IMapper mapper
            ) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
        }

        public BaseResponsePagingDto<ViewChienDichLogDto> PagingChienDichLog(FindPagingChienDichLogDto dto)
        {
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            _logger.LogInformation($"{nameof(PagingChienDichLog)} dto={JsonSerializer.Serialize(dto)}");
            var query = from clog in _smDbContext.ChienDichLogTrangThaiGuis
                        where !clog.Deleted && (isSuperAdmin || clog.CreatedBy == currentUserId)
                              && (!dto.FromDate.HasValue || (clog.CreatedDate.HasValue && clog.CreatedDate.Value.Date >= dto.FromDate.Value.Date))
                              && (!dto.ToDate.HasValue || (clog.CreatedDate.HasValue && clog.CreatedDate.Value.Date <= dto.ToDate.Value.Date))
                        join cd in _smDbContext.ChienDiches on clog.IdChienDich equals cd.Id
                        where !cd.Deleted && (isSuperAdmin || cd.CreatedBy == currentUserId)
                        join db in _smDbContext.DanhBas on clog.IdDanhBa equals db.Id into dbGroup
                        from db in dbGroup.DefaultIfEmpty()
                        join u in _userManager.Users on cd.CreatedBy equals u.Id
                        where !cd.Deleted && !clog.Deleted && (db == null || !db.Deleted)
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || cd.TenChienDich.Contains(dto.Keyword)
                                  || clog.NoiDung.Contains(dto.Keyword)
                                  || (db != null && db.TenDanhBa.Contains(dto.Keyword)))
                        select new ViewChienDichLogDto
                        {
                            IdChienDich = cd.Id,
                            TenChienDich = cd.TenChienDich,
                            TongSoSms = clog.TongSoSms,
                            SmsSentSuccess = clog.SmsSendSuccess,
                            SmsSentFailed = clog.SmsSendFailed,
                            NoiDung = clog.NoiDung,
                            TrangThai = clog.TrangThai,
                            TongChiPhi = isSuperAdmin ? clog.TongChiPhi : null,
                            NgayGui = clog.CreatedDate,
                            danhBa = db != null ? new ViewDanhBaLogDto
                            {
                                IdDanhBa = db.Id,
                                TenDanhBa = db.TenDanhBa
                            } : null,
                            Users = new ChienDichLogCreatedByDto
                            {
                                Id = u.Id,
                                FullName = u.FullName,
                            },
                        };

   
            if (!string.IsNullOrEmpty(dto.SapXepTheo) && dto.SapXepTheo.Equals("ASC", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(x => x.NgayGui);
            }
            else
            {
                query = query.OrderByDescending(x => x.NgayGui);
            }

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
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);
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
                            join u in _userManager.Users on log.CreatedBy equals u.Id
                            where !dbs.Deleted && !log.Deleted && !bn.Deleted
                                  && log.IdChienDich == idChienDich
                                  && dbs.IdDanhBa == dto.idDanhBa
                                  && (string.IsNullOrEmpty(dto.TrangThai) || log.TrangThai == dto.TrangThai)
                                  && (string.IsNullOrEmpty(dto.Keyword)
                                      || dbs.HoVaTen.Contains(dto.Keyword)
                                      || log.SoDienThoai.Contains(dto.Keyword)
                                      || log.NoiDungChiTiet.Contains(dto.Keyword)
                                      || bn.TenBrandName.Contains(dto.Keyword))
                            orderby log.CreatedDate descending, log.Id descending
                            select new ViewDanhBaSmsLogDto
                            {
                                Id = dbs.Id,
                                HoVaTen = dbs.HoVaTen,
                                IdDanhBa = dbs.IdDanhBa,
                                IdDanhBaSms = dbs.Id,
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
                                    Price = isSuperAdmin ? log.Price : null,
                                    Code = log.Code,
                                    Message = log.Message,
                                    NgayGui = log.CreatedDate,
                                    SoLuongTinNhan = log.SoLuongTinNhan,
                                },
                                Users = new CreatedByGuiTinNhanLogDto
                                {
                                    Id = u.Id,
                                    //UserName = u.UserName ?? "",
                                    FullName = u.FullName,
                                    //SoDienThoai = u.PhoneNumber ?? "",
                                    //Email = u.Email ?? "",
                                },
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
                                  && (string.IsNullOrEmpty(dto.TrangThai) || log.TrangThai == dto.TrangThai)
                                  && (string.IsNullOrEmpty(dto.Keyword)
                                      || log.SoDienThoai.Contains(dto.Keyword)
                                      || log.NoiDungChiTiet.Contains(dto.Keyword)
                                      || bn.TenBrandName.Contains(dto.Keyword))
                            orderby log.CreatedDate descending, log.Id descending
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
                                    Price = isSuperAdmin ? log.Price : null,
                                    Code = log.Code,
                                    Message = log.Message,
                                    NgayGui = log.CreatedDate,
                                    SoLuongTinNhan = log.SoLuongTinNhan,
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
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
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
                worksheet.Range(currentRow, 1, currentRow, 11).Merge();
                currentRow += 3;

                
                worksheet.Cell(currentRow, 1).Value = "Tháng";
                worksheet.Cell(currentRow, 2).Value = "STT";
                worksheet.Cell(currentRow, 3).Value = "Tên Chiến Dịch";
                worksheet.Cell(currentRow, 4).Value = "Họ và Tên";
                worksheet.Cell(currentRow, 5).Value = "Số điện thoại";
                worksheet.Cell(currentRow, 6).Value = "BrandName";
                worksheet.Cell(currentRow, 7).Value = "Nội Dung Chi Tiết";
                worksheet.Cell(currentRow, 8).Value = "Trạng Thái";
                worksheet.Cell(currentRow, 9).Value = "Số Lượng Tin Nhắn";
                worksheet.Cell(currentRow, 10).Value = "Người Đặt Lệnh";
                worksheet.Cell(currentRow, 11).Value = "Thời Gian Gửi";

                for (int col = 1; col <= 11; col++)
                {
                    var headerCell = worksheet.Cell(currentRow, col);
                    headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerCell.Style.Font.Bold = true;
                }
                currentRow++;

                int sttTong = 1;

                foreach (var idChienDich in dto.idChienDichs)
                {
                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                    if (chienDich == null)
                        continue;

                    var chienDichLogs = await _smDbContext.ChienDichLogTrangThaiGuis
                        .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                        .ToListAsync();

                    if (!chienDichLogs.Any())
                        continue;

                    var chiTietLogs = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                        .Join(_smDbContext.BrandName, log => log.IdBrandName, bn => bn.Id, (log, bn) => new { log, bn })
                        .ToListAsync();

                    if (!chiTietLogs.Any())
                        continue;

                    foreach (var chiTiet in chiTietLogs)
                    {
                        worksheet.Cell(currentRow, 1).Value = chiTiet.log.CreatedDate.HasValue
                            ? $"Tháng {chiTiet.log.CreatedDate.Value.Month}/{chiTiet.log.CreatedDate.Value.Year}"
                            : "";
                        worksheet.Cell(currentRow, 2).Value = sttTong;
                        worksheet.Cell(currentRow, 3).Value = chienDich.TenChienDich;
                        worksheet.Cell(currentRow, 4).Value = chiTiet.log.IdDanhBaSms.HasValue ?
                            _smDbContext.DanhBaSms.FirstOrDefault(x => x.Id == chiTiet.log.IdDanhBaSms)?.HoVaTen ?? "" : "";
                        worksheet.Cell(currentRow, 5).Value = chiTiet.log.SoDienThoai;
                        worksheet.Cell(currentRow, 6).Value = chiTiet.bn.TenBrandName;
                        worksheet.Cell(currentRow, 7).Value = chiTiet.log.NoiDungChiTiet;
                        worksheet.Cell(currentRow, 8).Value = chiTiet.log.TrangThai;
                        worksheet.Cell(currentRow, 9).Value = chiTiet.log.SoLuongTinNhan;
                        worksheet.Cell(currentRow, 10).Value = !string.IsNullOrEmpty(chiTiet.log.CreatedBy) ?
                            _smDbContext.Users.FirstOrDefault(x => x.Id == chiTiet.log.CreatedBy)?.FullName ?? "" : "";
                        worksheet.Cell(currentRow, 11).Value = chiTiet.log.CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss");

                        currentRow++;
                        sttTong++;
                    }
                }

                // Column widths
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 8;
                worksheet.Column(3).Width = 40;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 18;
                worksheet.Column(6).Width = 18;
                worksheet.Column(7).Width = 45;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 20;
                worksheet.Column(10).Width = 22;
                worksheet.Column(11).Width = 22;

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
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            using (var workbook = new XLWorkbook())
            {
                // ===== SHEET 1: THỐNG KÊ CHIẾN DỊCH =====
                var worksheetChienDich = workbook.Worksheets.Add("Thống Kê");

                int cdRow = 1;
                var cdTitleCell = worksheetChienDich.Cell(cdRow, 1);
                cdTitleCell.Value = $"THỐNG KÊ CHIẾN DỊCH THÁNG {dto.Thang} NĂM {dto.Nam}";
                cdTitleCell.Style.Font.Bold = true;
                cdTitleCell.Style.Font.FontSize = 16;
                cdTitleCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cdTitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheetChienDich.Range(cdRow, 1, cdRow, 9).Merge();
                cdRow += 2;

                // Header Sheet 1
                worksheetChienDich.Cell(cdRow, 1).Value = "STT";
                worksheetChienDich.Cell(cdRow, 2).Value = "Nội dung chiến dịch";
                worksheetChienDich.Cell(cdRow, 3).Value = "Thời gian";
                worksheetChienDich.Cell(cdRow, 4).Value = "Người gửi";
                worksheetChienDich.Cell(cdRow, 5).Value = "Số lượt gửi";
                worksheetChienDich.Cell(cdRow, 6).Value = "Số lượng tin nhắn";
                worksheetChienDich.Cell(cdRow, 7).Value = "Đơn giá";
                worksheetChienDich.Cell(cdRow, 8).Value = "Chi phí";
                worksheetChienDich.Cell(cdRow, 9).Value = "Ghi chú";

                for (int col = 1; col <= 9; col++)
                {
                    var headerCell = worksheetChienDich.Cell(cdRow, col);
                    headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerCell.Style.Font.Bold = true;
                }
                cdRow++;

                // Lấy dữ liệu Sheet 1
                var chienDichLogsTheoThangForSheet1 = await _smDbContext.ChienDichLogTrangThaiGuis
                    .Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Month == dto.Thang && x.CreatedDate.Value.Year == dto.Nam && !x.Deleted)
                    .ToListAsync();

                var groupByChienDichForSheet1 = chienDichLogsTheoThangForSheet1.GroupBy(x => x.IdChienDich);

                int sttChienDich = 1;
                long tongChiPhiTatCa = 0;
                long tongSoLuotGuiTatCa = 0;
                long tongSoLuongTinNhanTatCa = 0;

                foreach (var chienDichGroup in groupByChienDichForSheet1)
                {
                    var idChienDich = chienDichGroup.Key;
                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                    if (chienDich == null)
                        continue;

                    var chienDichLogs = chienDichGroup.ToList();
                    var totalSms = chienDichLogs.Sum(x => x.TongSoSms);
                    var tongChiPhi = chienDichLogs.Sum(x => (long)x.TongChiPhi);

                    var tongSoLuongTinNhan = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich && x.CreatedDate.HasValue && x.CreatedDate.Value.Month == dto.Thang && x.CreatedDate.Value.Year == dto.Nam && !x.Deleted)
                        .SumAsync(x => x.SoLuongTinNhan);

                    // ===== TÍNH ĐƠN GIÁ =====
                    // Group by Price ngay trên DB → chỉ kéo về (Price, SoLuongTinNhan) đại diện của mỗi Price
                    var donGiaRaw = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich
                            && x.CreatedDate.HasValue
                            && x.CreatedDate.Value.Month == dto.Thang
                            && x.CreatedDate.Value.Year == dto.Nam
                            && !x.Deleted)
                        .GroupBy(x => x.Price)
                        .Select(g => new { Price = g.Key, SoLuongTinNhan = g.Min(x => x.SoLuongTinNhan) })
                        .ToListAsync();

                    // Distinct Price → chia Price / SoLuongTinNhan → distinct đơn giá → sort tăng dần
                    var donGia = string.Join(" - ", donGiaRaw
                        .Select(x => x.SoLuongTinNhan > 0 ? (double)x.Price / x.SoLuongTinNhan : 0)
                        .Distinct()
                        .Where(x => x > 0)
                        .OrderBy(x => x)
                        .Select(x => x % 1 == 0 ? ((long)x).ToString() : x.ToString("G")));
                    // =========================

                    var nguoiGui = !string.IsNullOrEmpty(chienDich.CreatedBy)
                        ? _smDbContext.Users.FirstOrDefault(x => x.Id == chienDich.CreatedBy)?.FullName ?? ""
                        : "";

                    worksheetChienDich.Cell(cdRow, 1).Value = sttChienDich;
                    worksheetChienDich.Cell(cdRow, 2).Value = chienDich.TenChienDich;
                    worksheetChienDich.Cell(cdRow, 3).Value = chienDich.CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheetChienDich.Cell(cdRow, 4).Value = nguoiGui;
                    worksheetChienDich.Cell(cdRow, 5).Value = totalSms;
                    worksheetChienDich.Cell(cdRow, 6).Value = tongSoLuongTinNhan;
                    worksheetChienDich.Cell(cdRow, 7).Value = donGia;
                    worksheetChienDich.Cell(cdRow, 8).Value = tongChiPhi;
                    worksheetChienDich.Cell(cdRow, 9).Value = "";

                    tongSoLuotGuiTatCa += totalSms;
                    tongSoLuongTinNhanTatCa += tongSoLuongTinNhan;
                    tongChiPhiTatCa += tongChiPhi;

                    cdRow++;
                    sttChienDich++;
                }

                // Dòng Tổng cộng - màu vàng
                worksheetChienDich.Cell(cdRow, 1).Value = "Tổng cộng";
                worksheetChienDich.Cell(cdRow, 5).Value = tongSoLuotGuiTatCa;
                worksheetChienDich.Cell(cdRow, 6).Value = tongSoLuongTinNhanTatCa;
                worksheetChienDich.Cell(cdRow, 8).Value = tongChiPhiTatCa;

                for (int col = 1; col <= 9; col++)
                {
                    var totalCell = worksheetChienDich.Cell(cdRow, col);
                    totalCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    totalCell.Style.Font.Bold = true;
                }

                // Column widths Sheet 1
                worksheetChienDich.Column(1).Width = 8;
                worksheetChienDich.Column(2).Width = 40;
                worksheetChienDich.Column(3).Width = 22;
                worksheetChienDich.Column(4).Width = 25;
                worksheetChienDich.Column(5).Width = 15;
                worksheetChienDich.Column(6).Width = 20;
                worksheetChienDich.Column(7).Width = 12;
                worksheetChienDich.Column(8).Width = 18;
                worksheetChienDich.Column(9).Width = 20;

                // ===== SHEET 2: THỐNG KÊ CHI TIẾT THEO LỆNH =====
                var worksheet = workbook.Worksheets.Add("Thống Kê Chi Tiết Theo Lệnh");

                int currentRow = 1;

                var titleCell = worksheet.Cell(currentRow, 1);
                titleCell.Value = $"THỐNG KÊ CÁC CHIẾN DỊCH GỬI TIN NHẮN THÁNG {dto.Thang} NĂM {dto.Nam}";
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(currentRow, 1, currentRow, 11).Merge();
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
                    }
                }

                var groupByChienDich = chienDichLogsTheoThang.GroupBy(x => x.IdChienDich);

                // Header 1 lần duy nhất - ngoài foreach
                worksheet.Cell(currentRow, 1).Value = "Tháng";
                worksheet.Cell(currentRow, 2).Value = "STT";
                worksheet.Cell(currentRow, 3).Value = "Tên Chiến Dịch";
                worksheet.Cell(currentRow, 4).Value = "Họ và Tên";
                worksheet.Cell(currentRow, 5).Value = "Số điện thoại";
                worksheet.Cell(currentRow, 6).Value = "BrandName";
                worksheet.Cell(currentRow, 7).Value = "Nội Dung Chi Tiết";
                worksheet.Cell(currentRow, 8).Value = "Trạng Thái";
                worksheet.Cell(currentRow, 9).Value = "Số Lượng Tin Nhắn";
                worksheet.Cell(currentRow, 10).Value = "Người Đặt Lệnh";
                worksheet.Cell(currentRow, 11).Value = "Thời Gian Gửi";

                for (int col = 1; col <= 11; col++)
                {
                    var headerCell = worksheet.Cell(currentRow, col);
                    headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerCell.Style.Font.Bold = true;
                }
                currentRow++;

                int sttTong = 1;

                foreach (var chienDichGroup in groupByChienDich)
                {
                    var idChienDich = chienDichGroup.Key;
                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                    if (chienDich == null)
                        continue;

                    var chiTietLogs = await _smDbContext.GuiTinNhanLogChiTiets
                        .Where(x => x.IdChienDich == idChienDich && x.CreatedDate.HasValue && x.CreatedDate.Value.Month == dto.Thang && x.CreatedDate.Value.Year == dto.Nam && !x.Deleted)
                        .Join(_smDbContext.BrandName, log => log.IdBrandName, bn => bn.Id, (log, bn) => new { log, bn })
                        .ToListAsync();

                    if (!chiTietLogs.Any())
                        continue;

                    foreach (var chiTiet in chiTietLogs)
                    {
                        worksheet.Cell(currentRow, 1).Value = $"Tháng {chiTiet.log.CreatedDate?.Month}/{chiTiet.log.CreatedDate?.Year}";
                        worksheet.Cell(currentRow, 2).Value = sttTong;
                        worksheet.Cell(currentRow, 3).Value = chienDich.TenChienDich;
                        worksheet.Cell(currentRow, 4).Value = chiTiet.log.IdDanhBaSms.HasValue ?
                            _smDbContext.DanhBaSms.FirstOrDefault(x => x.Id == chiTiet.log.IdDanhBaSms)?.HoVaTen ?? "" : "";
                        worksheet.Cell(currentRow, 5).Value = chiTiet.log.SoDienThoai;
                        worksheet.Cell(currentRow, 6).Value = chiTiet.bn.TenBrandName;
                        worksheet.Cell(currentRow, 7).Value = chiTiet.log.NoiDungChiTiet;
                        worksheet.Cell(currentRow, 8).Value = chiTiet.log.TrangThai;
                        worksheet.Cell(currentRow, 9).Value = chiTiet.log.SoLuongTinNhan;
                        worksheet.Cell(currentRow, 10).Value = !string.IsNullOrEmpty(chiTiet.log.CreatedBy) ?
                            _smDbContext.Users.FirstOrDefault(x => x.Id == chiTiet.log.CreatedBy)?.FullName ?? "" : "";
                        worksheet.Cell(currentRow, 11).Value = chiTiet.log.CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss");

                        currentRow++;
                        sttTong++;
                    }
                }

                // Column widths Sheet 2
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 8;
                worksheet.Column(3).Width = 40;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 18;
                worksheet.Column(6).Width = 18;
                worksheet.Column(7).Width = 45;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 20;
                worksheet.Column(10).Width = 22;
                worksheet.Column(11).Width = 22;

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}