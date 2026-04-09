using AutoMapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.Config.Implements
{
    public class NhaMangService : BaseService, INhaMangService
    {
        private readonly UserManager<AppUser> _userManager;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public NhaMangService(
            SmDbContext smDbContext,
            ILogger<NhaMangService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
        }


        public void AddNhaMang(AddNhaMangDto dto)
        {
            _logger.LogInformation($"{nameof(AddNhaMang)} dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var existingNhaMang = _smDbContext.NhaMangs
                                  .FirstOrDefault(x => x.TenNhaMang == dto.TenNhaMang && !x.Deleted);
            if (existingNhaMang != null)
                throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaMangExists);
            var brandNames = _smDbContext.BrandName
                .Where(x => !x.Deleted && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu)
                .ToList();
            var nhaMang = new domain.Config.NhaMang
            {
                TenNhaMang = dto.TenNhaMang,
                Prefix = dto.Prefix,
                CreatedDate = vietNamNow,
                CreatedBy = currentUserId
            };
            _smDbContext.NhaMangs.Add(nhaMang);
            _smDbContext.SaveChanges();

            var nhaMangId = nhaMang.Id;
            //_logger.LogInformation($"nhaMangId = {nhaMangId}");

            var nhaMangExists = _smDbContext.CauHinhDonGias.Any(x => x.IdNhaMang == nhaMangId  && !x.Deleted);
            if (nhaMangExists)
            {
                nhaMang.Deleted = true;
                nhaMang.DeletedBy = currentUserId;
                nhaMang.DeletedDate = vietNamNow;
                _smDbContext.NhaMangs.Update(nhaMang);
                _smDbContext.SaveChanges();
                throw new UserFriendlyException(ErrorCodes.ConfigErrorCauHinhDonGiaExists);
            }
            else
            {
                foreach (var item in brandNames)
                {
                    var cauHinhDonGia = new domain.Config.CauHinhDonGia
                    {
                        IdNhaCungCapDichVu = dto.IdNhaCungCapDichVu,
                        //IdBrandName = dto.IdBrandName,
                        IdBrandName = item.Id,
                        IdNhaMang = nhaMangId,
                        DonGia = dto.DonGia,
                        ThoiHan = dto.ThoiHan,
                        CreatedDate = vietNamNow,
                        CreatedBy = currentUserId
                    };
                    _smDbContext.CauHinhDonGias.Add(cauHinhDonGia);
                    _smDbContext.SaveChanges();
                }
            }
        }

        public void UpdateNhaMang(UpdateNhaMangDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateNhaMang)} dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var nhaMang = _smDbContext.NhaMangs.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaMangNotFound);
            var nhaCungCapDichVu = _smDbContext.NhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);
            /*var brandName = _smDbContext.BrandName.FirstOrDefault(x => x.Id == dto.IdBrandName && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);*/
            var cauHinhDonGia = _smDbContext.CauHinhDonGias.FirstOrDefault(x => x.IdNhaMang == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorCauHinhDonGiaNotFound);
            var brandNames = _smDbContext.BrandName
                .Where(x => !x.Deleted && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu)
                .ToList();

            nhaMang.TenNhaMang = dto.TenNhaMang;
            nhaMang.Prefix = dto.Prefix;
            nhaMang.ModifiedDate = vietNamNow;
            nhaMang.ModifiedBy = currentUserId;
            _smDbContext.NhaMangs.Update(nhaMang);
            _smDbContext.SaveChanges();
            var cauHinhList = _smDbContext.CauHinhDonGias
                    .Where(x => x.IdNhaMang == dto.Id
                           
                           && !x.Deleted)
                    .ToList();
            foreach (var item in brandNames)
            {
                var cauHinhItem = cauHinhList
                    .FirstOrDefault(x => x.IdBrandName == item.Id);
                //cauHinhDonGia.IdBrandName = dto.IdBrandName;
                if (cauHinhItem != null)
                {
                    cauHinhDonGia.DonGia = dto.DonGia;
                    //cauHinhDonGia.IdBrandName = item.Id;
                    cauHinhDonGia.ThoiHan = dto.ThoiHan;
                    cauHinhDonGia.ModifiedDate = vietNamNow;
                    cauHinhDonGia.ModifiedBy = currentUserId;
                    _smDbContext.CauHinhDonGias.Update(cauHinhDonGia);
                    
                }
                
            }
            _smDbContext.SaveChanges();
        }

        public void DeleteNhaMang(int id)
        {
            _logger.LogInformation($"{nameof(DeleteNhaMang)} id = {id}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var nhaMang = _smDbContext.NhaMangs.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaMangNotFound);
            var cauHinhDonGia = _smDbContext.CauHinhDonGias
                .Where(x => x.IdNhaMang == id && !x.Deleted)
                .ToList();
            nhaMang.Deleted = true;
            nhaMang.DeletedBy = currentUserId;
            nhaMang.DeletedDate = vietNamNow;
            _smDbContext.NhaMangs.Update(nhaMang);
            _smDbContext.SaveChanges();
            foreach (var cauHinh in cauHinhDonGia)
            {
                cauHinh.Deleted = true;
                cauHinh.DeletedBy = currentUserId;
                cauHinh.DeletedDate = vietNamNow;
                _smDbContext.CauHinhDonGias.Update(cauHinh);
                _smDbContext.SaveChanges();
            }
            
            //_smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewNhaMangDto> FindPaging (FindPagingNhaMangDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)} dto = {JsonSerializer.Serialize(dto)}");
            
            var query = from nm in _smDbContext.NhaMangs
                        where !nm.Deleted
                        let chdg = _smDbContext.CauHinhDonGias
                            .Where(x => x.IdNhaMang == nm.Id && !x.Deleted)
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefault()                    
                        where chdg != null
                        join ncc in _smDbContext.NhaCungCapDichVus
                        on chdg.IdNhaCungCapDichVu equals ncc.Id
                        where !ncc.Deleted
                        orderby nm.Id
                        select new ViewNhaMangDto
                        {
                            Id = nm.Id,
                            TenNhaMang = nm.TenNhaMang,
                            Prefix = nm.Prefix,
                            DonGia = new DonGiaDto
                            {
                                Id = chdg.Id,
                                //IdBrandName = chdg.IdBrandName,
                                //IdNhaMang = chdg.IdNhaMang,
                                DonGia = chdg.DonGia,
                                ThoiHan = chdg.ThoiHan,
                            },
                            NhaCungCapDichVu = ncc != null ? new NhaCungCapDichVu
                            {
                                IdNhaCungCapDichVu = ncc.Id,
                                TenNhaCungCapDichVu = ncc.Name,
                                BrandNames = _smDbContext.BrandName
                                .Where(x => x.IdNhaCungCapDichVu == ncc.Id && !x.Deleted)
                                .Select(x => new BrandNameDto
                                {
                                    Id = x.Id,
                                    TenBrandName = x.TenBrandName,
                                }).ToList()
                            } : null
                        };
            var data = query.Paging(dto).ToList();
            return new BaseResponsePagingDto<ViewNhaMangDto>
            {
                Items = data,
                TotalItems = query.Count(),
            };
        }

        public ViewNhaMangDto GetById(int id)
        {
            _logger.LogInformation($"{nameof(GetById)} id = {id}");
            var query = from nm in _smDbContext.NhaMangs
                        join chdg in _smDbContext.CauHinhDonGias
                            
                        on nm.Id equals chdg.IdNhaMang
                        join ncc in _smDbContext.NhaCungCapDichVus
                        on chdg.IdNhaCungCapDichVu equals ncc.Id
                        where !nm.Deleted
                        && !chdg.Deleted
                        && !ncc.Deleted
                        && nm.Id == id
                        orderby nm.Id
                        
                        select new ViewNhaMangDto
                        {
                            Id = nm.Id,
                            TenNhaMang = nm.TenNhaMang,
                            Prefix = nm.Prefix,
                            DonGia = new DonGiaDto
                            {
                                Id = chdg.Id,
                                //IdBrandName = chdg.IdBrandName,
                                //IdNhaMang = chdg.IdNhaMang,
                                DonGia = chdg.DonGia,
                                ThoiHan = chdg.ThoiHan,
                            },
                            NhaCungCapDichVu = new NhaCungCapDichVu
                            {
                                IdNhaCungCapDichVu = ncc.Id,
                                TenNhaCungCapDichVu = ncc.Name,
                                BrandNames = _smDbContext.BrandName
                                .Where(x => x.IdNhaCungCapDichVu == ncc.Id && !x.Deleted)
                                .Select(x => new BrandNameDto
                                {
                                    Id = x.Id,
                                    TenBrandName = x.TenBrandName,
                                }).ToList()
                            }
                        };
            var result = query.FirstOrDefault()
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaMangNotFound);
            return result;
        }
        }
}
