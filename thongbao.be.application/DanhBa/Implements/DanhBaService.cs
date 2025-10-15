using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using EFCore.BulkExtensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.DanhBa.Interfaces;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.Constants.DanhBa;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using Volo.Abp.Users;
using static QRCoder.PayloadGenerator;

namespace thongbao.be.application.DanhBa.Implements
{
    public class DanhBaService : BaseService, IDanhBaService
    {
        private readonly IConfiguration _configuration;
        /*private readonly string[] Scopes = {
               "https://www.googleapis.com/auth/drive",
               "https://www.googleapis.com/auth/drive.file",
               "https://www.googleapis.com/auth/spreadsheets"
        };*/
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public DanhBaService(
            SmDbContext smDbContext,
            ILogger<DanhBaService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }


        public void Create(CreateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            if (dto.Type != TypeDanhBa.Sms && dto.Type != TypeDanhBa.Email)
            {
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
            var danhBa = new domain.DanhBa.DanhBa
            {
                TenDanhBa = dto.TenDanhBa,
                Mota = dto.Mota,
                GhiChu = dto.GhiChu,
                Type = dto.Type,
                CreatedDate = vietnamNow,
                CreatedBy = currentUserId,
            };
            _smDbContext.DanhBas.Add(danhBa);
            _smDbContext.SaveChanges();
        }

        public void Update(int idDanhBa, UpdateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} idDanhBa={idDanhBa}, dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var existingDanhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            existingDanhBa.TenDanhBa = dto.TenDanhBa;
            existingDanhBa.Mota = dto.Mota;
            existingDanhBa.GhiChu = dto.GhiChu;
            existingDanhBa.Type = dto.Type;
            _smDbContext.DanhBas.Update(existingDanhBa);
            _smDbContext.SaveChanges();
        }
        public void Delete(int idDanhBa)
        {
            _logger.LogInformation($"{nameof(Delete)} - Deleting DanhBa with ID: {idDanhBa}");
            var vietNamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var existingDanhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));

            var danhBaChiTietIds = _smDbContext.DanhBaSms
                .Where(dbct => dbct.IdDanhBa == idDanhBa  && (isSuperAdmin || dbct.CreatedBy == currentUserId) && !dbct.Deleted)
                .Select(dbct => dbct.Id)
                .ToList();

            if (danhBaChiTietIds.Any())
            {
                var danhBaDataList = _smDbContext.DanhBaDatas
                    .Where(dbd => danhBaChiTietIds.Contains(dbd.IdDanhBaChienDich) && !dbd.Deleted)
                    .ToList();

                foreach (var danhBaData in danhBaDataList)
                {
                    danhBaData.Deleted = true;
                    danhBaData.DeletedDate = vietNamNow;
                    danhBaData.DeletedBy = currentUserId;
                }

            }

            var danhBaTruongDataList = _smDbContext.DanhBaTruongDatas
                .Where(dbtd => dbtd.IdDanhBa == idDanhBa && !dbtd.Deleted)
                .ToList();

            foreach (var danhBaTruongData in danhBaTruongDataList)
            {
                danhBaTruongData.Deleted = true;
                danhBaTruongData.DeletedDate = vietNamNow;
                danhBaTruongData.DeletedBy = currentUserId;
            }

            var danhBaChiTietList = _smDbContext.DanhBaSms
                .Where(dbct => dbct.IdDanhBa == idDanhBa && !dbct.Deleted)
                .ToList();

            foreach (var danhBaChiTiet in danhBaChiTietList)
            {
                danhBaChiTiet.Deleted = true;
                danhBaChiTiet.DeletedDate = vietNamNow;
                danhBaChiTiet.DeletedBy = currentUserId;
            }

            existingDanhBa.Deleted = true;
            existingDanhBa.DeletedDate = vietNamNow;
            existingDanhBa.DeletedBy = currentUserId;

            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewDanhBaChiTietDto> FindDanhBaChiTiet(int idDanhBa, FindPagingDanhBaChiTietDto dto)
        {
            _logger.LogInformation($"{nameof(FindDanhBaChiTiet)} dto={JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                 ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            var query = from dbct in _smDbContext.DanhBaSms
                        where dbct.IdDanhBa == idDanhBa
                              && !dbct.Deleted
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || dbct.HoVaTen.Contains(dto.Keyword)
                                  || dbct.SoDienThoai.Contains(dto.Keyword))
                        orderby dbct.Id ascending
                        select dbct;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewDanhBaChiTietDto>>(data);
            var response = new BaseResponsePagingDto<ViewDanhBaChiTietDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
            return response;
        }
        public void DeleteDanhBaChiTiet(int idDanhBa, int idDanhBaChiTiet)
        {
            _logger.LogInformation($"{nameof(DeleteDanhBaChiTiet)}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                 ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            var danhBaChiTiet = _smDbContext.DanhBaSms.FirstOrDefault(x => x.Id == idDanhBaChiTiet && x.IdDanhBa == idDanhBa  && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhBaChiTietNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorDanhBaChiTietNotFound));
            danhBaChiTiet.Deleted = true;
            danhBaChiTiet.DeletedDate = vietnamNow;
            danhBaChiTiet.DeletedBy = currentUserId;
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewDanhBaDto> Find(FindPagingDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from db in _smDbContext.DanhBas
                        where !db.Deleted && (isSuperAdmin || db.CreatedBy == currentUserId)
                              && (string.IsNullOrEmpty(dto.Keyword)
                                  || db.TenDanhBa.Contains(dto.Keyword)
                                  || db.Mota.Contains(dto.Keyword))
                        orderby db.CreatedDate descending
                        select db;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewDanhBaDto>>(data);
            var danhBaIds = items.Select(x => x.Id).ToList();
            var soLuongDict = _smDbContext.DanhBaSms
                .Where(x => danhBaIds.Contains(x.IdDanhBa) && !x.Deleted)
                .GroupBy(x => x.IdDanhBa)
                .Select(g => new { IdDanhBa = g.Key, Count = g.Count() })
                .ToDictionary(x => x.IdDanhBa, x => x.Count);
            foreach (var item in items)
            {
                item.SoLuongNguoiNhan = soLuongDict.ContainsKey(item.Id) ? soLuongDict[item.Id] : 0;
            }
            var response = new BaseResponsePagingDto<ViewDanhBaDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
            return response;
        }
        public async Task<GetTruongDataDanhBaSmsResponseDto> GetTruongData( int idDanhBa)
        {
            _logger.LogInformation($"{nameof(GetTruongData)} idDanhBa={idDanhBa}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var danhBa = await _smDbContext.DanhBas.FirstOrDefaultAsync(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            var truongDataList = await _smDbContext.DanhBaTruongDatas
                 .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .ToListAsync();
           foreach(var truong in truongDataList)
            {
                truong.TenTruong = truong.TenTruong;
            }
            var response = new GetTruongDataDanhBaSmsResponseDto
            {
                TruongData = truongDataList.Select(x => new TruongDataItem
                {
                    Id = x.Id,
                    TenTruong = x.TenTruong,
                }).ToList()
            };
            return response;
        }
        public void CreateNguoiNhan(CreateNguoiNhanDto dto)
        {
            _logger.LogInformation($"{nameof(CreateNguoiNhan)} dto={JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == dto.IdDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            if (string.IsNullOrWhiteSpace(dto.HoVaTen))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }
            /*if (string.IsNullOrWhiteSpace(dto.MaSoNguoiDung))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }
            var existingMaSo = _smDbContext.DanhBaSms
                .Any(x => x.MaSoNguoiDung == dto.MaSoNguoiDung && !x.Deleted);
            if (existingMaSo)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorMaSoNguoiDungFound));
            }*/
            if (string.IsNullOrWhiteSpace(dto.SoDienThoai))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }

            if (dto.SoDienThoai.Length != 10 || !dto.SoDienThoai.All(char.IsDigit))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiInvalid, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiInvalid));
            }
            var existingSoDienThoai = _smDbContext.DanhBaSms
               .Any(x => x.SoDienThoai == dto.SoDienThoai && !x.Deleted);

            if (existingSoDienThoai)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound);
            }
            /*if (string.IsNullOrWhiteSpace(dto.EmailHuce))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }*/

            /*if (!System.Text.RegularExpressions.Regex.IsMatch(dto.EmailHuce, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))

            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorEmailInvalid, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorEmailInvalid));
            }
            var existingEmail = _smDbContext.DanhBaChiTiets
                .Any(x => x.EmailHuce.ToLower() == dto.EmailHuce.ToLower() && !x.Deleted);
            if (existingEmail)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorEmailFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorEmailFound));
            }*/

            var vietnamNow = GetVietnamTime();
            var nguoiNhan = new domain.DanhBa.DanhBaSms
            {
                IdDanhBa = dto.IdDanhBa,
                HoVaTen = dto.HoVaTen,
                //MaSoNguoiDung = dto.MaSoNguoiDung,
                SoDienThoai = dto.SoDienThoai,
                //EmailHuce = dto.EmailHuce,
                CreatedDate = vietnamNow,
                CreatedBy = currentUserId,
            };
            _smDbContext.DanhBaSms.Add(nguoiNhan);
            _smDbContext.SaveChanges();
        }
        public List<GetListDanhBaResponseDto> GetListDanhBa()
        {
            _logger.LogInformation($"{nameof(GetListDanhBa)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var query = from db in _smDbContext.DanhBas
                        where !db.Deleted && db.Type == 1 && (isSuperAdmin || db.CreatedBy == currentUserId)
                        orderby db.CreatedDate descending
                        select new GetListDanhBaResponseDto
                        {
                            Id = db.Id,
                            TenDanhBa = db.TenDanhBa + "(" + _smDbContext.DanhBaSms
                                                                .Where(dbs => dbs.IdDanhBa == db.Id && !dbs.Deleted)
                                                                .Count() + ")",
                            TruongData = (from truong in _smDbContext.DanhBaTruongDatas
                                          where truong.IdDanhBa == db.Id && !truong.Deleted
                                          select new GetTruongDanhBaDto
                                          {
                                              Id = truong.Id,
                                              TenTruong = truong.TenTruong
                                          }).ToList()
                        };
            var result = query.ToList();
            return result;
        }
        public async Task<byte[]> ExportDanhBaCungExcelTemplate()
        {
            _logger.LogInformation($"{nameof(ExportDanhBaCungExcelTemplate)}");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");




            var headers = new string[]
            {
                "STT", "Họ và tên(*)", "Họ đệm(*)", "Tên(*)", "Số điện thoại(*)","Email(*)",
                "Ngày sinh", "Giới tính", "Địa chỉ", "Loại người dùng(*)", "Mã số người dùng(*)", "Trạng thái hoạt động","Mã số tổ chức(*)"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // Set column widths
            worksheet.Column(1).Width = 5;   // STT
            worksheet.Column(2).Width = 20;  // Họ tên
            worksheet.Column(3).Width = 15;  // Họ đệm
            worksheet.Column(4).Width = 10;  // Tên
            worksheet.Column(5).Width = 20;  // Số điện thoại
            worksheet.Column(6).Width = 25;  // Email Huce
            worksheet.Column(7).Width = 20;  // Ngày sinh
            worksheet.Column(8).Width = 10;  // Giới tính
            worksheet.Column(9).Width = 30;  // Địa chỉ
            worksheet.Column(10).Width = 20; // Là người dùng
            worksheet.Column(11).Width = 20; // Mã số người dùng
            worksheet.Column(12).Width = 20; // Trạng thái hoạt động
            //worksheet.Column(13).Width = 25; // Tổ chức
            worksheet.Column(13).Width = 20; // Mã số tổ chức

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
        public async Task<byte[]> ExportDanhBaChiTietExcelTemplate()
        {
            _logger.LogInformation($"{nameof(ExportDanhBaChiTietExcelTemplate)}");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");





            var headers = new string[]
            {
                "STT", "Họ và tên(*)", "Số điện thoại(*)", "Trường dữ liệu 1","Trường dữ liệu 2","...","Trường dữ liệu n"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // Set column widths
            worksheet.Column(1).Width = 5;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 20;
            //worksheet.Column(4).Width = 20;
            //worksheet.Column(4).Width = 20;
            worksheet.Column(4).Width = 25;
            worksheet.Column(5).Width = 25;
            worksheet.Column(6).Width = 10;
            worksheet.Column(7).Width = 25;


            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
        /* public async Task<string> CreateDanhBaGoogleSheetTemplate()
         {
             _logger.LogInformation($"{nameof(CreateDanhBaGoogleSheetTemplate)}");

             var (sheetsService, driveService) = await InitializeGoogleServices();


             var spreadsheet = new Spreadsheet
             {
                 Properties = new SpreadsheetProperties
                 {
                     Title = $"Mẫu danh bạ người dùng import"
                 }
             };

             var createdSpreadsheet = await sheetsService.Spreadsheets.Create(spreadsheet).ExecuteAsync();
             var spreadsheetId = createdSpreadsheet.SpreadsheetId;
             await FormatTemplate(sheetsService, spreadsheetId);
             await SetPermissions(driveService, spreadsheetId);

             return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/edit";
         }

         private async Task<(SheetsService, DriveService)> InitializeGoogleServices()
         {
             var oauthSecretPath = _configuration["Google:OauthSecretPath"];
             var refreshToken = _configuration["Google:RefreshToken"];

             GoogleClientSecrets clientSecrets;
             using (var stream = new FileStream(oauthSecretPath, FileMode.Open, FileAccess.Read))
             {
                 clientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);
             }

             var credential = new UserCredential(
                 new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                 {
                     ClientSecrets = clientSecrets.Secrets,
                     Scopes = Scopes
                 }),
                 "user",
                 new TokenResponse
                 {
                     RefreshToken = refreshToken
                 });

             var sheetsService = new SheetsService(new BaseClientService.Initializer()
             {
                 HttpClientInitializer = credential,
                 ApplicationName = "SmsMarketing"
             });

             var driveService = new DriveService(new BaseClientService.Initializer()
             {
                 HttpClientInitializer = credential,
                 ApplicationName = "SmsMarketing"
             });

             return (sheetsService, driveService);
         }

         private async Task FormatTemplate(SheetsService sheetsService, string spreadsheetId)
         {
             var requests = new List<Request>();

             // Title
             requests.Add(new Request
             {
                 UpdateCells = new UpdateCellsRequest
                 {
                     Range = new GridRange { SheetId = 0, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = 14 },
                     Rows = new List<RowData>
             {
                 new RowData
                 {
                     Values = new List<CellData>
                     {
                         new CellData
                         {
                             UserEnteredValue = new ExtendedValue { StringValue = "IMPORT DANH BẠ NGƯỜI DÙNG" },
                             UserEnteredFormat = new Google.Apis.Sheets.v4.Data.CellFormat
                             {
                                 TextFormat = new TextFormat { FontSize = 18, Bold = true },
                                 HorizontalAlignment = "CENTER"
                             }
                         }
                     }
                 }
             },
                     Fields = "userEnteredValue,userEnteredFormat"
                 }
             });

             // Merge title cells
             requests.Add(new Request
             {
                 MergeCells = new MergeCellsRequest
                 {
                     Range = new GridRange { SheetId = 0, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = 14 }
                 }
             });

             var headers = new[] { "STT", "Họ tên(*)", "Họ đệm(*)", "Tên(*)", "Số điện thoại(*)", "Email Huce(*)",
                         "Ngày sinh", "Giới tính", "Địa chỉ", "Là người dùng(*)", "Mã số người dùng(*)", "Trạng thái hoạt động","Tổ chức(*)","Mã số tổ chức(*)" };

             var headerValues = headers.Select(h => new CellData
             {
                 UserEnteredValue = new ExtendedValue { StringValue = h },
                 UserEnteredFormat = new Google.Apis.Sheets.v4.Data.CellFormat
                 {
                     TextFormat = new TextFormat { Bold = true },
                     HorizontalAlignment = "CENTER",
                     BackgroundColor = new Google.Apis.Sheets.v4.Data.Color { Red = 0.8f, Green = 0.8f, Blue = 0.8f }
                 }
             }).ToList();

             requests.Add(new Request
             {
                 UpdateCells = new UpdateCellsRequest
                 {
                     Range = new GridRange { SheetId = 0, StartRowIndex = 3, EndRowIndex = 4, StartColumnIndex = 0, EndColumnIndex = 14 },
                     Rows = new List<RowData> { new RowData { Values = headerValues } },
                     Fields = "userEnteredValue,userEnteredFormat"
                 }
             });

             // Set specific column widths
             var columnWidths = new Dictionary<int, int>
             {
                 { 0, 40 },   
                 { 1, 160 },  
                 { 2, 120 },  
                 { 3, 80 },  
                 { 4, 160 },
                 { 5, 200 },  
                 { 6, 160 },  
                 { 7, 80 },   
                 { 8, 240 },  
                 { 9, 120 },  
                 { 10, 160 }, 
                 { 11, 160 }, 
                 { 12, 160 },
                 { 13, 120 }
             };

             foreach (var column in columnWidths)
             {
                 requests.Add(new Request
                 {
                     UpdateDimensionProperties = new UpdateDimensionPropertiesRequest
                     {
                         Range = new DimensionRange
                         {
                             SheetId = 0,
                             Dimension = "COLUMNS",
                             StartIndex = column.Key,
                             EndIndex = column.Key + 1
                         },
                         Properties = new DimensionProperties
                         {
                             PixelSize = column.Value
                         },
                         Fields = "pixelSize"
                     }
                 });
             }

             await sheetsService.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests }, spreadsheetId).ExecuteAsync();
         }
         private async Task SetPermissions(DriveService driveService, string spreadsheetId)
         {
             var permission = new Google.Apis.Drive.v3.Data.Permission
             {
                 Role = "writer",
                 Type = "anyone"
             };

             await driveService.Permissions.Create(permission, spreadsheetId).ExecuteAsync();
         }
         public async Task<GetRefreshTokenDto> GetGoogleRefreshToken()
         {
             _logger.LogInformation($"{nameof(GetGoogleRefreshToken)}");

             var oauthSecretPath = _configuration["Google:OauthSecretPath"];
             var gmail = _configuration["Google:Gmail"];

             GoogleClientSecrets clientSecrets;
             using (var stream = new FileStream(oauthSecretPath, FileMode.Open, FileAccess.Read))
             {
                 clientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);
             }

             var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                 clientSecrets.Secrets,
                 Scopes,
                 gmail,
                 CancellationToken.None,
                 new NullDataStore()
             );

             return new GetRefreshTokenDto
             {
                 RefreshToken = credential.Token.RefreshToken,
                 AccessToken = credential.Token.AccessToken,
                 ExpiresAt = credential.Token.IssuedUtc.AddSeconds(credential.Token.ExpiresInSeconds ?? 3600),
                 TokenType = credential.Token.TokenType
             };
         }*/
        public async Task<VerifyImportDanhBaCungResponseDto> VerifyImportAppendDanhBaCung(ImportAppendDanhBaCungDto dto)
        {
            _logger.LogInformation($"{nameof(VerifyImportAppendDanhBaCung)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");

            if (!await _checkGoogleSheetPermission(dto.Url))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid, ErrorMessages.GetMessage(ErrorCodes.GoogleSheetUrlErrorInvalid));
            }

            var sheetData = await _getSheetData(dto.Url, dto.SheetName);

            var headerRowIndex = dto.IndexRowHeader - 1;
            var startImportRowIndex = dto.IndexRowStartImport - 1;

            if (sheetData.Count <= headerRowIndex || headerRowIndex < 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid, string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }


            var expectedHeaders = new[]
            {
                 "STT", "Họ và tên(*)", "Họ đệm(*)", "Tên(*)", "Số điện thoại(*)","Email(*)",
                 "Ngày sinh", "Giới tính", "Địa chỉ", "Loại người dùng(*)", "Mã số người dùng(*)",
                 "Trạng thái hoạt động", "Tổ chức(*)", "Mã số tổ chức(*)"
            };

            var headerRow = sheetData[headerRowIndex];
            if (headerRow.Count < expectedHeaders.Length)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid, string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                if (headerRow[i]?.Trim() != expectedHeaders[i])
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid, string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }
            }


            var toChucDict = _smDbContext.ToChucs
                .Where(tc => !tc.Deleted)
                .AsNoTracking()
                .Select(tc => new { tc.TenToChuc, tc.MaSoToChuc })
                .ToList()
                .GroupBy(tc => $"{tc.MaSoToChuc?.Trim()}")
                .ToDictionary(g => g.Key, g => true);

            int totalRowsImported = 0;
            int totalDataImported = 0;

            for (int rowIndex = startImportRowIndex; rowIndex < sheetData.Count; rowIndex++)
            {
                var row = sheetData[rowIndex];
                var actualRowNumber = rowIndex + 1;


                if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                    continue;

                totalRowsImported++;


                for (int colIndex = 0; colIndex < Math.Min(row.Count, expectedHeaders.Length); colIndex++)
                {
                    var cellValue = row[colIndex];
                    if (!string.IsNullOrWhiteSpace(cellValue) ||
                        (cellValue != null && (cellValue.Trim().ToUpper() == "NULL" || cellValue.Trim().ToUpper() == "UNDEFINED")))
                    {
                        totalDataImported++;
                    }
                }


                var requiredColumnIndexes = new[] { 1, 2, 3, 4,5,9 ,10, 12 };

                foreach (var colIndex in requiredColumnIndexes)
                {
                    if (row.Count <= colIndex || string.IsNullOrWhiteSpace(row[colIndex]))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }
                }

                if (row.Count > 4 && !string.IsNullOrWhiteSpace(row[4]))
                {
                    var phoneNumber = row[4].Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$"))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                    }
                }

                if (row.Count > 5 && !string.IsNullOrWhiteSpace(row[5]))
                {
                    var email = row[5].Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportEmailErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportEmailErrorInvalid), actualRowNumber));
                    }
                }


                if (row.Count > 9 && !string.IsNullOrWhiteSpace(row[9]))
                {
                    var loaiNguoiDung = row[8].Trim().ToLower();
                    if (loaiNguoiDung != "sinh viên" && loaiNguoiDung != "nhân viên" && loaiNguoiDung != "sinh vien" && loaiNguoiDung != "nhan vien")
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportLoaiNguoiDungErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportLoaiNguoiDungErrorInvalid), actualRowNumber));
                    }
                }
                else
                {
                    throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                }


                string maSoToChuc = row.Count > 12 ? row[12]?.Trim() : "";

                if ( !string.IsNullOrWhiteSpace(maSoToChuc))
                {
               
                    var maSoToChucKey = $"|{maSoToChuc}";

                    bool toChucExists = toChucDict.Keys.Any(k => k.EndsWith(maSoToChucKey) && !string.IsNullOrWhiteSpace(maSoToChuc));

                    if (!toChucExists)
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportToChucErrorNotFound,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportToChucErrorNotFound), actualRowNumber));
                    }
                }
            }

            return new VerifyImportDanhBaCungResponseDto
            {
                TotalRowsImported = totalRowsImported,
                TotalDataImported = totalDataImported
            };
        }
        public async Task<ImportDanhBaCungResponseDto> ImportAppendDanhBaCung(ImportAppendDanhBaCungDto dto)
        {
            _logger.LogInformation($"{nameof(ImportAppendDanhBaCung)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int BATCH_SIZE = 500;
            const int MAX_CONCURRENT_BATCHES = 2;

            try
            {
                var sheetData = await _getSheetData(dto.Url, dto.SheetName);

                var existingMaSoNguoiDung = (await _smDbContext.DanhBaCungChiTiets
                    .Where(x => !x.Deleted)
                    .Select(x => x.MaSoNguoiDung)
                    .AsNoTracking()
                    .ToListAsync())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var toChucDict = await _smDbContext.ToChucs
                    .Where(tc => !tc.Deleted)
                    .AsNoTracking()
                    .Select(tc => new { tc.Id, tc.MaSoToChuc })
                    .ToListAsync();

                var toChucLookup = toChucDict
                    .GroupBy(tc => $"{tc.MaSoToChuc?.Trim().ToLower()}")
                    .ToDictionary(g => g.Key, g => g.First().Id);

                var gioiTinhMap = new Dictionary<string, GioiTinhEnum>(StringComparer.OrdinalIgnoreCase)
                {
                    { "nam", GioiTinhEnum.Nam },
                    { "nữ", GioiTinhEnum.Nu },
                    { "nu", GioiTinhEnum.Nu },
                    { "khác", GioiTinhEnum.Khac },
                    { "khac", GioiTinhEnum.Khac },
                    { "", GioiTinhEnum.ChuaXacDinh }
                };

                var loaiNguoiDungMap = new Dictionary<string, LoaiNguoiDungEnum>(StringComparer.OrdinalIgnoreCase)
                {
                    { "sinh viên", LoaiNguoiDungEnum.SinhVien },
                    { "sinh vien", LoaiNguoiDungEnum.SinhVien },
                    { "nhân viên", LoaiNguoiDungEnum.NhanVien },
                    { "nhan vien", LoaiNguoiDungEnum.NhanVien }
                };

                var trangThaiHoatDongMap = new Dictionary<string, TrangThaiHoatDongEnum>(StringComparer.OrdinalIgnoreCase)
                {
                    { "đang hoạt động", TrangThaiHoatDongEnum.DangHoatDong },
                    { "dang hoat dong", TrangThaiHoatDongEnum.DangHoatDong },
                    { "hoạt động", TrangThaiHoatDongEnum.DangHoatDong },
                    { "hoat dong", TrangThaiHoatDongEnum.DangHoatDong },
                    { "ngừng hoạt động", TrangThaiHoatDongEnum.NgungHoatDong },
                    { "ngung hoat dong", TrangThaiHoatDongEnum.NgungHoatDong },
                    { "không hoạt động", TrangThaiHoatDongEnum.NgungHoatDong },
                    { "khong hoat dong", TrangThaiHoatDongEnum.NgungHoatDong },
                    { "", TrangThaiHoatDongEnum.DangHoatDong }
                };

                var vietnamNow = GetVietnamTime();
                var allValidRecords = new List<domain.DanhBa.DanhBaCungChiTiet>();
                var allOrgMappings = new List<(string MaSoNguoiDung, int ToChucId)>();
                var totalDataImported = 0;

                for (int rowIndex = dto.IndexRowStartImport; rowIndex < sheetData.Count; rowIndex++)
                {
                    var row = sheetData[rowIndex];
                    var actualRowNumber = rowIndex + 1;

                    // Skip empty rows
                    if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                        continue;

                    var maSoNguoiDung = row.Count > 10 ? row[10]?.Trim() : "";

                    if (existingMaSoNguoiDung.Contains(maSoNguoiDung))
                        continue;

                    try
                    {

                        string maSoToChuc = row.Count > 12 ? row[12]?.Trim()?.ToLower() : "";
                        string toChucKey = $"|{maSoToChuc}"; 

                        if (!toChucLookup.TryGetValue(toChucKey, out int toChucId))
                        {
                            var partialMatch = toChucLookup.Keys.FirstOrDefault(k =>
                                (!string.IsNullOrWhiteSpace(maSoToChuc) && k.EndsWith($"|{maSoToChuc}")));

                            if (partialMatch != null)
                            {
                                toChucId = toChucLookup[partialMatch];
                            }
                            else
                            {
                                throw new UserFriendlyException(ErrorCodes.ImportToChucErrorNotFound,
                                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportToChucErrorNotFound), actualRowNumber));
                            }
                        }

                        // Parse enums
                        var gioiTinh = gioiTinhMap.GetValueOrDefault(row.Count > 7 ? row[7]?.Trim() ?? "" : "", GioiTinhEnum.ChuaXacDinh);
                        var loaiNguoiDung = loaiNguoiDungMap.GetValueOrDefault(row.Count > 9 ? row[9]?.Trim() ?? "" : "", LoaiNguoiDungEnum.Khac);
                        var trangThaiHoatDong = trangThaiHoatDongMap.GetValueOrDefault(row.Count > 11 ? row[11]?.Trim() ?? "" : "", TrangThaiHoatDongEnum.DangHoatDong);

                        // Parse ngày sinh
                        DateTime? ngaySinh = null;
                        if (row.Count > 6 && !string.IsNullOrWhiteSpace(row[6]))
                        {
                            DateTime.TryParse(row[6].Trim(), out var parsedDate);
                            ngaySinh = parsedDate == default ? null : parsedDate;
                        }

                        var newRecord = new domain.DanhBa.DanhBaCungChiTiet
                        {
                            HoVaTen = row.Count > 1 ? row[1]?.Trim() ?? "" : "",
                            HoDem = row.Count > 2 ? row[2]?.Trim() ?? "" : "",
                            Ten = row.Count > 3 ? row[3]?.Trim() ?? "" : "",
                            SoDienThoai = row.Count > 4 ? row[4]?.Trim() ?? "" : "",
                            Email = row.Count > 5 ? row[5]?.Trim() ?? "" : "",
                            NgaySinh = ngaySinh,
                            GioiTinh = gioiTinh,
                            DiaChi = row.Count > 8 ? row[8]?.Trim() : null,
                            LaNguoiDung = (int)loaiNguoiDung,
                            MaSoNguoiDung = maSoNguoiDung,
                            TrangThaiHoatDong = trangThaiHoatDong,
                            CreatedDate = vietnamNow,
                            Deleted = false
                        };

                        allValidRecords.Add(newRecord);
                        allOrgMappings.Add((maSoNguoiDung, toChucId));

                        var cellCount = Math.Min(row.Count, 13);
                        for (int colIndex = 0; colIndex < cellCount; colIndex++)
                        {
                            var cellValue = row[colIndex];
                            if (!string.IsNullOrWhiteSpace(cellValue) ||
                                (cellValue?.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) == true ||
                                 cellValue?.Trim().Equals("UNDEFINED", StringComparison.OrdinalIgnoreCase) == true))
                            {
                                totalDataImported++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing row {actualRowNumber}");
                        throw;
                    }
                }

                // 5. BATCH INSERT với SHORT TRANSACTIONS để tránh lock
                if (allValidRecords.Any())
                {
                    var insertedRecordsDict = new Dictionary<string, int>();

                    // Chia thành batches nhỏ hơn
                    var batches = allValidRecords
                        .Select((record, index) => new { record, index })
                        .GroupBy(x => x.index / BATCH_SIZE)
                        .Select(g => g.Select(x => x.record).ToList())
                        .ToList();

                    _logger.LogInformation($"Processing {batches.Count} batches of {BATCH_SIZE} records each");

                    // XỬ LÝ TUẦN TỰ TỪNG BATCH với SHORT TRANSACTION
                    for (int batchIndex = 0; batchIndex < batches.Count; batchIndex++)
                    {
                        var batch = batches[batchIndex];

                        // SỬ DỤNG SHORT TRANSACTION cho từng batch
                        using var batchTransaction = await _smDbContext.Database.BeginTransactionAsync();
                        try
                        {
                            // Insert batch records
                            await _smDbContext.BulkInsertAsync(batch, options =>
                            {
                                options.BatchSize = BATCH_SIZE;
                                options.BulkCopyTimeout = 60; // 1 minute timeout per batch
                                options.EnableStreaming = true; // Giảm memory usage
                            });

                            // Get inserted IDs cho batch này
                            var batchMaSoNguoiDungs = batch.Select(x => x.MaSoNguoiDung).ToList();
                            var batchInsertedRecords = await _smDbContext.DanhBaCungChiTiets
                                .Where(x => batchMaSoNguoiDungs.Contains(x.MaSoNguoiDung) && !x.Deleted)
                                .Select(x => new { x.Id, x.MaSoNguoiDung })
                                .AsNoTracking()
                                .ToListAsync();

                            // Cache inserted IDs
                            foreach (var record in batchInsertedRecords)
                            {
                                insertedRecordsDict[record.MaSoNguoiDung] = record.Id;
                            }

                            // Create org mappings cho batch này
                            var batchOrgMappings = allOrgMappings
                                .Where(mapping => batchMaSoNguoiDungs.Contains(mapping.MaSoNguoiDung))
                                .Select(mapping => new domain.ToChuc.ToChucDanhBaChiTiet
                                {
                                    IdToChuc = mapping.ToChucId,
                                    IdDanhBaNguoiDung = insertedRecordsDict[mapping.MaSoNguoiDung],
                                    CreatedDate = vietnamNow,
                                    Deleted = false
                                })
                                .ToList();

                            // Insert org mappings
                            if (batchOrgMappings.Any())
                            {
                                await _smDbContext.BulkInsertAsync(batchOrgMappings, options =>
                                {
                                    options.BatchSize = BATCH_SIZE;
                                    options.BulkCopyTimeout = 60;
                                    options.EnableStreaming = true;
                                });
                            }

                            await batchTransaction.CommitAsync();

                            _logger.LogInformation($"Completed batch {batchIndex + 1}/{batches.Count} ({batch.Count} records)");

                            if (batchIndex < batches.Count - 1)
                            {
                                await Task.Delay(50);
                            }
                        }
                        catch (Exception ex)
                        {
                            await batchTransaction.RollbackAsync();
                            _logger.LogError(ex, $"Error processing batch {batchIndex + 1}");
                            throw;
                        }
                    }
                }

                stopwatch.Stop();

                _logger.LogInformation($"Import completed: {allValidRecords.Count} records, {totalDataImported} data cells, {stopwatch.Elapsed.TotalSeconds:F2}s");

                return new ImportDanhBaCungResponseDto
                {
                    TotalRowsImported = allValidRecords.Count,
                    TotalDataImported = totalDataImported,
                    ImportTimeInSeconds = (int)stopwatch.Elapsed.TotalSeconds
                };
            }
            catch (Exception ex)
            {
                stopwatch?.Stop();
                _logger.LogError(ex, "Error importing DanhBa data");
                throw;
            }
        }
        public async Task<VerifyImportDanhBaChienDichResponseDto> VerifyImportDanhBaChienDich(ImportAppendDanhBaChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(VerifyImportDanhBaChienDich)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.IdDanhBa, dto.IndexColumnHoTen, dto.IndexColumnSoDienThoai })}");
            
            var vietNamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
           
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
            }

            if (string.IsNullOrWhiteSpace(dto.SheetName))
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelSheetNameErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelSheetNameErrorEmpty));
            }

            var danhBaExists = await _smDbContext.DanhBas
                .AnyAsync(x => x.Id == dto.IdDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

            if (!danhBaExists)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            }

            var excelData = await _readExcelFile(dto.File, dto.SheetName);

            var headerRowIndex = dto.IndexRowHeader - 1;
            var startImportRowIndex = dto.IndexRowStartImport - 1;

            if (excelData.Count <= headerRowIndex || headerRowIndex < 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            var headerRow = excelData[headerRowIndex];
            var hoTenColumnIndex = dto.IndexColumnHoTen - 1;
            var soDienThoaiColumnIndex = dto.IndexColumnSoDienThoai - 1;

            if (headerRow.Count <= hoTenColumnIndex || hoTenColumnIndex < 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            if (headerRow.Count <= soDienThoaiColumnIndex || soDienThoaiColumnIndex < 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            var existingSoDienThoai = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == dto.IdDanhBa && !x.Deleted)
                .Select(x => x.SoDienThoai)
                .AsNoTracking()
                .ToListAsync();

            var existingMaSoSet = new HashSet<string>(existingSoDienThoai, StringComparer.OrdinalIgnoreCase);
            var newMaSoSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int totalRowsImported = 0;
            int totalDataImported = 0;

            for (int rowIndex = startImportRowIndex; rowIndex < excelData.Count; rowIndex++)
            {
                var row = excelData[rowIndex];
                var actualRowNumber = rowIndex + 1;

                if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                {
                    continue;
                }

                totalRowsImported++;

                if (row.Count <= hoTenColumnIndex || string.IsNullOrWhiteSpace(row[hoTenColumnIndex]))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                }

                if (row.Count <= soDienThoaiColumnIndex || string.IsNullOrWhiteSpace(row[soDienThoaiColumnIndex]))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                }

                var phoneNumber = row[soDienThoaiColumnIndex].Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$"))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                }

                var soDienThoai = row[soDienThoaiColumnIndex].Trim();

                if (existingMaSoSet.Contains(soDienThoai))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorSoDienThoaiDuplicate), soDienThoai, actualRowNumber));
                }
                if (newMaSoSet.Contains(soDienThoai))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorSoDienThoaiDuplicate), soDienThoai, actualRowNumber));
                }

                newMaSoSet.Add(soDienThoai);
                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var cellValue = row[colIndex];
                    if (!string.IsNullOrWhiteSpace(cellValue) ||
                        (cellValue != null && (cellValue.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
                                              cellValue.Trim().Equals("UNDEFINED", StringComparison.OrdinalIgnoreCase))))
                    {
                        totalDataImported++;
                    }
                }
            }
            return new VerifyImportDanhBaChienDichResponseDto
            {
                TotalRowsImported = totalRowsImported,
                TotalDataImported = totalDataImported
            };
        }
        public async Task<ImportDanhBaChienDichResponseDto> ImportAppendDanhBaChienDich(ImportAppendDanhBaChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(ImportAppendDanhBaChienDich)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.IdDanhBa, dto.IndexColumnHoTen, dto.IndexColumnSoDienThoai })}");
            var vietNamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int BATCH_SIZE = 5000;

            try
            {
                if (dto.File == null || dto.File.Length == 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
                }

                var danhBaExists = await _smDbContext.DanhBas
                    .AnyAsync(x => x.Id == dto.IdDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                if (!danhBaExists)
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
                }

                var excelData = await _readExcelFile(dto.File, dto.SheetName);

                var headerRowIndex = dto.IndexRowHeader - 1;
                var startImportRowIndex = dto.IndexRowStartImport - 1;

                if (excelData.Count <= headerRowIndex || headerRowIndex < 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                var headerRow = excelData[headerRowIndex];
                var hoTenColumnIndex = dto.IndexColumnHoTen - 1;
                var soDienThoaiColumnIndex = dto.IndexColumnSoDienThoai - 1;

                if (headerRow.Count <= hoTenColumnIndex || hoTenColumnIndex < 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                if (headerRow.Count <= soDienThoaiColumnIndex || soDienThoaiColumnIndex < 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                var allHeaders = headerRow.Where(h => !string.IsNullOrWhiteSpace(h?.Trim())).Select(h => h.Trim()).ToList();

                var vietnamNow = GetVietnamTime();

                var existingDanhBaChiTiets = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == dto.IdDanhBa && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .AsNoTracking()
                    .ToListAsync();

                var existingSoDienThoaiDict = new Dictionary<string, int>(existingDanhBaChiTiets.Count, StringComparer.OrdinalIgnoreCase);
                foreach (var item in existingDanhBaChiTiets)
                {
                    existingSoDienThoaiDict[item.SoDienThoai] = item.Id;
                }
                var existingTruongData = await _smDbContext.DanhBaTruongDatas
                    .Where(x => x.IdDanhBa == dto.IdDanhBa && !x.Deleted)
                    .AsNoTracking()
                    .ToListAsync();

                var existingTruongDict = new Dictionary<string, int>(existingTruongData.Count, StringComparer.OrdinalIgnoreCase);
                foreach (var item in existingTruongData)
                {
                    existingTruongDict[item.TenTruong] = item.Id;
                }
                var totalRows = excelData.Count - startImportRowIndex;
                var newDanhBaChiTiets = new List<domain.DanhBa.DanhBaSms>(totalRows);
                var updateDanhBaChiTietIds = new HashSet<int>();
                var newTruongDatas = new List<domain.DanhBa.DanhBaTruongData>();

                var pendingDanhBaDataMappings = new List<(string SoDienThoai, string HeaderName, string CellValue)>(totalRows * allHeaders.Count);

                var newTruongHeaders = allHeaders.Where(h => !existingTruongDict.ContainsKey(h)).ToList();

                foreach (var header in newTruongHeaders)
                {
                    newTruongDatas.Add(new domain.DanhBa.DanhBaTruongData
                    {
                        IdDanhBa = dto.IdDanhBa,
                        TenTruong = header,
                        Type = "string",
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId,
                        Deleted = false
                    });
                }

                if (newTruongDatas.Any())
                {
                    await _smDbContext.BulkInsertAsync(newTruongDatas, options =>
                    {
                        options.BatchSize = BATCH_SIZE;
                        options.BulkCopyTimeout = 600;

                    });
                    var newInsertedTruongData = await _smDbContext.DanhBaTruongDatas
                        .Where(x => x.IdDanhBa == dto.IdDanhBa && newTruongHeaders.Contains(x.TenTruong) && !x.Deleted)
                        .Select(x => new { x.TenTruong, x.Id })
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var item in newInsertedTruongData)
                    {
                        existingTruongDict[item.TenTruong] = item.Id;
                    }
                }

                int totalRowsImported = 0;
                int totalDataImported = 0;

                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\d{10}$", System.Text.RegularExpressions.RegexOptions.Compiled);

                var updateDanhBaChiTiets = new Dictionary<int, domain.DanhBa.DanhBaSms>();

                for (int rowIndex = startImportRowIndex; rowIndex < excelData.Count; rowIndex++)
                {
                    var row = excelData[rowIndex];
                    var actualRowNumber = rowIndex + 1;

                    if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                        continue;

                    var hoVaTen = row.Count > hoTenColumnIndex ? row[hoTenColumnIndex]?.Trim() : "";
                    var soDienThoai = row.Count > soDienThoaiColumnIndex ? row[soDienThoaiColumnIndex]?.Trim() : "";

                    if (string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }

                    if (!phoneRegex.IsMatch(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                    }

                    totalRowsImported++;

                    for (int colIndex = 0; colIndex < Math.Min(row.Count, headerRow.Count); colIndex++)
                    {
                        var cellValue = row[colIndex];
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            totalDataImported++;
                        }
                    }
                    if (existingSoDienThoaiDict.TryGetValue(soDienThoai, out int existingId))
                    {
                        updateDanhBaChiTietIds.Add(existingId);

                        updateDanhBaChiTiets[existingId] = new domain.DanhBa.DanhBaSms
                        {
                            Id = existingId,
                            IdDanhBa = dto.IdDanhBa,
                            HoVaTen = hoVaTen,
                            SoDienThoai = soDienThoai,
                            CreatedBy = currentUserId,

                        };
                    }
                    else
                    {
                        var newRecord = new domain.DanhBa.DanhBaSms
                        {
                            IdDanhBa = dto.IdDanhBa,
                            HoVaTen = hoVaTen,
                            SoDienThoai = soDienThoai,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId,
                            Deleted = false
                        };
                        newDanhBaChiTiets.Add(newRecord);
                        existingSoDienThoaiDict[soDienThoai] = -newDanhBaChiTiets.Count;
                    }
                    for (int colIndex = 0; colIndex < Math.Min(row.Count, headerRow.Count); colIndex++)
                    {
                        var headerName = headerRow[colIndex]?.Trim();
                        if (string.IsNullOrWhiteSpace(headerName)) continue;

                        var cellValue = row[colIndex]?.Trim() ?? "";
                        pendingDanhBaDataMappings.Add((soDienThoai, headerName, cellValue));
                    }
                }
                using var transaction = await _smDbContext.Database.BeginTransactionAsync();
                try
                {
                    if (newDanhBaChiTiets.Any())
                    {
                        await _smDbContext.BulkInsertAsync(newDanhBaChiTiets, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 600;

                        });

                        var newMaSoList = newDanhBaChiTiets.Select(x => x.SoDienThoai).ToList();
                        var insertedRecords = await _smDbContext.DanhBaSms
                            .Where(x => x.IdDanhBa == dto.IdDanhBa && newMaSoList.Contains(x.SoDienThoai) && !x.Deleted)
                            .Select(x => new { x.Id, x.SoDienThoai })
                            .AsNoTracking()
                            .ToListAsync();
                        foreach (var record in insertedRecords)
                        {
                            existingSoDienThoaiDict[record.SoDienThoai] = record.Id;
                        }
                    }

                    if (updateDanhBaChiTiets.Any())
                    {
                        await _smDbContext.BulkUpdateAsync(updateDanhBaChiTiets.Values.ToList(), options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 600;
                        });

                        if (updateDanhBaChiTietIds.Any())
                        {
                            await _smDbContext.DanhBaDatas
                                .Where(x => updateDanhBaChiTietIds.Contains(x.IdDanhBaChienDich) && !x.Deleted)
                                .BatchUpdateAsync(x => new domain.DanhBa.DanhBaData
                                {
                                    Deleted = true,
                                    DeletedDate = vietnamNow,
                                    DeletedBy = currentUserId
                                });
                        }
                    }


                    var finalDanhBaDatas = new List<domain.DanhBa.DanhBaData>(pendingDanhBaDataMappings.Count);

                    foreach (var mapping in pendingDanhBaDataMappings)
                    {
                        if (existingSoDienThoaiDict.TryGetValue(mapping.SoDienThoai, out int danhBaChiTietId) &&
                            existingTruongDict.TryGetValue(mapping.HeaderName, out int truongDataId))
                        {
                            finalDanhBaDatas.Add(new domain.DanhBa.DanhBaData
                            {
                                Data = mapping.CellValue,
                                IdTruongData = truongDataId,
                                IdDanhBaChiTiet = danhBaChiTietId,
                                IdDanhBaChienDich = dto.IdDanhBa,
                                CreatedDate = vietnamNow,
                                CreatedBy = currentUserId,
                                Deleted = false
                            });
                        }
                    }
                    if (finalDanhBaDatas.Any())
                    {
                        await _smDbContext.BulkInsertAsync(finalDanhBaDatas, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 600;

                        });
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during bulk import transaction");
                    throw;
                }

                stopwatch.Stop();

                _logger.LogInformation($"Import completed: {totalRowsImported} rows, {totalDataImported} data cells, {stopwatch.Elapsed.TotalSeconds:F2}s");

                return new ImportDanhBaChienDichResponseDto
                {
                    TotalRowsImported = totalRowsImported,
                    TotalDataImported = totalDataImported,
                    ImportTimeInSeconds = (int)stopwatch.Elapsed.TotalSeconds
                };
            }
            catch (Exception ex)
            {
                stopwatch?.Stop();
                _logger.LogError(ex, "Error importing DanhBa ChienDich data");
                throw;
            }
        }
        public async Task<VerifyImportDanhBaChienDichResponseDto> VerifyImportCreateDanhBaSms(ImportDanhBaSmsDto dto)
        {
            _logger.LogInformation($"{nameof(VerifyImportCreateDanhBaSms)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.TenDanhBa, dto.Type, dto.IndexColumnHoTen, dto.InDexColumnSoDienThoai })}");
            var vietNamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            if (dto.Type != TypeDanhBa.Sms && dto.Type != TypeDanhBa.Email)
            {
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
            }

            if (string.IsNullOrWhiteSpace(dto.SheetName))
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelSheetNameErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelSheetNameErrorEmpty));
            }

            var excelData = await _readExcelFile(dto.File, dto.SheetName);

            var headerRowIndex = dto.IndexRowHeader - 1;
            var startImportRowIndex = dto.IndexRowStartImport - 1;
            var hoTenColIndex = dto.IndexColumnHoTen - 1;
            var soDienThoaiColIndex = dto.InDexColumnSoDienThoai - 1;

            if (excelData.Count <= headerRowIndex || headerRowIndex < 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            var headerRow = excelData[headerRowIndex];

            if (hoTenColIndex < 0 || hoTenColIndex >= headerRow.Count)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            if (soDienThoaiColIndex < 0 || soDienThoaiColIndex >= headerRow.Count)
            {
                throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                    string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
            }

            var newSoDienThoaiSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int totalRowsImported = 0;
            int totalDataImported = 0;

            for (int rowIndex = startImportRowIndex; rowIndex < excelData.Count; rowIndex++)
            {
                var row = excelData[rowIndex];
                var actualRowNumber = rowIndex + 1;

                if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                {
                    continue;
                }

                if (row.Count <= hoTenColIndex || string.IsNullOrWhiteSpace(row[hoTenColIndex]))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                }

                if (row.Count <= soDienThoaiColIndex || string.IsNullOrWhiteSpace(row[soDienThoaiColIndex]))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                }

                var phoneNumber = row[soDienThoaiColIndex].Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$"))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                }

                if (newSoDienThoaiSet.Contains(phoneNumber))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorSoDienThoaiDuplicate), phoneNumber, actualRowNumber));
                }

                newSoDienThoaiSet.Add(phoneNumber);

                totalRowsImported++;

                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var cellValue = row[colIndex];
                    if (!string.IsNullOrWhiteSpace(cellValue) ||
                        (cellValue != null && (cellValue.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
                                              cellValue.Trim().Equals("UNDEFINED", StringComparison.OrdinalIgnoreCase))))
                    {
                        totalDataImported++;
                    }
                }
            }

            return new VerifyImportDanhBaChienDichResponseDto
            {
                TotalRowsImported = totalRowsImported,
                TotalDataImported = totalDataImported
            };
        }
        public async Task<ImportDanhBaChienDichResponseDto> ImportCreateDanhBaChienDich(ImportDanhBaSmsDto dto)
        {
            _logger.LogInformation($"{nameof(ImportCreateDanhBaChienDich)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.TenDanhBa, dto.Type, dto.IndexColumnHoTen, dto.InDexColumnSoDienThoai })}");
            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int BATCH_SIZE = 5000;

            try
            {
                if (dto.Type != TypeDanhBa.Sms && dto.Type != TypeDanhBa.Email)
                {
                    throw new UserFriendlyException(ErrorCodes.InternalServerError);
                }
                if (dto.File == null || dto.File.Length == 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
                }

                if (string.IsNullOrWhiteSpace(dto.SheetName))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportExcelSheetNameErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelSheetNameErrorEmpty));
                }

                if (string.IsNullOrWhiteSpace(dto.TenDanhBa))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
                }

                var excelDataVerify = await _readExcelFile(dto.File, dto.SheetName);

                var headerRowIndexVerify = dto.IndexRowHeader - 1;
                var startImportRowIndexVerify = dto.IndexRowStartImport - 1;
                var hoTenColIndex = dto.IndexColumnHoTen - 1;
                var soDienThoaiColIndex = dto.InDexColumnSoDienThoai - 1;

                if (excelDataVerify.Count <= headerRowIndexVerify || headerRowIndexVerify < 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                var headerRowVerify = excelDataVerify[headerRowIndexVerify];

                if (hoTenColIndex < 0 || hoTenColIndex >= headerRowVerify.Count)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                if (soDienThoaiColIndex < 0 || soDienThoaiColIndex >= headerRowVerify.Count)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                var newSoDienThoaiSetVerify = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int rowIndex = startImportRowIndexVerify; rowIndex < excelDataVerify.Count; rowIndex++)
                {
                    var row = excelDataVerify[rowIndex];
                    var actualRowNumber = rowIndex + 1;

                    if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                    {
                        continue;
                    }
                    if (row.Count <= hoTenColIndex || string.IsNullOrWhiteSpace(row[hoTenColIndex]))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }

                    if (row.Count <= soDienThoaiColIndex || string.IsNullOrWhiteSpace(row[soDienThoaiColIndex]))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }

                    var phoneNumber = row[soDienThoaiColIndex].Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$"))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                    }

                    if (newSoDienThoaiSetVerify.Contains(phoneNumber))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorSoDienThoaiDuplicate), phoneNumber, actualRowNumber));
                    }

                    newSoDienThoaiSetVerify.Add(phoneNumber);
                }
                //var vietnamNow = GetVietnamTime();

                var danhBa = new domain.DanhBa.DanhBa
                {
                    TenDanhBa = dto.TenDanhBa,
                    Mota = dto.Mota,
                    GhiChu = "",
                    Type = dto.Type,
                    CreatedDate = vietnamNow,
                    CreatedBy = currentUserId,
                };
                _smDbContext.DanhBas.Add(danhBa);
                await _smDbContext.SaveChangesAsync();

                var idDanhBa = danhBa.Id;
                var excelData = await _readExcelFile(dto.File, dto.SheetName);

                var headerRowIndex = dto.IndexRowHeader - 1;
                var startImportRowIndex = dto.IndexRowStartImport - 1;

                if (excelData.Count <= headerRowIndex || headerRowIndex < 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                var headerRow = excelData[headerRowIndex];
                var allHeaders = headerRow.Where(h => !string.IsNullOrWhiteSpace(h?.Trim())).Select(h => h.Trim()).ToList();

                var newTruongDatas = new List<domain.DanhBa.DanhBaTruongData>();

                foreach (var header in allHeaders)
                {
                    newTruongDatas.Add(new domain.DanhBa.DanhBaTruongData
                    {
                        IdDanhBa = idDanhBa,
                        TenTruong = header,
                        Type = "string",
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId,
                        Deleted = false
                    });
                }

                var existingTruongDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                if (newTruongDatas.Any())
                {
                    await _smDbContext.BulkInsertAsync(newTruongDatas, options =>
                    {
                        options.BatchSize = BATCH_SIZE;
                        options.BulkCopyTimeout = 600;
                    });
                    var newInsertedTruongData = await _smDbContext.DanhBaTruongDatas
                        .Where(x => x.IdDanhBa == idDanhBa && allHeaders.Contains(x.TenTruong) && !x.Deleted)
                        .Select(x => new { x.TenTruong, x.Id })
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var item in newInsertedTruongData)
                    {
                        existingTruongDict[item.TenTruong] = item.Id;
                    }
                }

                int totalRowsImported = 0;
                int totalDataImported = 0;

                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\d{10}$", System.Text.RegularExpressions.RegexOptions.Compiled);

                var totalRows = excelData.Count - startImportRowIndex;
                var newDanhBaChiTiets = new List<domain.DanhBa.DanhBaSms>(totalRows);
                var existingSoDienThoaiDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                var pendingDanhBaDataMappings = new List<(string SoDienThoai, string HeaderName, string CellValue)>(totalRows * allHeaders.Count);

                for (int rowIndex = startImportRowIndex; rowIndex < excelData.Count; rowIndex++)
                {
                    var row = excelData[rowIndex];
                    var actualRowNumber = rowIndex + 1;

                    if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                        continue;

                    var hoVaTen = row.Count > hoTenColIndex ? row[hoTenColIndex]?.Trim() : "";
                    var soDienThoai = row.Count > soDienThoaiColIndex ? row[soDienThoaiColIndex]?.Trim() : "";

                    if (string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }

                    if (!phoneRegex.IsMatch(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                    }

                    if (existingSoDienThoaiDict.ContainsKey(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorMaSoNguoiDungDuplicate), soDienThoai, actualRowNumber));
                    }

                    totalRowsImported++;

                    for (int colIndex = 0; colIndex < Math.Min(row.Count, headerRow.Count); colIndex++)
                    {
                        var cellValue = row[colIndex];
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            totalDataImported++;
                        }
                    }

                    var newRecord = new domain.DanhBa.DanhBaSms
                    {
                        IdDanhBa = idDanhBa,
                        HoVaTen = hoVaTen,
                        SoDienThoai = soDienThoai,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId,
                        Deleted = false
                    };
                    newDanhBaChiTiets.Add(newRecord);
                    existingSoDienThoaiDict[soDienThoai] = -newDanhBaChiTiets.Count;

                    for (int colIndex = 0; colIndex < Math.Min(row.Count, headerRow.Count); colIndex++)
                    {
                        var headerName = headerRow[colIndex]?.Trim();
                        if (string.IsNullOrWhiteSpace(headerName)) continue;

                        var cellValue = row[colIndex]?.Trim() ?? "";
                        pendingDanhBaDataMappings.Add((soDienThoai, headerName, cellValue));
                    }
                }

                using var transaction = await _smDbContext.Database.BeginTransactionAsync();
                try
                {
                    if (newDanhBaChiTiets.Any())
                    {
                        await _smDbContext.BulkInsertAsync(newDanhBaChiTiets, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 600;
                        });

                        var newSoDienThoaiList = newDanhBaChiTiets.Select(x => x.SoDienThoai).ToList();
                        var insertedRecords = await _smDbContext.DanhBaSms
                            .Where(x => x.IdDanhBa == idDanhBa && newSoDienThoaiList.Contains(x.SoDienThoai) && !x.Deleted)
                            .Select(x => new { x.Id, x.SoDienThoai })
                            .AsNoTracking()
                            .ToListAsync();

                        existingSoDienThoaiDict.Clear();
                        foreach (var record in insertedRecords)
                        {
                            existingSoDienThoaiDict[record.SoDienThoai] = record.Id;
                        }
                    }

                    var finalDanhBaDatas = new List<domain.DanhBa.DanhBaData>(pendingDanhBaDataMappings.Count);

                    foreach (var mapping in pendingDanhBaDataMappings)
                    {
                        if (existingSoDienThoaiDict.TryGetValue(mapping.SoDienThoai, out int danhBaChiTietId) &&
                            existingTruongDict.TryGetValue(mapping.HeaderName, out int truongDataId))
                        {
                            finalDanhBaDatas.Add(new domain.DanhBa.DanhBaData
                            {
                                Data = mapping.CellValue,
                                IdTruongData = truongDataId,
                                IdDanhBaChiTiet = danhBaChiTietId,
                                IdDanhBaChienDich = idDanhBa,
                                CreatedDate = vietnamNow,
                                CreatedBy = currentUserId,
                                Deleted = false
                            });
                        }
                    }

                    if (finalDanhBaDatas.Any())
                    {
                        await _smDbContext.BulkInsertAsync(finalDanhBaDatas, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 600;
                        });
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during bulk import transaction");
                    throw;
                }

                stopwatch.Stop();

                _logger.LogInformation($"Import completed: {totalRowsImported} rows, {totalDataImported} data cells, {stopwatch.Elapsed.TotalSeconds:F2}s");

                return new ImportDanhBaChienDichResponseDto
                {
                    TotalRowsImported = totalRowsImported,
                    TotalDataImported = totalDataImported,
                    ImportTimeInSeconds = (int)stopwatch.Elapsed.TotalSeconds
                };
            }
            catch (Exception ex)
            {
                stopwatch?.Stop();
                _logger.LogError(ex, "Error importing DanhBa ChienDich data");
                throw;
            }
        }
        public async Task CreateDanhBaSmsQuick(int idDanhBa, CreateDanhBaDataNhanhDto dto)
        {
            _logger.LogInformation($"{nameof(CreateDanhBaSmsQuick)} idDanhBa={idDanhBa}, dataCount={dto?.Data?.Count ?? 0}");

            var vietnamNow = GetVietnamTime();
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();

            var danhBaExists = await _smDbContext.DanhBas
                .AnyAsync(x => x.Id == idDanhBa && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

            if (!danhBaExists)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound,
                    ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            }
            var danhBaTruongDatas = await _smDbContext.DanhBaTruongDatas
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .AsNoTracking()
                .ToListAsync();

            if (!danhBaTruongDatas.Any())
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorTruongDataNotFound,
                    ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorTruongDataNotFound));
            }

            int hoVaTenIndex = dto.IndexTruongHoTen - 1;
            int soDienThoaiIndex = dto.IndexTruongSoDienThoai - 1;

            if (hoVaTenIndex < 0 || hoVaTenIndex >= danhBaTruongDatas.Count ||
                soDienThoaiIndex < 0 || soDienThoaiIndex >= danhBaTruongDatas.Count)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequiredFieldNotFound,
                    ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequiredFieldNotFound));
            }

            var existingSoDienThoai = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => x.SoDienThoai)
                .AsNoTracking()
                .ToListAsync();

            var existingSoDienThoaiSet = new HashSet<string>(existingSoDienThoai, StringComparer.OrdinalIgnoreCase);

            var newDanhBaSms = new List<domain.DanhBa.DanhBaSms>();
            var newDanhBaData = new List<domain.DanhBa.DanhBaData>();
            for (int rowIndex = 0; rowIndex < dto.Data.Count; rowIndex++)
            {
                var dataRow = dto.Data[rowIndex]; ;
                var row = dataRow.Split(',');
                if (row.Length != danhBaTruongDatas.Count)
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorDataColumnMismatch,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorDataColumnMismatch), rowIndex + 1));
                }

                var hoVaTen = row[hoVaTenIndex]?.Trim();
                var soDienThoai = row[soDienThoaiIndex]?.Trim();

                if (string.IsNullOrWhiteSpace(hoVaTen))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorHoVaTenRequired), rowIndex + 1));
                }

                if (string.IsNullOrWhiteSpace(soDienThoai))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiRequired), rowIndex + 1));
                }

                if (soDienThoai.Length != 10 || !soDienThoai.All(char.IsDigit))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiInvalidAtRow), rowIndex + 1));
                }
                if (existingSoDienThoaiSet.Contains(soDienThoai))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiFound), soDienThoai, rowIndex + 1));
                }

                existingSoDienThoaiSet.Add(soDienThoai);

                var danhBaSms = new domain.DanhBa.DanhBaSms
                {
                    IdDanhBa = idDanhBa,
                    HoVaTen = hoVaTen,
                    SoDienThoai = soDienThoai,
                    CreatedDate = vietnamNow,
                    CreatedBy = currentUserId,
                    Deleted = false
                };

                newDanhBaSms.Add(danhBaSms);
            }

            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            try
            {
                if (newDanhBaSms.Any())
                {
                    await _smDbContext.BulkInsertAsync(newDanhBaSms, options =>
                    {
                        options.BatchSize = 1000;
                        options.BulkCopyTimeout = 300;
                    });
                }

                var insertedSoDienThoaiList = newDanhBaSms.Select(x => x.SoDienThoai).ToList();
                var insertedDanhBaSms = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa && insertedSoDienThoaiList.Contains(x.SoDienThoai) && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .AsNoTracking()
                    .ToListAsync();

                var soDienThoaiToIdDict = insertedDanhBaSms.ToDictionary(x => x.SoDienThoai, x => x.Id);

                for (int rowIndex = 0; rowIndex < dto.Data.Count; rowIndex++)
                {
                    var dataRow = dto.Data[rowIndex];
                    var row = dataRow.Split(',');
                    var soDienThoai = row[soDienThoaiIndex]?.Trim();

                    if (soDienThoaiToIdDict.TryGetValue(soDienThoai ?? "", out int danhBaChiTietId))
                    {
                        for (int colIndex = 0; colIndex < row.Length; colIndex++)
                        {
                            var cellData = row[colIndex]?.Trim() ?? "";
                            var truongData = danhBaTruongDatas[colIndex];

                            var danhBaDataItem = new domain.DanhBa.DanhBaData
                            {
                                Data = cellData,
                                IdTruongData = truongData.Id,
                                IdDanhBaChiTiet = danhBaChiTietId,
                                IdDanhBaChienDich = idDanhBa,
                                CreatedDate = vietnamNow,
                                CreatedBy = currentUserId,
                                Deleted = false
                            };

                            newDanhBaData.Add(danhBaDataItem);
                        }
                    }
                }

                if (newDanhBaData.Any())
                {
                    await _smDbContext.BulkInsertAsync(newDanhBaData, options =>
                    {
                        options.BatchSize = 1000;
                        options.BulkCopyTimeout = 300;
                    });
                }

                await transaction.CommitAsync();

                _logger.LogInformation($"CreateDanhBaSmsQuick completed successfully. Inserted {newDanhBaSms.Count} DanhBaSms records and {newDanhBaData.Count} DanhBaData records");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during CreateDanhBaSmsQuick transaction");
                throw;
            }
        }
        public async Task CreateDanhBaChienDichNhanh(CreateDanhBaChienDichNhanhDto dto)
        {
            _logger.LogInformation($"{nameof(CreateDanhBaChienDichNhanh)} dto={JsonSerializer.Serialize(new { dto.TenDanhBa, dto.Type, TruongCount = dto.Truong?.Count ?? 0, DataCount = dto.Data?.Count ?? 0 })}");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int BATCH_SIZE = 5000;

            try
            {
                if (dto.Type != TypeDanhBa.Sms && dto.Type != TypeDanhBa.Email)
                {
                    throw new UserFriendlyException(ErrorCodes.InternalServerError);
                }

                if (string.IsNullOrWhiteSpace(dto.TenDanhBa))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired,
                        ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
                }

                if (dto.Truong == null || !dto.Truong.Any())
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired,
                        ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
                }

                if (dto.Data == null || !dto.Data.Any())
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired,
                        ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
                }

                var truongList = dto.Truong[0].Split(';').Select(t => t.Trim()).ToList();

                int hoVaTenIndex = -1;
                int soDienThoaiIndex = -1;
                //int maSoNguoiDungIndex = -1;

                for (int i = 0; i < truongList.Count; i++)
                {
                    var tenTruong = truongList[i].ToLower();

                    if (tenTruong.Contains("họ") && tenTruong.Contains("tên"))
                    {
                        hoVaTenIndex = i;
                    }
                    else if (tenTruong.Contains("số") && tenTruong.Contains("điện thoại"))
                    {
                        soDienThoaiIndex = i;
                    }
                    /*else if (tenTruong.Contains("mã số") && tenTruong.Contains("người dùng"))
                    {
                        maSoNguoiDungIndex = i;
                    }*/
                }

                if (hoVaTenIndex == -1 || soDienThoaiIndex == -1 /*|| maSoNguoiDungIndex == -1*/)
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequiredFieldNotFound,
                        ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequiredFieldNotFound));
                }

                var vietnamNow = GetVietnamTime();
                var isSuperAdmin = IsSuperAdmin();
                var currentUserId = getCurrentUserId();
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\d{10}$", System.Text.RegularExpressions.RegexOptions.Compiled);
                var existingSoDienThoaiSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int rowIndex = 0; rowIndex < dto.Data.Count; rowIndex++)
                {
                    var dataRow = dto.Data[rowIndex];
                    var row = dataRow.Split(';');
                    var actualRowNumber = rowIndex + 1;

                    if (row.Length != truongList.Count)
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorDataColumnMismatch,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorDataColumnMismatch), actualRowNumber));
                    }

                    var hoVaTen = row[hoVaTenIndex]?.Trim();
                    var soDienThoai = row[soDienThoaiIndex]?.Trim();
                    //var maSoNguoiDung = row[maSoNguoiDungIndex]?.Trim();

                    if (string.IsNullOrWhiteSpace(hoVaTen))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorHoVaTenRequired,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorHoVaTenRequired), actualRowNumber));
                    }

                    if (string.IsNullOrWhiteSpace(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiRequired,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiRequired), actualRowNumber));
                    }

                    if (!phoneRegex.IsMatch(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiInvalidAtRow,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiInvalidAtRow), actualRowNumber));
                    }

                    /*if (string.IsNullOrWhiteSpace(maSoNguoiDung))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungRequired,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorMaSoNguoiDungRequired), actualRowNumber));
                    }*/

                    if (existingSoDienThoaiSet.Contains(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiFound,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiFound), soDienThoai, actualRowNumber));
                    }

                    existingSoDienThoaiSet.Add(soDienThoai);
                }
                int idDanhBa;
                using (var danhBaTransaction = await _smDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var danhBa = new domain.DanhBa.DanhBa
                        {
                            TenDanhBa = dto.TenDanhBa,
                            Mota = "",
                            GhiChu = "",
                            Type = dto.Type,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId,
                            Deleted = false
                        };
                        _smDbContext.DanhBas.Add(danhBa);
                        await _smDbContext.SaveChangesAsync();
                        idDanhBa = danhBa.Id;

                        await danhBaTransaction.CommitAsync();
                        _logger.LogInformation($"Created DanhBa with ID: {idDanhBa}");
                    }
                    catch (Exception ex)
                    {
                        await danhBaTransaction.RollbackAsync();
                        _logger.LogError(ex, "Error creating DanhBa");
                        throw;
                    }
                }
                var truongDataDict = new Dictionary<string, int>(truongList.Count, StringComparer.OrdinalIgnoreCase);
                using (var truongTransaction = await _smDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var newTruongDatas = truongList.Select(tenTruong => new domain.DanhBa.DanhBaTruongData
                        {
                            IdDanhBa = idDanhBa,
                            TenTruong = tenTruong,
                            Type = "string",
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId,
                            Deleted = false
                        }).ToList();

                        await _smDbContext.BulkInsertAsync(newTruongDatas, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 300;
                        });

                        var insertedTruongData = await _smDbContext.DanhBaTruongDatas
                            .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                            .Select(x => new { x.TenTruong, x.Id })
                            .AsNoTracking()
                            .ToListAsync();

                        foreach (var item in insertedTruongData)
                        {
                            truongDataDict[item.TenTruong] = item.Id;
                        }

                        await truongTransaction.CommitAsync();
                        _logger.LogInformation($"Created {truongDataDict.Count} TruongData records");
                    }
                    catch (Exception ex)
                    {
                        await truongTransaction.RollbackAsync();
                        _logger.LogError(ex, "Error creating TruongData");
                        throw;
                    }
                }
                using (var dataTransaction = await _smDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var newDanhBaSms = new List<domain.DanhBa.DanhBaSms>(dto.Data.Count);

                        foreach (var dataRow in dto.Data)
                        {
                            var row = dataRow.Split(';');
                            var hoVaTen = row[hoVaTenIndex]?.Trim();
                            var soDienThoai = row[soDienThoaiIndex]?.Trim();
                            //var maSoNguoiDung = row[maSoNguoiDungIndex]?.Trim();

                            newDanhBaSms.Add(new domain.DanhBa.DanhBaSms
                            {
                                IdDanhBa = idDanhBa,
                                HoVaTen = hoVaTen ?? "",
                                SoDienThoai = soDienThoai ?? "",
                               // MaSoNguoiDung = maSoNguoiDung ?? "",
                                CreatedDate = vietnamNow,
                                CreatedBy = currentUserId,
                                Deleted = false
                            });
                        }

                        await _smDbContext.BulkInsertAsync(newDanhBaSms, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 300;
                        });

                        var insertedDanhBaSms = await _smDbContext.DanhBaSms
                            .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                            .Select(x => new { x.Id, x.SoDienThoai })
                            .AsNoTracking()
                            .ToListAsync();

                        var soDienThoaiToIdDict = insertedDanhBaSms.ToDictionary(x => x.SoDienThoai, x => x.Id, StringComparer.OrdinalIgnoreCase);

                        var newDanhBaData = new List<domain.DanhBa.DanhBaData>(dto.Data.Count * truongList.Count);

                        for (int rowIndex = 0; rowIndex < dto.Data.Count; rowIndex++)
                        {
                            var dataRow = dto.Data[rowIndex];
                            var row = dataRow.Split(';');
                            var soDienThoai = row[soDienThoaiIndex]?.Trim();

                            if (soDienThoaiToIdDict.TryGetValue(soDienThoai ?? "", out int danhBaChiTietId))
                            {
                                for (int colIndex = 0; colIndex < row.Length; colIndex++)
                                {
                                    var cellData = row[colIndex]?.Trim() ?? "";
                                    var tenTruong = truongList[colIndex];

                                    if (truongDataDict.TryGetValue(tenTruong, out int truongDataId))
                                    {
                                        newDanhBaData.Add(new domain.DanhBa.DanhBaData
                                        {
                                            Data = cellData,
                                            IdTruongData = truongDataId,
                                            IdDanhBaChiTiet = danhBaChiTietId,
                                            IdDanhBaChienDich = idDanhBa,
                                            CreatedDate = vietnamNow,
                                            CreatedBy = currentUserId,
                                            Deleted = false
                                        });
                                    }
                                }
                            }
                        }

                        await _smDbContext.BulkInsertAsync(newDanhBaData, options =>
                        {
                            options.BatchSize = BATCH_SIZE;
                            options.BulkCopyTimeout = 300;
                        });

                        await dataTransaction.CommitAsync();

                        _logger.LogInformation($"Created {newDanhBaSms.Count} DanhBaSms and {newDanhBaData.Count} DanhBaData records");
                    }
                    catch (Exception ex)
                    {
                        await dataTransaction.RollbackAsync();
                        _logger.LogError(ex, "Error creating DanhBaSms and DanhBaData");
                        throw;
                    }
                }

                stopwatch.Stop();
                _logger.LogInformation($"CreateDanhBaChienDichNhanh completed successfully in {stopwatch.Elapsed.TotalSeconds:F2}s. DanhBa ID: {idDanhBa}");
            }
            catch (Exception ex)
            {
                stopwatch?.Stop();
                _logger.LogError(ex, "Error in CreateDanhBaChienDichNhanh");
                throw;
            }
        }
        public async Task<GetFileExcelInforResponseDto> GetFileExcelInfor(GetFileExcelInforDto dto)
        {
            _logger.LogInformation($"{nameof(GetFileExcelInfor)}");

            if (dto.File == null || dto.File.Length == 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty,
                    ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
            }

            var result = new GetFileExcelInforResponseDto
            {
                Sheets = new List<SheetInfoDto>()
            };

            using var stream = new MemoryStream();
            await dto.File.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);

            foreach (var worksheet in workbook.Worksheets)
            {
                var sheetInfo = new SheetInfoDto
                {
                    SheetName = worksheet.Name,
                    Headers = new List<string>()
                };

                var firstRow = worksheet.FirstRowUsed();
                if (firstRow != null)
                {
                    var lastColumnUsed = firstRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

                    for (int col = 1; col <= lastColumnUsed; col++)
                    {
                        var cell = firstRow.Cell(col);
                        var headerValue = cell.IsEmpty() ? string.Empty : cell.GetValue<string>().Trim();
                        sheetInfo.Headers.Add(headerValue);
                    }
                }

                result.Sheets.Add(sheetInfo);
            }

            return result;
        }

        private async Task<List<List<string>>> _getSheetData(string sheetUrl, string sheetName)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            if (!Path.IsPathRooted(serviceAccountPath))
            {
                var basePath = AppContext.BaseDirectory;
                serviceAccountPath = Path.Combine(basePath, serviceAccountPath);
            }

            if (!File.Exists(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets.readonly");

            var service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API"
            });

            var spreadsheetId = _extractSpreadsheetId(sheetUrl);
            var range = sheetName;

            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            var responseData = new List<List<string>>();

            if (values != null && values.Any())
            {
                foreach (var row in values)
                {
                    var stringRow = row.Select(c => c?.ToString() ?? string.Empty).ToList();
                    responseData.Add(stringRow);
                }
            }

            return responseData;
        }
        private async Task<List<List<string>>> _readExcelFile(IFormFile file, string sheetName)
        {
            var result = new List<List<string>>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            using var workbook = new XLWorkbook(stream);
            IXLWorksheet worksheet;
            if (workbook.Worksheets.TryGetWorksheet(sheetName, out worksheet) == false)
            {
                if (workbook.Worksheets.Any())
                {
                    worksheet = workbook.Worksheets.First();
                }
                else
                {
                    throw new UserFriendlyException(ErrorCodes.ImportExcelSheetErrorNotFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportExcelSheetErrorNotFound), sheetName));
                }
            }
            var lastRowUsed = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            for (int rowNumber = 1; rowNumber <= lastRowUsed; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);
                var rowData = new List<string>();
                var lastColumnUsed = row.LastCellUsed()?.Address.ColumnNumber ?? 0;
                for (int col = 1; col <= Math.Max(lastColumnUsed, 20); col++)
                {
                    var cell = row.Cell(col);
                    var cellValue = string.Empty;
                    if (!cell.IsEmpty())
                    {
                        cellValue = cell.GetValue<string>();
                    }

                    rowData.Add(cellValue);
                }

                result.Add(rowData);
            }

            return result;
        }


        private string _extractSpreadsheetId(string sheetUrl)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sheetUrl, @"/spreadsheets/d/([a-zA-Z0-9-_]+)");
            if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid, ErrorMessages.GetMessage(ErrorCodes.GoogleSheetUrlErrorInvalid));
            }
            return match.Groups[1].Value;
        }
        /*
        private string GetColumnLetter(int columnIndex)
        {
            const int AlphabetLength = 26;
            string column = "";
            columnIndex++;

            while (columnIndex > 0)
            {
                int rem = (columnIndex - 1) % AlphabetLength;
                column = (char)(rem + 'A') + column;
                columnIndex = (columnIndex - 1) / AlphabetLength;
            }

            return column;
        }
        */
        private async Task<bool> _checkGoogleSheetPermission(string sheetUrl)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            if (!Path.IsPathRooted(serviceAccountPath))
            {
                var basePath = AppContext.BaseDirectory;
                serviceAccountPath = Path.Combine(basePath, serviceAccountPath);
            }

            if (!File.Exists(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
            }

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets");

            var service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API"
            });

            var spreadsheetId = _extractSpreadsheetId(sheetUrl);

            try
            {
                var request = service.Spreadsheets.Get(spreadsheetId);
                request.IncludeGridData = false;

                var response = await request.ExecuteAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}