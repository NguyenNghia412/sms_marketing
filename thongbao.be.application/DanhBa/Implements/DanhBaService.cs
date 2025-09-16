using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using thongbao.be.application.Base;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.DanhBa.Interfaces;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.Constants.DanhBa;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.DanhBa.Implements
{
    public  class DanhBaService :BaseService, IDanhBaService
    {
        private readonly IConfiguration _configuration;
        private readonly string[] Scopes = {
               "https://www.googleapis.com/auth/drive",
               "https://www.googleapis.com/auth/drive.file",
               "https://www.googleapis.com/auth/spreadsheets"
        };
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

        public void Update(int idDanhBa, UpdateDanhBaDto dto) { 
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
        public BaseResponsePagingDto<ViewDanhBaDto>Find (FindPagingDanhBaDto dto)
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
        public async Task<byte[]> ExportDanhBaChiTietExcelTemplate()
        {
            _logger.LogInformation($"{nameof(ExportDanhBaChiTietExcelTemplate)}");

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

        public async Task<string> CreateDanhBaGoogleSheetTemplate()
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
        }
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
                    if (!email.EndsWith("@st.huce.edu.vn", StringComparison.OrdinalIgnoreCase))
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

                            // THÊM DELAY nhỏ giữa các batch để giảm áp lực lên DB
                            if (batchIndex < batches.Count - 1)
                            {
                                await Task.Delay(50); // 50ms delay
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
        private async Task<List<List<string>>> _getSheetData(string sheetUrl, string sheetName)
        {
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];

            if (string.IsNullOrEmpty(serviceAccountPath))
            {
                throw new UserFriendlyException(ErrorCodes.ServiceAccountErrorNotFound,ErrorMessages.GetMessage(ErrorCodes.ServiceAccountErrorNotFound));
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

        private string _extractSpreadsheetId(string sheetUrl)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sheetUrl, @"/spreadsheets/d/([a-zA-Z0-9-_]+)");
            if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
            {
               throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid,ErrorMessages.GetMessage(ErrorCodes.GoogleSheetUrlErrorInvalid)) ;
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
