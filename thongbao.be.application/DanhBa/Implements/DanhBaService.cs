using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
            _logger.LogInformation($"{nameof(Create)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var danhBa = new domain.DanhBa.DanhBa
            {
                TenDanhBa = dto.TenDanhBa,
                Mota = dto.Mota,
                GhiChu = dto.GhiChu,
                CreatedDate = vietnamNow,
            };
            _smDbContext.DanhBas.Add(danhBa);
            _smDbContext.SaveChanges();
        }

        public void Update(int idDanhBa, UpdateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} idDanhBa={idDanhBa}, dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var existingDanhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            existingDanhBa.TenDanhBa = dto.TenDanhBa;
            existingDanhBa.Mota = dto.Mota;
            existingDanhBa.GhiChu = dto.GhiChu;
            _smDbContext.DanhBas.Update(existingDanhBa);
            _smDbContext.SaveChanges();
        }
        public void Delete(int idDanhBa)
        {
            _logger.LogInformation($"{nameof(Delete)} - Deleting DanhBa with ID: {idDanhBa}");
            var vietNamNow = GetVietnamTime();

            var existingDanhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));

            var danhBaChiTietIds = _smDbContext.DanhBaChiTiets
                .Where(dbct => dbct.IdDanhBa == idDanhBa && !dbct.Deleted)
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
                }

            }

            var danhBaTruongDataList = _smDbContext.DanhBaTruongDatas
                .Where(dbtd => dbtd.IdDanhBa == idDanhBa && !dbtd.Deleted)
                .ToList();

            foreach (var danhBaTruongData in danhBaTruongDataList)
            {
                danhBaTruongData.Deleted = true;
                danhBaTruongData.DeletedDate = vietNamNow;
            }

            var danhBaChiTietList = _smDbContext.DanhBaChiTiets
                .Where(dbct => dbct.IdDanhBa == idDanhBa && !dbct.Deleted)
                .ToList();

            foreach (var danhBaChiTiet in danhBaChiTietList)
            {
                danhBaChiTiet.Deleted = true;
                danhBaChiTiet.DeletedDate = vietNamNow;
            }

            existingDanhBa.Deleted = true;
            existingDanhBa.DeletedDate = vietNamNow;

            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewDanhBaChiTietDto> FindDanhBaChiTiet(int idDanhBa, FindPagingDanhBaChiTietDto dto)
        {
            _logger.LogInformation($"{nameof(FindDanhBaChiTiet)} dto={JsonSerializer.Serialize(dto)}");
            var query = from dbct in _smDbContext.DanhBaChiTiets
                        where dbct.IdDanhBa == idDanhBa && !dbct.Deleted
                        orderby dbct.CreatedDate descending
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
            var danhBa = _smDbContext.DanhBas.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                 ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            var danhBaChiTiet = _smDbContext.DanhBaChiTiets.FirstOrDefault(x => x.Id == idDanhBaChiTiet && x.IdDanhBa == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhBaChiTietNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorDanhBaChiTietNotFound));
            danhBaChiTiet.Deleted = true;
            danhBaChiTiet.DeletedDate = vietnamNow;
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewDanhBaDto> Find(FindPagingDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var query = from db in _smDbContext.DanhBas
                        where !db.Deleted
                        orderby db.CreatedDate descending
                        select db;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewDanhBaDto>>(data);
            var response = new BaseResponsePagingDto<ViewDanhBaDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
            return response;
        }

        public void CreateNguoiNhan(CreateNguoiNhanDto dto)
        {
            _logger.LogInformation($"{nameof(CreateNguoiNhan)} dto={JsonSerializer.Serialize(dto)}");
            if (string.IsNullOrWhiteSpace(dto.HoVaTen))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }
            if (string.IsNullOrWhiteSpace(dto.MaSoNguoiDung))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }
            var existingMaSo = _smDbContext.DanhBaChiTiets
                .Any(x => x.MaSoNguoiDung == dto.MaSoNguoiDung && !x.Deleted);
            if (existingMaSo)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorMaSoNguoiDungFound));
            }
            if (string.IsNullOrWhiteSpace(dto.SoDienThoai))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }

            if (dto.SoDienThoai.Length != 10 || !dto.SoDienThoai.All(char.IsDigit))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorSoDienThoaiInvalid, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorSoDienThoaiInvalid));
            }
            if (string.IsNullOrWhiteSpace(dto.EmailHuce))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorRequired, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorRequired));
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.EmailHuce, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))

            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorEmailInvalid, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorEmailInvalid));
            }
            var existingEmail = _smDbContext.DanhBaChiTiets
                .Any(x => x.EmailHuce.ToLower() == dto.EmailHuce.ToLower() && !x.Deleted);
            if (existingEmail)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorEmailFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorEmailFound));
            }

            var vietnamNow = GetVietnamTime();
            var nguoiNhan = new domain.DanhBa.DanhBaChiTiet
            {
                IdDanhBa = dto.IdDanhBa,
                HoVaTen = dto.HoVaTen,
                MaSoNguoiDung = dto.MaSoNguoiDung,
                SoDienThoai = dto.SoDienThoai,
                EmailHuce = dto.EmailHuce,
                CreatedDate = vietnamNow,
            };
            _smDbContext.DanhBaChiTiets.Add(nguoiNhan);
            _smDbContext.SaveChanges();
        }
        public List<GetListDanhBaResponseDto> GetListDanhBa()
        {
            _logger.LogInformation($"{nameof(GetListDanhBa)}");

            var query = from db in _smDbContext.DanhBas
                        where !db.Deleted
                        orderby db.CreatedDate descending
                        select db;

            var data = query.ToList();
            var result = _mapper.Map<List<GetListDanhBaResponseDto>>(data);

            return result;
        }
        public async Task<byte[]> ExportDanhBaCungExcelTemplate()
        {
            _logger.LogInformation($"{nameof(ExportDanhBaCungExcelTemplate)}");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");


            worksheet.Cell(1, 1).Value = "IMPORT DANH BẠ NGƯỜI DÙNG";
            worksheet.Range(1, 1, 1, 14).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Row(1).Height = 25;


            var headers = new string[]
            {
                "STT", "Họ tên(*)", "Họ đệm(*)", "Tên(*)", "Số điện thoại(*)", "Email Huce(*)",
                "Ngày sinh", "Giới tính", "Địa chỉ", "Là người dùng(*)", "Mã số người dùng(*)", "Trạng thái hoạt động","Tổ chức(*)","Mã số tổ chức(*)"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
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
            worksheet.Column(10).Width = 15; // Là người dùng
            worksheet.Column(11).Width = 20; // Mã số người dùng
            worksheet.Column(12).Width = 20; // Trạng thái hoạt động
            worksheet.Column(13).Width = 25; // Tổ chức
            worksheet.Column(14).Width = 20; // Mã số tổ chức

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
        public async Task<byte[]> ExportDanhBaChiTietExcelTemplate()
        {
            _logger.LogInformation($"{nameof(ExportDanhBaChiTietExcelTemplate)}");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");


            worksheet.Cell(1, 1).Value = "IMPORT DANH BẠ NGƯỜI DÙNG THEO CHIẾN DỊCH";
            worksheet.Range(1, 1, 1, 9).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Row(1).Height = 25;


            var headers = new string[]
            {
                "STT", "Họ tên(*)", "Số điện thoại(*)", "Email Huce(*)",
                 "Mã số người dùng(*)", "Trường dữ liệu 1","Trường dữ liệu 2","...","Trường dữ liệu n"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
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
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 25;
            worksheet.Column(7).Width = 25;
            worksheet.Column(8).Width = 10;
            worksheet.Column(9).Width = 25;


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
                 "STT", "Họ tên(*)", "Họ đệm(*)", "Tên(*)", "Số điện thoại(*)", "Email Huce(*)",
                 "Ngày sinh", "Giới tính", "Địa chỉ", "Là người dùng(*)", "Mã số người dùng(*)",
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
                .GroupBy(tc => $"{tc.TenToChuc?.Trim()}|{tc.MaSoToChuc?.Trim()}")
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


                var requiredColumnIndexes = new[] { 1, 2, 3, 4, 5, 10, 12, 13 };

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
                        throw new UserFriendlyException(ErrorCodes.ImportEmailHuceErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportEmailHuceErrorInvalid), actualRowNumber));
                    }
                }


                if (row.Count > 9 && !string.IsNullOrWhiteSpace(row[9]))
                {
                    var loaiNguoiDung = row[9].Trim().ToLower();
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

                string tenToChuc = row.Count > 12 ? row[12]?.Trim() : "";
                string maSoToChuc = row.Count > 13 ? row[13]?.Trim() : "";

                if (!string.IsNullOrWhiteSpace(tenToChuc) || !string.IsNullOrWhiteSpace(maSoToChuc))
                {
                    var toChucKey = $"{tenToChuc}|{maSoToChuc}";
                    var tenToChucKey = $"{tenToChuc}|";
                    var maSoToChucKey = $"|{maSoToChuc}";

                    bool toChucExists = toChucDict.ContainsKey(toChucKey) ||
                                       toChucDict.Keys.Any(k => k.StartsWith(tenToChucKey) && !string.IsNullOrWhiteSpace(tenToChuc)) ||
                                       toChucDict.Keys.Any(k => k.EndsWith(maSoToChucKey) && !string.IsNullOrWhiteSpace(maSoToChuc));

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
                    .Select(tc => new { tc.Id, tc.TenToChuc, tc.MaSoToChuc })
                    .ToListAsync();

                var toChucLookup = toChucDict
                    .GroupBy(tc => $"{tc.TenToChuc?.Trim().ToLower()}|{tc.MaSoToChuc?.Trim().ToLower()}")
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
                        // Validate và lookup tổ chức
                        string tenToChuc = row.Count > 12 ? row[12]?.Trim()?.ToLower() : "";
                        string maSoToChuc = row.Count > 13 ? row[13]?.Trim()?.ToLower() : "";
                        var toChucKey = $"{tenToChuc}|{maSoToChuc}";

                        if (!toChucLookup.TryGetValue(toChucKey, out int toChucId))
                        {
                            var partialMatch = toChucLookup.Keys.FirstOrDefault(k =>
                                (!string.IsNullOrWhiteSpace(tenToChuc) && k.StartsWith($"{tenToChuc}|")) ||
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
                            EmailHuce = row.Count > 5 ? row[5]?.Trim() ?? "" : "",
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

                        var cellCount = Math.Min(row.Count, 14);
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
            _logger.LogInformation($"{nameof(VerifyImportDanhBaChienDich)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.IdDanhBa })}");

            if (dto.File == null || dto.File.Length == 0)
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
            }

            if (string.IsNullOrWhiteSpace(dto.SheetName))
            {
                throw new UserFriendlyException(ErrorCodes.ImportExcelSheetNameErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelSheetNameErrorEmpty));
            }

            var danhBaExists = await _smDbContext.DanhBas
                .AnyAsync(x => x.Id == dto.IdDanhBa && !x.Deleted);

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

            var requiredHeaders = new[]
            {
                "Họ tên(*)", "Số điện thoại(*)", "Email Huce(*)", "Mã số người dùng(*)"
            };

            var headerRow = excelData[headerRowIndex];

            var requiredHeaderIndexes = new Dictionary<string, int>();

            foreach (var requiredHeader in requiredHeaders)
            {
                var headerIndex = -1;
                for (int i = 0; i < headerRow.Count; i++)
                {
                    var currentHeader = headerRow[i]?.Trim();

                    if (currentHeader == requiredHeader)
                    {
                        headerIndex = i;
                        break;
                    }
                }

                if (headerIndex == -1)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                }

                requiredHeaderIndexes[requiredHeader] = headerIndex;
            }

            var existingMaSoNguoiDung = await _smDbContext.DanhBaChiTiets
                .Where(x => x.IdDanhBa == dto.IdDanhBa && !x.Deleted)
                .Select(x => x.MaSoNguoiDung)
                .AsNoTracking()
                .ToListAsync();

            var existingMaSoSet = new HashSet<string>(existingMaSoNguoiDung, StringComparer.OrdinalIgnoreCase);
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
                var requiredColumnIndexes = new[] { 1, 2, 3, 4 };

                foreach (var colIndex in requiredColumnIndexes)
                {
                    if (row.Count <= colIndex || string.IsNullOrWhiteSpace(row[colIndex]))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }
                }

                var phoneNumber = row[2].Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$"))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                }

                var email = row[3].Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    throw new UserFriendlyException(ErrorCodes.ImportEmailHuceErrorInvalid,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportEmailHuceErrorInvalid), actualRowNumber));
                }

                var maSoNguoiDung = row[4].Trim();

                if (existingMaSoSet.Contains(maSoNguoiDung))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorMaSoNguoiDungDuplicate), maSoNguoiDung, actualRowNumber));
                }
                if (newMaSoSet.Contains(maSoNguoiDung))
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorMaSoNguoiDungFound,
                        string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportDanhBaChienDichErrorMaSoNguoiDungDuplicate), maSoNguoiDung, actualRowNumber));
                }

                newMaSoSet.Add(maSoNguoiDung);
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
            _logger.LogInformation($"{nameof(ImportAppendDanhBaChienDich)} dto={JsonSerializer.Serialize(new { dto.IndexRowStartImport, dto.IndexRowHeader, dto.SheetName, dto.IdDanhBa })}");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int BATCH_SIZE = 5000;

            try
            {
                if (dto.File == null || dto.File.Length == 0)
                {
                    throw new UserFriendlyException(ErrorCodes.ImportExcelFileErrorEmpty, ErrorMessages.GetMessage(ErrorCodes.ImportExcelFileErrorEmpty));
                }

                var danhBaExists = await _smDbContext.DanhBas
                    .AnyAsync(x => x.Id == dto.IdDanhBa && !x.Deleted);

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

                var headerPatterns = new Dictionary<string, string[]>
                {
                    ["HoTen"] = new[] { "Họ tên(*)", "Ho ten(*)", "Họ tên", "Ho ten", "HoTen(*)", "HoTen" },
                    ["SoDienThoai"] = new[] { "Số điện thoại(*)", "So dien thoai(*)", "Số điện thoại", "So dien thoai", "SoDienThoai(*)", "SoDienThoai" },
                    ["EmailHuce"] = new[] { "Email Huce(*)", "Email Huce", "EmailHuce(*)", "EmailHuce" },
                    ["MaSoNguoiDung"] = new[] { "Mã số người dùng(*)", "Ma so nguoi dung(*)", "Mã số người dùng", "Ma so nguoi dung", "MaSoNguoiDung(*)", "MaSoNguoiDung" }
                };
                var headerIndexMap = new Dictionary<string, int>();

                foreach (var patternGroup in headerPatterns)
                {
                    var headerIndex = -1;

                    for (int i = 0; i < headerRow.Count; i++)
                    {
                        var cellHeader = headerRow[i]?.Trim();
                        if (string.IsNullOrWhiteSpace(cellHeader)) continue;

                        if (patternGroup.Value.Any(pattern =>
                            string.Equals(cellHeader, pattern, StringComparison.OrdinalIgnoreCase) ||
                            cellHeader.Contains("tên") && patternGroup.Key == "HoTen" ||
                            cellHeader.Contains("điện thoại") && patternGroup.Key == "SoDienThoai" ||
                            cellHeader.Contains("dien thoai") && patternGroup.Key == "SoDienThoai" ||
                            cellHeader.Contains("Email") && cellHeader.Contains("Huce") && patternGroup.Key == "EmailHuce" ||
                            cellHeader.Contains("Mã số") && patternGroup.Key == "MaSoNguoiDung" ||
                            cellHeader.Contains("Ma so") && patternGroup.Key == "MaSoNguoiDung"))
                        {
                            headerIndex = i;
                            break;
                        }
                    }

                    if (headerIndex == -1)
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportHeaderErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportHeaderErrorInvalid), dto.IndexRowHeader));
                    }

                    headerIndexMap[patternGroup.Key] = headerIndex;
                }

                var allHeaders = headerRow.Where(h => !string.IsNullOrWhiteSpace(h?.Trim())).Select(h => h.Trim()).ToList();

                var vietnamNow = GetVietnamTime();

                var existingDanhBaChiTiets = await _smDbContext.DanhBaChiTiets
                    .Where(x => x.IdDanhBa == dto.IdDanhBa && !x.Deleted)
                    .Select(x => new { x.Id, x.MaSoNguoiDung })
                    .AsNoTracking()
                    .ToListAsync();

                var existingMaSoDict = new Dictionary<string, int>(existingDanhBaChiTiets.Count, StringComparer.OrdinalIgnoreCase);
                foreach (var item in existingDanhBaChiTiets)
                {
                    existingMaSoDict[item.MaSoNguoiDung] = item.Id;
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
                var newDanhBaChiTiets = new List<domain.DanhBa.DanhBaChiTiet>(totalRows);
                var updateDanhBaChiTietIds = new HashSet<int>();
                var newTruongDatas = new List<domain.DanhBa.DanhBaTruongData>();

                var pendingDanhBaDataMappings = new List<(string MaSoNguoiDung, string HeaderName, string CellValue)>(totalRows * allHeaders.Count);

                var newTruongHeaders = allHeaders.Where(h => !existingTruongDict.ContainsKey(h)).ToList();

                foreach (var header in newTruongHeaders)
                {
                    newTruongDatas.Add(new domain.DanhBa.DanhBaTruongData
                    {
                        IdDanhBa = dto.IdDanhBa,
                        TenTruong = header,
                        Type = "string",
                        CreatedDate = vietnamNow,
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

                var updateDanhBaChiTiets = new Dictionary<int, domain.DanhBa.DanhBaChiTiet>();

                for (int rowIndex = startImportRowIndex; rowIndex < excelData.Count; rowIndex++)
                {
                    var row = excelData[rowIndex];
                    var actualRowNumber = rowIndex + 1;

                    if (row.All(cell => string.IsNullOrWhiteSpace(cell)))
                        continue;

                    var hoVaTen = row.Count > headerIndexMap["HoTen"] ? row[headerIndexMap["HoTen"]]?.Trim() : "";
                    var soDienThoai = row.Count > headerIndexMap["SoDienThoai"] ? row[headerIndexMap["SoDienThoai"]]?.Trim() : "";
                    var emailHuce = row.Count > headerIndexMap["EmailHuce"] ? row[headerIndexMap["EmailHuce"]]?.Trim() : "";
                    var maSoNguoiDung = row.Count > headerIndexMap["MaSoNguoiDung"] ? row[headerIndexMap["MaSoNguoiDung"]]?.Trim() : "";

                    if (string.IsNullOrWhiteSpace(hoVaTen) || string.IsNullOrWhiteSpace(soDienThoai) ||
                        string.IsNullOrWhiteSpace(emailHuce) || string.IsNullOrWhiteSpace(maSoNguoiDung))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportRequiredFieldErrorEmpty,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportRequiredFieldErrorEmpty), actualRowNumber));
                    }

                    if (!phoneRegex.IsMatch(soDienThoai))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportPhoneNumberErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportPhoneNumberErrorInvalid), actualRowNumber));
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(emailHuce, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new UserFriendlyException(ErrorCodes.ImportEmailHuceErrorInvalid,
                            string.Format(ErrorMessages.GetMessage(ErrorCodes.ImportEmailHuceErrorInvalid), actualRowNumber));
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
                    if (existingMaSoDict.TryGetValue(maSoNguoiDung, out int existingId))
                    {
                        updateDanhBaChiTietIds.Add(existingId);

                        updateDanhBaChiTiets[existingId] = new domain.DanhBa.DanhBaChiTiet
                        {
                            Id = existingId,
                            IdDanhBa = dto.IdDanhBa,
                            HoVaTen = hoVaTen,
                            SoDienThoai = soDienThoai,
                            EmailHuce = emailHuce,
                            MaSoNguoiDung = maSoNguoiDung
                        };
                    }
                    else
                    {
                        var newRecord = new domain.DanhBa.DanhBaChiTiet
                        {
                            IdDanhBa = dto.IdDanhBa,
                            HoVaTen = hoVaTen,
                            MaSoNguoiDung = maSoNguoiDung,
                            SoDienThoai = soDienThoai,
                            EmailHuce = emailHuce,
                            CreatedDate = vietnamNow,
                            Deleted = false
                        };
                        newDanhBaChiTiets.Add(newRecord);
                        existingMaSoDict[maSoNguoiDung] = -newDanhBaChiTiets.Count;
                    }
                    for (int colIndex = 0; colIndex < Math.Min(row.Count, headerRow.Count); colIndex++)
                    {
                        var headerName = headerRow[colIndex]?.Trim();
                        if (string.IsNullOrWhiteSpace(headerName)) continue;

                        var cellValue = row[colIndex]?.Trim() ?? "";
                        pendingDanhBaDataMappings.Add((maSoNguoiDung, headerName, cellValue));
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

                        var newMaSoList = newDanhBaChiTiets.Select(x => x.MaSoNguoiDung).ToList();
                        var insertedRecords = await _smDbContext.DanhBaChiTiets
                            .Where(x => x.IdDanhBa == dto.IdDanhBa && newMaSoList.Contains(x.MaSoNguoiDung) && !x.Deleted)
                            .Select(x => new { x.Id, x.MaSoNguoiDung })
                            .AsNoTracking()
                            .ToListAsync();
                        foreach (var record in insertedRecords)
                        {
                            existingMaSoDict[record.MaSoNguoiDung] = record.Id;
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
                                    DeletedDate = vietnamNow
                                });
                        }
                    }

               
                    var finalDanhBaDatas = new List<domain.DanhBa.DanhBaData>(pendingDanhBaDataMappings.Count);

                    foreach (var mapping in pendingDanhBaDataMappings)
                    {
                        if (existingMaSoDict.TryGetValue(mapping.MaSoNguoiDung, out int danhBaChiTietId) &&
                            existingTruongDict.TryGetValue(mapping.HeaderName, out int truongDataId))
                        {
                            finalDanhBaDatas.Add(new domain.DanhBa.DanhBaData
                            {
                                Data = mapping.CellValue,
                                IdTruongData = truongDataId,
                                IdDanhBaChiTiet = danhBaChiTietId,        
                                IdDanhBaChienDich = dto.IdDanhBa,         
                                CreatedDate = vietnamNow,
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