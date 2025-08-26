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
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class ChienDichService : BaseService, IChienDichService
    {
        public ChienDichService(
            SmDbContext smDbContext, 
            ILogger<ChienDichService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) 
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
        }

        public void Create(CreateChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");

            var tinNhan = new domain.GuiTinNhan.ChienDich
            {
                TenChienDich = dto.TenChienDich,
                NgayBatDau = dto.NgayBatDau ?? DateTime.Now,
                NgayKetThuc = dto.NgayKetThuc,
                MoTa = dto.MoTa,
                IsFlashSms = dto.IsFlashSms,
            };

            _smDbContext.ChienDiches.Add(tinNhan);
            _smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewChienDichDto> Find(FindPagingChienDichDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={JsonSerializer.Serialize(dto)}");

            var query = from cd in _smDbContext.ChienDiches
                        where !cd.Deleted
                        orderby cd.CreatedDate descending
                        select cd;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewChienDichDto>>(data);

            return new BaseResponsePagingDto<ViewChienDichDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }
    }
}
