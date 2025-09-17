using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.DiemDanh.Implements;
using thongbao.be.application.ToChuc.Dtos;
using thongbao.be.application.ToChuc.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.Constants.ToChuc;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.ToChuc.Implements
{
    public  class ToChucService: BaseService, IToChucService
    {
        private readonly IConfiguration _configuration;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public ToChucService (
            IConfiguration configuration,
            SmDbContext smDbContext,
            ILogger<ToChucService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }


        public void Create (CreateToChucDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}  dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            if (dto.LoaiToChuc != ToChucConstants.NhanVien && dto.LoaiToChuc != ToChucConstants.SinhVien) {
                throw new UserFriendlyException(ErrorCodes.ToChucErrorLoaiToChucNotFound,ErrorMessages.GetMessage(ErrorCodes.ToChucErrorLoaiToChucNotFound));
            }
                var toChuc = new domain.ToChuc.ToChuc
            {
                TenToChuc = dto.TenToChuc,
                MoTa = dto.MoTa,
                //LoaiToChuc = dto.LoaiToChuc,
                MaSoToChuc = dto.MaSoToChuc,
            };
            _smDbContext.ToChucs.Add( toChuc );
            _smDbContext.SaveChanges();
        }
        public void Update(int idToChuc,UpdateToChucDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}  dto = {JsonSerializer.Serialize(dto)}");
            var vietnamNow = GetVietnamTime();
            var existingToChuc = _smDbContext.ToChucs.FirstOrDefault(x => x.Id == idToChuc && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ToChucErrorNotFound));
            if (dto.LoaiToChuc != ToChucConstants.NhanVien && dto.LoaiToChuc != ToChucConstants.SinhVien)
            {
                throw new UserFriendlyException(ErrorCodes.ToChucErrorLoaiToChucNotFound, ErrorMessages.GetMessage(ErrorCodes.ToChucErrorLoaiToChucNotFound));
            }
            existingToChuc.TenToChuc = dto.TenToChuc;
            existingToChuc.MoTa = dto.MoTa;
            //existingToChuc.LoaiToChuc = dto.LoaiToChuc;
            existingToChuc.MaSoToChuc = dto.MaSoToChuc;
            _smDbContext.ToChucs.Update(existingToChuc);
            _smDbContext.SaveChanges();
        }
        public void Delete (int idToChuc)
        {
            _logger.LogInformation($"{nameof(Delete)}");
            var vietnamNow = GetVietnamTime();
            var existingToChuc = _smDbContext.ToChucs.FirstOrDefault(x => x.Id == idToChuc && !x.Deleted)
                  ?? throw new UserFriendlyException(ErrorCodes.ToChucErrorNotFound, ErrorMessages.GetMessage(ErrorCodes.ToChucErrorNotFound));
            existingToChuc.Deleted = true;
            existingToChuc.DeletedDate = vietnamNow;
            _smDbContext.ToChucs.Update(existingToChuc);
            _smDbContext.SaveChanges();
        }
        public BaseResponsePagingDto<ViewToChucDto> Find(FindPagingToChucDto dto)
        {
            _logger.LogInformation($"{nameof(Find)}  dto={JsonSerializer.Serialize(dto)}");
            var query = from tc in _smDbContext.ToChucs
                        where !tc.Deleted
                        orderby tc.CreatedDate descending
                        select tc;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewToChucDto>>(data);
            return new BaseResponsePagingDto<ViewToChucDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }


        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
