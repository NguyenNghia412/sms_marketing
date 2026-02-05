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
            var user = _smDbContext.Users.FirstOrDefault(x => x.Id == dto.UserId)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);
            var userCreditExist = _smDbContext.UserCredits.FirstOrDefault(x => x.UserId == dto.UserId && x.ThoiGianBatDauApDungHanMuc == dto.ThoiGianBatDauApDungHanMuc && !x.Deleted);
            if (userCreditExist != null)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserCreditsExist);
            }

            // Kiểm tra bản ghi gần nhất của userId và cộng thêm credit còn lại nếu có
            var hanMucCredit = Convert.ToInt64(dto.HanMucCredit);
            var latestUserCredit = _smDbContext.UserCredits
                .Where(x => x.UserId == dto.UserId && !x.Deleted && x.ThoiGianKetThucApDungHanMuc <= vietNamNow)
                .OrderByDescending(x => x.ThoiGianKetThucApDungHanMuc)
                .FirstOrDefault();
            if (latestUserCredit != null)
            {
                var creditConLai = Convert.ToInt64(latestUserCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc ?? "0");
                if (creditConLai > 0)
                {
                    hanMucCredit += creditConLai;
                }
            }

            
            var userCredits = new domain.Config.UserCredits
            {
                UserId = dto.UserId,
                HanMucCredit = hanMucCredit.ToString(),
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
            //var hanMucCredit = SoTienMacDinhConstants.SoTienMacDinh;
            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            try
            {
                var userCreditsDistinct = await _smDbContext.UserCredits
                .Where( x=>!x.Deleted)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();

            var latestUserCredit = await _smDbContext.UserCredits
                .Where( x => !x.Deleted 
                && x.ThoiGianKetThucApDungHanMuc < vietNamNow)
                .GroupBy( x => x.UserId)
                .Select( g => new
                {
                    UserId = g.Key,
                    LastestCredit = g.OrderByDescending( x=> x.ThoiGianKetThucApDungHanMuc).FirstOrDefault(),

                })
                .ToListAsync();
            var newCredits = new List<domain.Config.UserCredits>();
            foreach (var userCredits in latestUserCredit)
            {
                var hanMucCredit = SoTienMacDinhConstants.SoTienMacDinh;
                if(userCredits != null)
                {
                    var creditConLai = Convert.ToInt64(userCredits.LastestCredit.CreditConSauKhiKetThucThoiGianApDungHanMuc ?? "0");
                    if (creditConLai > 0)
                    {
                        hanMucCredit += creditConLai;
                    }
                }
                    var newUserCredits = new domain.Config.UserCredits
                    {
                        UserId = userCredits.UserId,
                        HanMucCredit = hanMucCredit.ToString(),
                        ThoiGianBatDauApDungHanMuc = vietNamNow,
                        ThoiGianKetThucApDungHanMuc = vietNamNow.AddMonths(1),
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
            _logger.LogInformation($"{nameof(AddUserCreditsDto)} dto = {JsonSerializer.Serialize(dto)}");
           
            var vietNamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();

            var user = _smDbContext.Users.FirstOrDefault(x => x.Id == dto.UserId)
                ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);
            var userCredits = _smDbContext.UserCredits.FirstOrDefault(x => x.Id == dto.Id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
            

            if(vietNamNow > userCredits.ThoiGianKetThucApDungHanMuc)
            {
                throw new UserFriendlyException(ErrorCodes.ConfigErrorUserCreditsQuaHanGiaHanHanMuc);
            }


            userCredits.UserId = dto.UserId;
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

        public BaseResponsePagingDto<ViewUserCreditsDto> Find (FindPagingDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto = {JsonSerializer.Serialize(dto)}");
            var query = from uc in _smDbContext.UserCredits
                        join u in _smDbContext.Users on uc.UserId equals u.Id
                        where !uc.Deleted
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
                            //LoaiApiCredit = uc.LoaiApiCredit,
                            CreditDaSuDung = uc.CreditDaSuDung,
                            CreditChuaSuDung = uc.CreditChuaSuDung,
                            CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                            DonVi = "VND"
                        };
            var data = query.Paging(dto).ToList();
            return new BaseResponsePagingDto<ViewUserCreditsDto>
            {
                Items = data,
                TotalItems = query.Count(),
            };
        }

        public ViewUserCreditsDto FindById (int id)
        {
            _logger.LogInformation($"{nameof(FindById)} id = {id}");
            var userCredits = _smDbContext.UserCredits.FirstOrDefault(x => x.Id == id && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
            var query = from uc in _smDbContext.UserCredits
                        join u in _smDbContext.Users on uc.UserId equals u.Id
                        where uc.Id == id && !uc.Deleted
                        select new ViewUserCreditsDto
                        {
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
                            //LoaiApiCredit = uc.LoaiApiCredit,
                            CreditDaSuDung = uc.CreditDaSuDung,
                            CreditChuaSuDung = uc.CreditChuaSuDung,
                            CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                            DonVi = "VND"
                        };
            var result = query.FirstOrDefault()
                ?? throw new UserFriendlyException(ErrorCodes.NotFound);
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
            var currentUserId = getCurrentUserId();
            var query = from uc in _smDbContext.UserCredits
                        join u in _smDbContext.Users on uc.UserId equals u.Id
                        where !uc.Deleted && uc.UserId == currentUserId
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
                            //LoaiApiCredit = uc.LoaiApiCredit,
                            CreditDaSuDung = uc.CreditDaSuDung,
                            CreditChuaSuDung = uc.CreditChuaSuDung,
                            CreditConSauKhiKetThucThoiGianApDungHanMuc = uc.CreditConSauKhiKetThucThoiGianApDungHanMuc,
                            DonVi = "VND"
                        };
            var data = query.Paging(dto).ToList();
            return new BaseResponsePagingDto<ViewUserCreditsDto>
            {
                Items = data,
                TotalItems = query.Count(),
            };
        }
    }
}
