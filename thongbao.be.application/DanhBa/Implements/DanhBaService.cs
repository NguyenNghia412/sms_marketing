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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.DanhBa.Interfaces;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.infrastructure.data;
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
            worksheet.Range(1, 1, 1, 12).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Row(1).Height = 25;

            
            var headers = new string[]
            {
                "STT", "Họ tên", "Họ đệm", "Tên", "Số điện thoại", "Email Huce",
                "Ngày sinh", "Giới tính", "Địa chỉ", "Là người dùng", "Mã số người dùng", "Trạng thái hoạt động"
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
                    Title = $"IMPORT DANH BẠ NGƯỜI DÙNG - {GetVietnamTime():yyyy-MM-dd HH:mm:ss}"
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
                UpdateCells = new   UpdateCellsRequest
                {
                    Range = new GridRange { SheetId = 0, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = 12 },
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
                    Range = new GridRange { SheetId = 0, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = 12 }
                }
            });

          
            var headers = new[] { "STT", "Họ tên", "Họ đệm", "Tên", "Số điện thoại", "Email Huce",
                                "Ngày sinh", "Giới tính", "Địa chỉ", "Là người dùng", "Mã số người dùng", "Trạng thái hoạt động" };

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
                    Range = new GridRange { SheetId = 0, StartRowIndex = 3, EndRowIndex = 4, StartColumnIndex = 0, EndColumnIndex = 12 },
                    Rows = new List<RowData> { new RowData { Values = headerValues } },
                    Fields = "userEnteredValue,userEnteredFormat"
                }
            });

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

        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
