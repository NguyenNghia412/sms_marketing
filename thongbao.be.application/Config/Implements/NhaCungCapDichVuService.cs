using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
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
using thongbao.be.application.Config.Dtos.NhaCungCapDichVu;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.Config.Implements
{
    public class NhaCungCapDichVuService : BaseService, INhaCungCapDichVuService
    {
        private readonly UserManager<AppUser> _userManager;
        public NhaCungCapDichVuService(
            SmDbContext smDbContext,
            ILogger<NhaCungCapDichVuService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
        }

        public void Create(CreateNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(Create)}, dto ={JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();


            var nhaCungCapExisted = _smDbContext.NhaCungCapDichVus
                .FirstOrDefault(x => x.Name == dto.Name && !x.Deleted);
            if (nhaCungCapExisted != null)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuExists);
            }


            var nhaCungCap = new domain.Config.NhaCungCapDichVu
            {
                Name = dto.Name,
                ApiKey = dto.ApiKey,
                ApiSecret = dto.ApiSecret,
                BaseUrl = dto.BaseUrl,
                IsConfigAuthReq = dto.IsConfigAuthReq,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow
            };

            _smDbContext.NhaCungCapDichVus.Add(nhaCungCap);
            _smDbContext.SaveChanges();

            var idNhaCungCapDichVu = nhaCungCap.Id;
            foreach (var brandName in dto.BrandNames)
            {
                var data = new domain.GuiTinNhan.BrandName
                {
                    TenBrandName = brandName.TenBrandName,
                    IdNhaCungCapDichVu = idNhaCungCapDichVu,
                    ThoiGianBatDauHoatDong = brandName.ThoiGianBatDauHoatDong,
                    ThoiGianKetThucHoatDong = brandName.ThoiGianKetThucHoatDong,
                    CreatedBy = currentUserId,
                    CreatedDate = vietNamNow
                };

                _smDbContext.BrandName.Add(data);

            }
            _smDbContext.SaveChanges();
        }


        public void Update(UpdateNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(Update)}, dto ={JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var nhaCungCap = _smDbContext.NhaCungCapDichVus
                .FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);


            var nhaCungCapExisted = _smDbContext.NhaCungCapDichVus
                .FirstOrDefault(x => x.Name == dto.Name && x.Id != dto.Id && !x.Deleted);

            if (nhaCungCapExisted != null)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuExists);
            }

            var brandNames = _smDbContext.BrandName
                .Where(b => b.IdNhaCungCapDichVu == dto.Id && !b.Deleted)
                .ToList();


            nhaCungCap.Name = dto.Name;
            nhaCungCap.ApiKey = dto.ApiKey;
            nhaCungCap.ApiSecret = dto.ApiSecret;
            nhaCungCap.BaseUrl = dto.BaseUrl;
            nhaCungCap.IsConfigAuthReq = dto.IsConfigAuthReq;
            nhaCungCap.ModifiedBy = currentUserId;
            nhaCungCap.ModifiedDate = vietNamNow;


            _smDbContext.NhaCungCapDichVus.Update(nhaCungCap);
            //_smDbContext.SaveChanges();

            foreach (var brandName in brandNames)
            {
                var brandNameDto = dto.BrandNames.FirstOrDefault(x => x.IdBrandName == brandName.Id);
                if (brandNameDto != null)
                {
                    brandName.TenBrandName = brandNameDto.TenBrandName;
                    brandName.ThoiGianBatDauHoatDong = brandNameDto.ThoiGianBatDauHoatDong;
                    brandName.ThoiGianKetThucHoatDong = brandNameDto.ThoiGianKetThucHoatDong;

                    _smDbContext.BrandName.Update(brandName);
                }

            }

            _smDbContext.SaveChanges();


        }

        public BaseResponsePagingDto<ViewNhaCungCapDichVuDto> FindPaging(FindPagingNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(FindPaging)}, dto ={JsonSerializer.Serialize(dto)}");
            var query = from ncc in _smDbContext.NhaCungCapDichVus
                        where !ncc.Deleted
                        orderby ncc.Id
                        select new ViewNhaCungCapDichVuDto
                        {
                            Id = ncc.Id,
                            Name = ncc.Name,
                            ApiKey = ncc.ApiKey,
                            ApiSecret = ncc.ApiSecret,
                            BaseUrl = ncc.BaseUrl,
                            IsConfigAuthReq = ncc.IsConfigAuthReq,
                            CreatedBy = ncc.CreatedBy,
                            CreatedDate = ncc.CreatedDate,
                            ModifiedBy = ncc.ModifiedBy,
                            ModifiedDate = ncc.ModifiedDate,
                            DeletedBy = ncc.DeletedBy,
                            DeletedDate = ncc.DeletedDate,
                            Deleted = ncc.Deleted,
                            BrandNames = _smDbContext.BrandName
                                .Where(b => b.IdNhaCungCapDichVu == ncc.Id && !b.Deleted)
                                .Select(b => new BrandName
                                {
                                    Id = b.Id,
                                    TenBrandName = b.TenBrandName,


                                })
                                .ToList()
                        };

            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewNhaCungCapDichVuDto>>(data);
            return new BaseResponsePagingDto<ViewNhaCungCapDichVuDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }


        public void Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}, idNhaCungCapDichVu ={id}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var nhaCungCap = _smDbContext.NhaCungCapDichVus
                .FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);

            var brandNames = _smDbContext.BrandName
                .Where(b => b.IdNhaCungCapDichVu == id && !b.Deleted)
                .ToList();
            nhaCungCap.Deleted = true;
            nhaCungCap.DeletedBy = currentUserId;
            nhaCungCap.DeletedDate = vietNamNow;
            _smDbContext.NhaCungCapDichVus.Update(nhaCungCap);
            //_smDbContext.SaveChanges();

            foreach (var brandName in brandNames)
            {
                brandName.Deleted = true;
                brandName.DeletedBy = currentUserId;
                brandName.DeletedDate = vietNamNow;
                _smDbContext.BrandName.Update(brandName);

            }
            _smDbContext.SaveChanges();
        }

        public ViewNhaCungCapByIdDto GetById(int id)
        {
            _logger.LogInformation($"{nameof(GetById)}, idNhaCungCapDichVu ={id}");
            var query = from ncc in _smDbContext.NhaCungCapDichVus
                        where ncc.Id == id && !ncc.Deleted
                        select new ViewNhaCungCapByIdDto
                        {
                            Id = ncc.Id,
                            Name = ncc.Name,
                            ApiKey = ncc.ApiKey,
                            ApiSecret = ncc.ApiSecret,
                            BaseUrl = ncc.BaseUrl,
                            IsConfigAuthReq = ncc.IsConfigAuthReq,
                            BrandNames = _smDbContext.BrandName
                            .Where(b => b.IdNhaCungCapDichVu == ncc.Id && !b.Deleted)
                            .Select(b => new BrandNameDto
                            {
                                Id = b.Id,
                                TenBrandName = b.TenBrandName
                            }).ToList()
                        };
            var data = query.FirstOrDefault();
            var item = _mapper.Map<ViewNhaCungCapByIdDto>(data);
            return item;
        }

        public List<GetDropDownNhaCungCapDichVuDto> GetDropDownNhaCungCapDichVu()
        {
            _logger.LogInformation($"{nameof(GetDropDownNhaCungCapDichVu)}");
            var query = from ncc in _smDbContext.NhaCungCapDichVus
                        where !ncc.Deleted
                        orderby ncc.Id
                        select ncc;
            var data = query.ToList();
            var items = _mapper.Map<List<GetDropDownNhaCungCapDichVuDto>>(data);
            return items;
        }



        public void AddBrandNameToNhaCungCapDichVu(AddBrandNameToNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(AddBrandNameToNhaCungCapDichVu)} dto = {JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();

            var nhaCungCapDichVu = _smDbContext.NhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);

            var brandNameExist = _smDbContext.BrandName.FirstOrDefault(x => x.TenBrandName == dto.TenBrandName && !x.Deleted);
            if (brandNameExist != null)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorBrandNameExitsted);
            }

            var brandName = new domain.GuiTinNhan.BrandName
            {
                TenBrandName = dto.TenBrandName,
                ThoiGianBatDauHoatDong = dto.ThoiGianBatDauHoatDong,
                ThoiGianKetThucHoatDong = dto.ThoiGianKetThucHoatDong,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };

            _smDbContext.BrandName.Add(brandName);
            _smDbContext.SaveChanges();
        }


        public void DeleteBrandNameToNhaCungCapDichVu(DeleteBrandNameToNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(DeleteBrandNameToNhaCungCapDichVu)} dto = {JsonSerializer.Serialize(dto)}");

            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();


            var nhaCungCapDichVu = _smDbContext.NhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);
            var brandName = _smDbContext.BrandName.FirstOrDefault(x => x.Id == dto.IdBrandName && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);

            brandName.Deleted = true;
            brandName.DeletedBy = currentUserId;
            brandName.DeletedDate = vietNamNow;
            _smDbContext.BrandName.Update(brandName);
            _smDbContext.SaveChanges();
        }


        public async Task AddUserToNhaCungCapDichVu(AddUserToNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(AddUserToNhaCungCapDichVu)} dto = {JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();


            var nhaCungCapDichVu = _smDbContext.NhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);
            var user = _userManager.FindByIdAsync(dto.IdUser).Result
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);

            var brandNameIds = dto.IdBrandName.Distinct().ToList();
            var brandNames = await _smDbContext.BrandName
                .Where(x => brandNameIds.Contains(x.Id)
                    && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu
                    && !x.Deleted)
                .ToListAsync();

            if (brandNames.Count != brandNameIds?.Count)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }

            var userNhaCungCapDichVuExisted = await _smDbContext.UserNhaCungCapDichVus
                .Where(x => x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu
                    && brandNameIds.Contains(x.Id)
                    && !x.Deleted)
                .Select(x => x.IdBrandName)
                .ToListAsync();
            if (userNhaCungCapDichVuExisted.Any())
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserNhaCungCapDichVuExisted);
            }

            if (dto.ThoiGianBatDauSuDungDichVu != null && dto.ThoiGianKetThucSuDungDichVu != null)
            {
                if (dto.ThoiGianBatDauSuDungDichVu >= dto.ThoiGianKetThucSuDungDichVu || dto.ThoiGianBatDauSuDungDichVu <= vietNamNow || dto.ThoiGianKetThucSuDungDichVu <= vietNamNow)
                {
                    throw new UserFriendlyException(ErrorCodes.ConfigErrorThoiGianKhongHopLe);
                }
            }
            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            try {


                var userNhaCungCapDichVu = brandNameIds.Select(brId =>new domain.Config.UserNhaCungCapDichVu
                {
                    UserId = dto.IdUser,
                    IdNhaCungCapDichVu = dto.IdNhaCungCapDichVu,
                    IdBrandName = brId,
                    ThoiGianBatDauSuDungDichVu = dto.ThoiGianBatDauSuDungDichVu,
                    ThoiGianKetThucSuDungDichVu = dto.ThoiGianKetThucSuDungDichVu,
                    CreatedBy = currentUserId,
                    CreatedDate = vietNamNow
                }).ToList();

                await _smDbContext.UserNhaCungCapDichVus.AddRangeAsync(userNhaCungCapDichVu);
                await _smDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            

            
        }

        public void UpdateUserToNhaCungCapDichVu(UpdateUserToNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateUserToNhaCungCapDichVu)} dto = {JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var userNhaCungCapDichVu = _smDbContext.UserNhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdUserNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorUserNhaCungCapDichVuNotFound);

            if (dto.ThoiGianBatDauSuDungDichVu != null && dto.ThoiGianKetThucSuDungDichVu != null)
            {
                if (dto.ThoiGianBatDauSuDungDichVu >= dto.ThoiGianKetThucSuDungDichVu || dto.ThoiGianBatDauSuDungDichVu <= vietNamNow || dto.ThoiGianKetThucSuDungDichVu <= vietNamNow)
                {
                    throw new UserFriendlyException(ErrorCodes.ConfigErrorThoiGianKhongHopLe);
                }
            }

            userNhaCungCapDichVu.ThoiGianBatDauSuDungDichVu = dto.ThoiGianBatDauSuDungDichVu;
            userNhaCungCapDichVu.ThoiGianKetThucSuDungDichVu = dto.ThoiGianKetThucSuDungDichVu;
            userNhaCungCapDichVu.ModifiedBy = currentUserId;
            userNhaCungCapDichVu.ModifiedDate = vietNamNow;
            _smDbContext.UserNhaCungCapDichVus.Update(userNhaCungCapDichVu);
            _smDbContext.SaveChanges();
        }


        public void DeleteUserToNhaCungCapDichVu(int idUserNhaCungCapDichVu)
        {
            _logger.LogInformation($"{nameof(DeleteUserToNhaCungCapDichVu)} idNhaCungCapDichVu = ${idUserNhaCungCapDichVu}");
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var userNhaCungCapDichVu = _smDbContext.UserNhaCungCapDichVus.FirstOrDefault(x => x.Id == idUserNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorUserNhaCungCapDichVuNotFound);
            userNhaCungCapDichVu.Deleted = true;
            userNhaCungCapDichVu.DeletedBy = currentUserId;
            userNhaCungCapDichVu.DeletedDate = vietNamNow;
            _smDbContext.UserNhaCungCapDichVus.Update(userNhaCungCapDichVu);
            _smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewUserNhaCungCapDto> FindPagingUserNhaCungCapDichVu(FindPagingUserToNhaCungCapDichVuDto dto)
        {
            _logger.LogInformation($"{nameof(FindPagingUserNhaCungCapDichVu)}, dto ={JsonSerializer.Serialize(dto)}");
            var query = from unc in _smDbContext.UserNhaCungCapDichVus
                        where !unc.Deleted
                        orderby unc.Id
                        select new ViewUserNhaCungCapDto
                        {
                            Id = unc.Id,
                            User = new ViewUserNhaCungCapWithDetailsDto
                            {
                                IdUser = unc.UserId,
                                FullName = _smDbContext.Users.Where(u => u.Id == unc.UserId).Select(u => u.FullName).FirstOrDefault(),
                                UserName = _smDbContext.Users.Where(u => u.Id == unc.UserId).Select(u => u.UserName).FirstOrDefault()
                            },
                            NhaCungCapDichVu = new NhaCungCapDichVu
                            {
                                Id = unc.IdNhaCungCapDichVu,
                                Name = _smDbContext.NhaCungCapDichVus.Where(ncc => ncc.Id == unc.IdNhaCungCapDichVu).Select(ncc => ncc.Name).FirstOrDefault()
                            },
                            BrandName = new ViewBrandName
                            {
                                Id = unc.IdBrandName,
                                TenBrandName = _smDbContext.BrandName.Where(b => b.Id == unc.IdBrandName && b.IdNhaCungCapDichVu == unc.IdNhaCungCapDichVu).Select(b => b.TenBrandName).FirstOrDefault()
                            }
                        };


            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewUserNhaCungCapDto>>(data);
            return new BaseResponsePagingDto<ViewUserNhaCungCapDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }

        public ViewUserToNhaCungCapDichVuByIdDto FindById (int idUserNhaCungCapDichVu)
        {
            _logger.LogInformation($"{nameof(FindById)}, idUserNhaCungCapDichVu ={idUserNhaCungCapDichVu}");
            var query = from unc in _smDbContext.UserNhaCungCapDichVus
                        where unc.Id == idUserNhaCungCapDichVu && !unc.Deleted
                        join u in _smDbContext.Users on unc.UserId equals u.Id
                        join ncc in _smDbContext.NhaCungCapDichVus on unc.IdNhaCungCapDichVu equals ncc.Id
                        join b in _smDbContext.BrandName on unc.IdBrandName equals b.Id
                        select new ViewUserToNhaCungCapDichVuByIdDto
                        {
                            Id = unc.Id,
                            User = new ViewUserNhaCungCapWithDetailsByIdDto
                            {
                                IdUser = u.Id,
                                FullName = u.FullName,
                                UserName = u.UserName,
                                
                            },
                            NhaCungCapDichVu = new ViewNhaCungCapDichVu
                            {
                                Id = ncc.Id,
                                Name = ncc.Name,
                               
                            },
                            BrandName = new ViewBrandNameNhaCungCapDichVu
                            {
                                Id = b.Id,
                                TenBrandName = b.TenBrandName
                            }
                        };
            var data = query.FirstOrDefault()
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorUserNhaCungCapDichVuNotFound);
            var item = _mapper.Map<ViewUserToNhaCungCapDichVuByIdDto>(data);
            return item;

        }

        public List<GetListBrandNameResponseDto> GetListBrandName(int idNhaCungCapDichVu)
        {
            _logger.LogInformation($"{nameof(GetListBrandName)} idNhaCungCapDichVu = ${idNhaCungCapDichVu}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();

            var query = from bn in _smDbContext.BrandName
                        where !bn.Deleted
                        && bn.IdNhaCungCapDichVu == idNhaCungCapDichVu
                        orderby bn.CreatedDate descending
                        select bn;

            var data = query.ToList();
            var result = _mapper.Map<List<GetListBrandNameResponseDto>>(data);

            return result;
        }
    }
}
