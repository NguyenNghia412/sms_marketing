using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
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
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.application.Config.Dtos.UserCredits;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;
using thongbao.be.lib.CdsConnect.Dtos.Base;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.Constants.Config;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.Config.Implements
{
    public class UserCreditsService : BaseService, IUserCreditsService
    {
        private readonly UserManager<AppUser> _userManager;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly IProfileService _profileService;
        public UserCreditsService(
            SmDbContext smDbContext,
            ILogger<UserCreditsService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IProfileService profileService,
            IMapper mapper
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _userManager = userManager;
            _profileService = profileService;
        }

        public void AddUserCredits(AddUserCreditsDto dto)
        {
            _logger.LogInformation($"{nameof(AddUserCreditsDto)} dto = {JsonSerializer.Serialize(dto)}");
            //dto.HanMucCredit = SoTienMacDinhConstants.SoTienMacDinh;
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var nhaCungCapDichVu = _smDbContext.NhaCungCapDichVus.FirstOrDefault(x => x.Id == dto.IdNhaCungCapDichVu && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ConfigErrorNhaCungCapDichVuNotFound);
            
            var user = _smDbContext.Users.FirstOrDefault(x => x.Id == dto.UserId)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);
            var isUserBelongsToNcc = _smDbContext.UserNhaCungCapDichVus
                .Any(x => x.UserId == dto.UserId && x.IdNhaCungCapDichVu == dto.IdNhaCungCapDichVu && !x.Deleted);
            if (!isUserBelongsToNcc)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserNhaCungCapDichVuNotFound);
            }
            var thoiGianKetThuc = dto.ThoiGianKetThucApDungHanMuc ?? dto.ThoiGianBatDauApDungHanMuc.AddMonths(1);
            var userCreditExist = _smDbContext.UserCredits.FirstOrDefault(
                x => x.UserId == dto.UserId
                && !x.Deleted
                && x.ThoiGianBatDauApDungHanMuc < thoiGianKetThuc
                && x.ThoiGianKetThucApDungHanMuc > dto.ThoiGianBatDauApDungHanMuc);
            if (userCreditExist != null)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserCreditsExist);
            }

            // Kiểm tra bản ghi gần nhất của userId và cộng thêm credit còn lại nếu có
            var hanMucCredit = Convert.ToInt64(dto.HanMucCredit);
            var latestUserCredit = _smDbContext.UserCredits
                .Where(x => x.UserId == dto.UserId
                && !x.Deleted
                && x.ThoiGianKetThucApDungHanMuc <= dto.ThoiGianBatDauApDungHanMuc)
                .OrderByDescending(x => x.ThoiGianKetThucApDungHanMuc)
                .FirstOrDefault();
            if (latestUserCredit != null)
            {
                var creditConLai = Convert.ToInt64(
                    string.IsNullOrEmpty(latestUserCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc)
                    ? "0"
                    : latestUserCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc
                );
                if (creditConLai > 0)
                {
                    hanMucCredit += creditConLai;
                }
            }

            
            var userCredits = new domain.Config.UserCredits
            {
                UserId = dto.UserId,
                IdNhaCungCapDichVu = dto.IdNhaCungCapDichVu,
                HanMucCredit = hanMucCredit.ToString(),
                CreditChuaSuDung = "0",
                CreditConSauKhiKetThucThoiGianApDungHanMuc = "0",
                CreditDaSuDung = "0",

                ThoiGianBatDauApDungHanMuc = dto.ThoiGianBatDauApDungHanMuc,
                ThoiGianKetThucApDungHanMuc = dto.ThoiGianKetThucApDungHanMuc ?? dto.ThoiGianBatDauApDungHanMuc.AddMonths(1),
                //LoaiApiCredit = ConfigConstants.StringeeApi,
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
            };
            _smDbContext.UserCredits.Add(userCredits);
            _smDbContext.SaveChanges();
        }

        public async Task AddCreditToUserCredits()
        {
            _logger.LogInformation($"{nameof(AddCreditToUserCredits)}");
            var vietNamNow = GetVietnamTime();
            var newPeriodStart = vietNamNow;
            var newPeriodEnd = vietNamNow.AddMonths(1);

            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            try
            {
                // Lấy tối đa hạn mức credit gia hạn
                var toiDaHanMuc = await _smDbContext.ToiDaHanMucCreditsGiaHan
                    .Where(x => !x.Deleted)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync();

                var toiDaHanMucValue = toiDaHanMuc != null
                    ? Convert.ToInt64(toiDaHanMuc.ToiDaHanMucCreditGiaHan)
                    : Convert.ToInt64(SoTienMacDinhConstants.SoTienMacDinh);

                // Lấy bản ghi hết hạn gần nhất của từng user
                var latestUserCredit = await _smDbContext.UserCredits
                    .Where(x => !x.Deleted && x.ThoiGianKetThucApDungHanMuc < vietNamNow)
                    .GroupBy(x => x.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        LastestCredit = g.OrderByDescending(x => x.ThoiGianKetThucApDungHanMuc).FirstOrDefault(),
                    })
                    .ToListAsync();

                // Lấy các user đã có credit active trong kỳ mới → tránh tạo trùng
                var usersAlreadyHaveActiveCredit = await _smDbContext.UserCredits
                    .Where(x => !x.Deleted
                        && x.ThoiGianBatDauApDungHanMuc < newPeriodEnd
                        && x.ThoiGianKetThucApDungHanMuc > newPeriodStart)
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToListAsync();

                var newCredits = new List<domain.Config.UserCredits>();

                foreach (var userCredits in latestUserCredit)
                {
                    // Bỏ qua user đã có credit active trong kỳ mới
                    if (usersAlreadyHaveActiveCredit.Contains(userCredits.UserId))
                        continue;

                    var soTienMacDinh = Convert.ToInt64(SoTienMacDinhConstants.SoTienMacDinh);
                    var creditConLai = Convert.ToInt64(
                        string.IsNullOrEmpty(userCredits.LastestCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc)
                        ? "0"
                        : userCredits.LastestCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc
                    );

                    var hanMucCreditMoi = soTienMacDinh + (creditConLai > 0 ? creditConLai : 0);

                    // Kiểm tra nếu hạn mức mới >= tối đa thì bỏ qua không add
                    if (hanMucCreditMoi >= toiDaHanMucValue)
                    {
                        _logger.LogInformation(
                            $"{nameof(AddCreditToUserCredits)} - UserId: {userCredits.UserId} bị bỏ qua vì hanMucCreditMoi={hanMucCreditMoi} >= toiDaHanMuc={toiDaHanMucValue}"
                        );
                        continue;
                    }

                    var newUserCredits = new domain.Config.UserCredits
                    {
                        UserId = userCredits.UserId,
                        IdNhaCungCapDichVu = userCredits.LastestCredit.IdNhaCungCapDichVu,
                        HanMucCredit = hanMucCreditMoi.ToString(),
                        CreditChuaSuDung = "0",
                        CreditDaSuDung = "0",
                        CreditConSauKhiKetThucThoiGianApDungHanMuc = "0",
                        ThoiGianBatDauApDungHanMuc = newPeriodStart,
                        ThoiGianKetThucApDungHanMuc = newPeriodEnd,
                        CreatedDate = vietNamNow,
                        CreatedBy = "CronJob"
                    };

                    newCredits.Add(newUserCredits);
                }

                if (newCredits.Any())
                {
                    await _smDbContext.UserCredits.AddRangeAsync(newCredits);
                    await _smDbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public void UpdateUserCredits(UpdateUserCreditsDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateUserCredits)} dto = {JsonSerializer.Serialize(dto)}");
           
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();

            
            var userCredits = _smDbContext.UserCredits.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
            

            if(vietNamNow > userCredits.ThoiGianKetThucApDungHanMuc)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserCreditsQuaHanGiaHanHanMuc);
            }

            var thoiGianKetThucMoi = dto.ThoiGianKetThucApDungHanMuc ?? dto.ThoiGianBatDauApDungHanMuc.AddMonths(1);
            if (dto.ThoiGianBatDauApDungHanMuc >= thoiGianKetThucMoi || dto.ThoiGianBatDauApDungHanMuc <= vietNamNow)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorThoiGianKhongHopLe);
            }
            var isOverlap = _smDbContext.UserCredits.Any(
                x => x.UserId == userCredits.UserId
                && x.Id != dto.Id
                && !x.Deleted
                && x.ThoiGianBatDauApDungHanMuc < thoiGianKetThucMoi
                && x.ThoiGianKetThucApDungHanMuc > dto.ThoiGianBatDauApDungHanMuc);
            if (isOverlap)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserCreditsExist);
            }

            userCredits.HanMucCredit = dto.HanMucCredit;
            userCredits.ThoiGianBatDauApDungHanMuc = dto.ThoiGianBatDauApDungHanMuc;
            userCredits.ThoiGianKetThucApDungHanMuc = dto.ThoiGianKetThucApDungHanMuc ?? dto.ThoiGianBatDauApDungHanMuc.AddMonths(1);
            userCredits.ModifiedBy = currentUserId;
            userCredits.ModifiedDate = vietNamNow;
            _smDbContext.UserCredits.Update(userCredits);
            _smDbContext.SaveChanges();
        }

        public void DeleteUserCredits(int id)
        {
            _logger.LogInformation($"{nameof(DeleteUserCredits)} id = {id}");
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
            var userCredits = _smDbContext.UserCredits.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
            userCredits.Deleted = true;
            userCredits.DeletedBy = currentUserId;
            userCredits.DeletedDate = vietNamNow;
            _smDbContext.UserCredits.Update(userCredits);
            _smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewUserCreditsDto> Find(FindPagingDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto = {JsonSerializer.Serialize(dto)}");
            var vietNamNow = GetVietnamTime();

            // AsEnumerable() để xử lý GroupBy + OrderBy ở client
            var nearestIds = _smDbContext.UserCredits
                .Where(x => !x.Deleted)
                .AsEnumerable()
                .GroupBy(x => x.UserId)
                .Select(g => g.OrderBy(x => Math.Abs((x.ThoiGianKetThucApDungHanMuc - vietNamNow).Ticks))
                              .First().Id)
                .ToList();

            var query = from uc in _smDbContext.UserCredits
                        join u in _smDbContext.Users on uc.UserId equals u.Id
                        where !uc.Deleted && nearestIds.Contains(uc.Id)
                        select new ViewUserCreditsDto
                        {
                            User = new ViewUserDto
                            {
                                UserId = u.Id,
                                UserName = u.UserName ?? "",
                                FullName = u.FullName,
                                Email = u.Email ?? "",
                            },
                            Id = uc.Id,
                            HanMucCredit = uc.HanMucCredit,
                            ThoiGianBatDauApDungHanMuc = uc.ThoiGianBatDauApDungHanMuc,
                            ThoiGianKetThucApDungHanMuc = uc.ThoiGianKetThucApDungHanMuc,
                            CreditDaSuDung = uc.CreditDaSuDung,
                            CreditChuaSuDung = uc.CreditChuaSuDung,
                            CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                            DonVi = "VND"
                        };

            var data = query.Paging(dto).ToList();
            var totalItems = query.Count();

            var userIds = data.Select(x => x.User.UserId).Distinct().ToList();
            var userNccList = (from uncc in _smDbContext.UserNhaCungCapDichVus
                               join ncc in _smDbContext.NhaCungCapDichVus on uncc.IdNhaCungCapDichVu equals ncc.Id
                               join bn in _smDbContext.BrandName on uncc.IdBrandName equals bn.Id
                               where userIds.Contains(uncc.UserId)
                                   && !uncc.Deleted
                                   && !ncc.Deleted
                                   && !bn.Deleted
                               select new
                               {
                                   uncc.UserId,
                                   IdNhaCungCapDichVu = ncc.Id,
                                   TenNhaCungCapDichVu = ncc.Name,
                                   IdBrandName = bn.Id,
                                   bn.TenBrandName
                               }).ToList();

            foreach (var item in data)
            {
                item.NhaCungCapDichVus = userNccList
                    .Where(x => x.UserId == item.User.UserId)
                    .GroupBy(x => new { x.IdNhaCungCapDichVu, x.TenNhaCungCapDichVu })
                    .Select(g => new ViewNhaCungCapDichVuByUserCredits
                    {
                        IdNhaCungCapDichVu = g.Key.IdNhaCungCapDichVu,
                        TenNhaCungCapDichVu = g.Key.TenNhaCungCapDichVu,
                        BrandNames = g.Select(b => new ViewBrandNameByUserCreditsNhaCungCapDichVu
                        {
                            Id = b.IdBrandName,
                            TenBrandName = b.TenBrandName
                        }).ToList()
                    }).ToList();
            }

            return new BaseResponsePagingDto<ViewUserCreditsDto>
            {
                Items = data,
                TotalItems = totalItems,
            };
        }

        public ViewUserCreditsDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)} id = {id}");

            var result = (from uc in _smDbContext.UserCredits
                          join u in _smDbContext.Users on uc.UserId equals u.Id
                          where uc.Id == id && !uc.Deleted
                          select new ViewUserCreditsDto
                          {
                              Id = uc.Id,
                              User = new ViewUserDto
                              {
                                  UserId = u.Id,
                                  UserName = u.UserName ?? "",
                                  FullName = u.FullName,
                                  Email = u.Email ?? "",
                              },
                              HanMucCredit = uc.HanMucCredit,
                              ThoiGianBatDauApDungHanMuc = uc.ThoiGianBatDauApDungHanMuc,
                              ThoiGianKetThucApDungHanMuc = uc.ThoiGianKetThucApDungHanMuc,
                              CreditDaSuDung = uc.CreditDaSuDung,
                              CreditChuaSuDung = uc.CreditChuaSuDung,
                              CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                              DonVi = "VND"
                          })
                          .FirstOrDefault()
                          ?? throw new UserFriendlyException(ErrorCodes.NotFound);

            // Enrich NhaCungCapDichVus
            var userNccList = (from uncc in _smDbContext.UserNhaCungCapDichVus
                               join ncc in _smDbContext.NhaCungCapDichVus on uncc.IdNhaCungCapDichVu equals ncc.Id
                               join bn in _smDbContext.BrandName on uncc.IdBrandName equals bn.Id
                               where uncc.UserId == result.User.UserId
                                   && !uncc.Deleted
                                   && !ncc.Deleted
                                   && !bn.Deleted
                               select new
                               {
                                   IdNhaCungCapDichVu = ncc.Id,
                                   TenNhaCungCapDichVu = ncc.Name,
                                   IdBrandName = bn.Id,
                                   bn.TenBrandName
                               }).ToList();

            result.NhaCungCapDichVus = userNccList
                .GroupBy(x => new { x.IdNhaCungCapDichVu, x.TenNhaCungCapDichVu })
                .Select(g => new ViewNhaCungCapDichVuByUserCredits
                {
                    IdNhaCungCapDichVu = g.Key.IdNhaCungCapDichVu,
                    TenNhaCungCapDichVu = g.Key.TenNhaCungCapDichVu,
                    BrandNames = g.Select(b => new ViewBrandNameByUserCreditsNhaCungCapDichVu
                    {
                        Id = b.IdBrandName,
                        TenBrandName = b.TenBrandName
                    }).ToList()
                }).ToList();

            return result;
        }

        public GetDonViDto GetDonVi(int id)
        {
            _logger.LogInformation($"{nameof(GetDonVi)} id = {id}");
            var userCredits = _smDbContext.UserCredits.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
            return new GetDonViDto
            {
                DonVi = userCredits.DonVi
            };
        }


        public BaseResponsePagingDto<ViewUserCreditsDto> FindPagingByUserId(FindPagingByUserIdDto dto)
        {
            _logger.LogInformation($"{nameof(FindPagingByUserId)} dto = {JsonSerializer.Serialize(dto)}");

            var query = from uc in _smDbContext.UserCredits
                        join u in _smDbContext.Users on uc.UserId equals u.Id
                        where !uc.Deleted && uc.UserId == dto.UserId
                        select new ViewUserCreditsDto
                        {
                            Id = uc.Id,
                            User = new ViewUserDto
                            {
                                UserId = u.Id,
                                UserName = u.UserName ?? "",
                                FullName = u.FullName,
                                Email = u.Email ?? "",
                            },
                            HanMucCredit = uc.HanMucCredit,
                            ThoiGianBatDauApDungHanMuc = uc.ThoiGianBatDauApDungHanMuc,
                            ThoiGianKetThucApDungHanMuc = uc.ThoiGianKetThucApDungHanMuc,
                            CreditDaSuDung = uc.CreditDaSuDung,
                            CreditChuaSuDung = uc.CreditChuaSuDung,
                            CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                            DonVi = "VND"
                        };

            var data = query.Paging(dto).ToList();
            var totalItems = query.Count();

            var userNccList = (from uncc in _smDbContext.UserNhaCungCapDichVus
                               join ncc in _smDbContext.NhaCungCapDichVus on uncc.IdNhaCungCapDichVu equals ncc.Id
                               join bn in _smDbContext.BrandName on uncc.IdBrandName equals bn.Id
                               where uncc.UserId == dto.UserId
                                   && !uncc.Deleted
                                   && !ncc.Deleted
                                   && !bn.Deleted
                               select new
                               {
                                   IdNhaCungCapDichVu = ncc.Id,
                                   TenNhaCungCapDichVu = ncc.Name,
                                   IdBrandName = bn.Id,
                                   bn.TenBrandName
                               }).ToList();

            var nhaCungCapDichVus = userNccList
                .GroupBy(x => new { x.IdNhaCungCapDichVu, x.TenNhaCungCapDichVu })
                .Select(g => new ViewNhaCungCapDichVuByUserCredits
                {
                    IdNhaCungCapDichVu = g.Key.IdNhaCungCapDichVu,
                    TenNhaCungCapDichVu = g.Key.TenNhaCungCapDichVu,
                    BrandNames = g.Select(b => new ViewBrandNameByUserCreditsNhaCungCapDichVu
                    {
                        Id = b.IdBrandName,
                        TenBrandName = b.TenBrandName
                    }).ToList()
                }).ToList();

            foreach (var item in data)
            {
                item.NhaCungCapDichVus = nhaCungCapDichVus;
            }

            return new BaseResponsePagingDto<ViewUserCreditsDto>
            {
                Items = data,
                TotalItems = totalItems,
            };
        }
        public ViewUserCreditsByUserDto? GetCurrentUserCredits()
        {
            _logger.LogInformation($"{nameof(GetCurrentUserCredits)}");

            var isSuperAdmin = IsSuperAdmin();
            if (isSuperAdmin)
            {
                return null;
            }

            var currentUserId = getCurrentUserId();
            var vietnamNow = GetVietnamTime();

            var result = (from uc in _smDbContext.UserCredits
                          join u in _smDbContext.Users on uc.UserId equals u.Id
                          where !uc.Deleted
                                && uc.UserId == currentUserId
                                && uc.ThoiGianBatDauApDungHanMuc <= vietnamNow
                                && uc.ThoiGianKetThucApDungHanMuc >= vietnamNow
                          orderby uc.ThoiGianKetThucApDungHanMuc descending
                          select new ViewUserCreditsByUserDto
                          {
                              UserCredit = new ViewUserCreditDto
                              {
                                  UserId = u.Id,
                                  UserName = u.UserName ?? string.Empty,
                                  FullName = u.FullName,
                                  Email = u.Email ?? string.Empty,
                              },
                              HanMucCredit = uc.HanMucCredit,
                              CreditDaSuDung = uc.CreditDaSuDung,
                              CreditChuaSuDung = uc.CreditChuaSuDung,
                              DonVi = uc.DonVi
                          })
                          .FirstOrDefault();

            return result;
        }

        public void UpdateToiDaHanMucCreditsGiaHan (UpdateToiDaHanMucCreditsGiaHanDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateToiDaHanMucCreditsGiaHan)} dto = {JsonSerializer.Serialize(dto)}");
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var vietNamNow = GetVietnamTime();
            var toiDaHanMuc = _smDbContext.ToiDaHanMucCreditsGiaHan.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted);

            toiDaHanMuc.ToiDaHanMucCreditGiaHan = dto.ToiDaHanMucCreditGiaHan;
            toiDaHanMuc.ModifiedBy = currentUserId;
            toiDaHanMuc.ModifiedDate = vietNamNow;
            _smDbContext.ToiDaHanMucCreditsGiaHan.Update(toiDaHanMuc);
            _smDbContext.SaveChanges();
        }

        public GetToiDaHanMucCreditsGiaHanDto GetToiDaHanMucCreditsGiaHan (int id)
        {
            _logger.LogInformation($"{nameof(GetToiDaHanMucCreditsGiaHan)} id ={id}");
            var toiDaHanMuc = _smDbContext.ToiDaHanMucCreditsGiaHan.FirstOrDefault(x => x.Id == id && !x.Deleted);

            return new GetToiDaHanMucCreditsGiaHanDto
            {
                Id = toiDaHanMuc.Id,
                ToiDaHanMucCreditGiaHan = toiDaHanMuc.ToiDaHanMucCreditGiaHan,
                DonVi =toiDaHanMuc.DonVi,
            };
        }
    }
}
