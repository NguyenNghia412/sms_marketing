using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.TraoBang.Dtos;
using thongbao.be.application.TraoBang.Interface;
using thongbao.be.infrastructure.data;
using thongbao.be.infrastructure.data.Migrations;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.TraoBang.Implements
{
    public class SubPlanService:BaseService, ISubPlanService
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly IConfiguration _configuration;
        public SubPlanService(
            SmDbContext smDbContext,
            ILogger<SubPlanService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }
        public void Create (int idPlan, CreateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto = {JsonSerializer.Serialize(dto)}");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = new domain.TraoBang.SubPlan
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                TruongKhoa = dto.TruongKhoa,
                MoBai = dto.MoBai ?? "",
                KetBai = dto.KetBai ?? "",
                Note = dto.Note,
                IsShow = true,
                Order = dto.Order,
                IdPlan = idPlan,
                CreatedDate = GetVietnamTime(),
                Deleted = false
            };
            _smDbContext.SubPlans.Add(subplan);
            _smDbContext.SaveChanges();
        }
        public void Update (UpdateSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto = {JsonSerializer.Serialize(dto)}");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            subplan.Ten = dto.Ten;
            subplan.MoTa = dto.MoTa;
            subplan.TruongKhoa = dto.TruongKhoa;
            subplan.MoBai = dto.MoBai ?? "";
            subplan.KetBai = dto.KetBai ?? "";
            subplan.Note = dto.Note;
            //subplan.Order = dto.Order;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewSubPlanDto> FindPaging (FindPagingSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)}, dto = {JsonSerializer.Serialize(dto)}");
            var query = from sp in _smDbContext.SubPlans
                        where !sp.Deleted
                        orderby sp.Order ascending
                        select sp;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewSubPlanDto>>(data);
            return new BaseResponsePagingDto<ViewSubPlanDto>
            {
                TotalItems = query.Count(),
                Items = items
            };
        }
        public void UpdateIsShow (UpdateSubPlanIsShowDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateIsShow)}, dto= {JsonSerializer.Serialize(dto)} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            subplan.IsShow = dto.IsShow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public void Delete (int idPlan, int idSubPlan)
        {
            _logger.LogInformation($"{nameof(Delete)}, idPlan= {idPlan}, idSubPlan= {idSubPlan} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == idSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var vietnamNow = GetVietnamTime();
            subplan.Deleted = true;
            subplan.DeletedDate = vietnamNow;
            _smDbContext.SubPlans.Update(subplan);
            _smDbContext.SaveChanges();
        }
        public async Task<List<UpdateOrderSubPlanResponseDto>> MoveOrder (MoveOrderSubPlanDto dto)
        {
            _logger.LogInformation($"{nameof(MoveOrder)}, dto= {JsonSerializer.Serialize(dto)} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == dto.IdPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subplan = _smDbContext.SubPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            var subPlans = _smDbContext.SubPlans
                .Where(x => x.IdPlan == dto.IdPlan && !x.Deleted)
                .OrderBy(x => x.Order)
                .ToList();
            var movingSubPlan = subPlans.FirstOrDefault(x => x.Id == dto.IdSubPlan);
            if(movingSubPlan == null)
            {
                throw new UserFriendlyException(ErrorCodes.TraoBangErrorSubPlanNotFound);
            }
            var currentOrder = movingSubPlan.Order;
            var newOrder = dto.NewOrder;
            if(currentOrder == newOrder)
            {
                return subPlans.Select(x => new UpdateOrderSubPlanResponseDto
                {
                    Id = x.Id,
                    Ten = x.Ten,
                    Order = x.Order
                })
                .OrderBy(x => x.Order)
                .ToList();
            }

            if (newOrder < currentOrder)
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order >= newOrder && sp.Order < currentOrder)
                    {
                        sp.Order++;
                    }

                }
            }
            else
            {
                foreach (var sp in subPlans)
                {
                    if (sp.Order <= newOrder && sp.Order > currentOrder)
                    {
                        sp.Order--;
                    }
                }
            }
            movingSubPlan.Order = newOrder;
            _smDbContext.SubPlans.UpdateRange(subPlans);
            await _smDbContext.SaveChangesAsync();
            return subPlans.Select(x => new UpdateOrderSubPlanResponseDto
            {
                Id = x.Id,
                Ten = x.Ten,
                Order = x.Order
            })
            .OrderBy(x => x.Order)
            .ToList();

        }
        public async Task<List<GetListSubPlanResponseDto>> GetListSubPlan(int idPlan)
        {
            _logger.LogInformation($"{nameof(GetListSubPlan)}, idPlan= {idPlan} ");
            var plan = _smDbContext.Plans.FirstOrDefault(x => x.Id == idPlan && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.TraoBangErrorPlanNotFound);
            var subPlans = await _smDbContext.SubPlans
                                .AsNoTracking()
                                .Where(x => x.IdPlan == idPlan && !x.Deleted)
                                .OrderBy(x => x.Order)
                                .Select(x => new GetListSubPlanResponseDto
                                        {
                                             Id = x.Id,
                                             Ten = x.Ten
                                        }) .ToListAsync();

            return subPlans;
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
        private string _extractSpreadsheetId(string sheetUrl)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sheetUrl, @"/spreadsheets/d/([a-zA-Z0-9-_]+)");
            if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                throw new UserFriendlyException(ErrorCodes.GoogleSheetUrlErrorInvalid, ErrorMessages.GetMessage(ErrorCodes.GoogleSheetUrlErrorInvalid));
            }
            return match.Groups[1].Value;
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
