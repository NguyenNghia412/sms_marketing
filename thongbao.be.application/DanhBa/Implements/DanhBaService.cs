using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
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
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public DanhBaService(
            SmDbContext smDbContext,
            ILogger<DanhBaService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }


        public void Create(CreateDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var danhBa = new domain.DanhBa.DanhBaChienDich
            {
                TenDanhBa = dto.TenDanhBa,
                Mota = dto.Mota,
                GhiChu = dto.GhiChu,
                CreatedDate = vietnamNow,
            };
            _smDbContext.DanhBaChienDichs.Add(danhBa);
            _smDbContext.SaveChanges();
        }

        public void Update(int idDanhBa, UpdateDanhBaDto dto) { 
            _logger.LogInformation($"{nameof(Update)} idDanhBa={idDanhBa}, dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var existingDanhBa = _smDbContext.DanhBaChienDichs.FirstOrDefault(x => x.Id == idDanhBa && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.DanhBaErrorNotFound));
            existingDanhBa.TenDanhBa = dto.TenDanhBa;
            existingDanhBa.Mota = dto.Mota;
            existingDanhBa.GhiChu = dto.GhiChu;
            _smDbContext.DanhBaChienDichs.Update(existingDanhBa);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewDanhBaDto>Find (FindPagingDanhBaDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={System.Text.Json.JsonSerializer.Serialize(dto)}");
            var query = from db in _smDbContext.DanhBaChienDichs
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

        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
