using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.infrastructure.data;
using thongbao.be.lib.Stringee.Implements;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.Constants.ChienDich;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using Volo.Abp.Users;

namespace thongbao.be.application.GuiTinNhan.Implements
{

    public class GuiTinNhanService : BaseService, IGuiTinNhanService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IProfileService _profileService;
        private readonly ISendSmsService _sendSmsService;
        private readonly UserManager<AppUser> _userManager;
        private const int BATCH_SIZE = 400;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public GuiTinNhanService(
             SmDbContext smDbContext,
            ILogger<GuiTinNhanService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IBackgroundJobClient backgroundJobClient,
            ISendSmsService sendSmsService,
            UserManager<AppUser> userManager,
            IProfileService profileService) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _backgroundJobClient = backgroundJobClient;
            _profileService = profileService;
            _userManager = userManager;
            _sendSmsService = sendSmsService;
        }


        /// <summary>
        /// Hủy chiến dịch đang gửi hoặc đã lên lịch
        /// - Nếu trạng thái là DangGui: Kill job hiện tại, chuyển trạng thái về Huy, lưu log các tin nhắn đã gửi
        /// - Nếu trạng thái là LenLich: Kill scheduler job, chuyển trạng thái về Huy, lưu ChienDichLogTrangThaiGui với TrangThai = Cancelled
        /// </summary>
        public async Task HuyChienDich(HuyJobSendSmsDto dto)
        {
            _logger.LogInformation($"{nameof(HuyChienDich)}, dto= {JsonSerializer.Serialize(dto)}");
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var vietNamNow = GetVietnamTime();

            var chienDich = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == dto.IdChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            if (chienDich.TrangThai != ChienDichConstants.DangGui && chienDich.TrangThai != ChienDichConstants.LenLich)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorChuaDuocDatLenhKhongTheHuy);
            }

            var chienDichBackgroundJob = await _smDbContext.ChienDichBackgroundJobs
                 .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                 .FirstOrDefaultAsync()
                 ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorChuaDuocDatLenhKhongTheHuy);

            var jobId = chienDichBackgroundJob.IdBackgroundJob.ToString();

            using var transaction = await _smDbContext.Database.BeginTransactionAsync();
            try
            {
                if (chienDich.TrangThai == ChienDichConstants.DangGui)
                {
                    chienDich.TrangThai = ChienDichConstants.Huy;
                    _smDbContext.ChienDiches.Update(chienDich);

                    chienDichBackgroundJob.Deleted = true;
                    chienDichBackgroundJob.DeletedDate = vietNamNow;
                    chienDichBackgroundJob.DeletedBy = currentUserId;

                    _smDbContext.ChienDichBackgroundJobs.Update(chienDichBackgroundJob);

                    var modeDanhBa = false;
                    var modeListSoDienThoai = false;

                    var chienDichDanhBa = await _smDbContext.ChienDichDanhBa.FirstOrDefaultAsync(x => x.IdChienDich == dto.IdChienDich && !x.Deleted);
                    if (chienDichDanhBa != null)
                    {
                        modeDanhBa = true;
                    }
                    else
                    {
                        modeListSoDienThoai = true;

                    }

                    //Mode: DanhBa
                    if (modeDanhBa == true)
                    {
                        var danhBaSmsList = await _smDbContext.DanhBaSms
                           .Where(x => x.IdDanhBa == chienDichDanhBa.IdDanhBa && !x.Deleted)
                           .OrderBy(x => x.Id)
                           .Select(x => new { x.Id, x.SoDienThoai })
                           .AsNoTracking()
                           .ToListAsync();
                        var logChiTietDaGui = await _smDbContext.GuiTinNhanLogChiTiets
                            .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                            .AsNoTracking()
                            .ToListAsync();
                        int tongChiPhiDaGui = logChiTietDaGui.Where(x => x.TrangThai == "Success").Sum(x => x.Price);
                        int smsSuccess = logChiTietDaGui.Count(x => x.TrangThai == "Success");

                        var danhBaCount = danhBaSmsList.Count;

                        var smsFailedCount = danhBaCount - smsSuccess;

                        var chienDichLogTrangThaiGui = new domain.GuiTinNhan.ChienDichLogTrangThaiGui
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = chienDichDanhBa.IdDanhBa,
                            IdBrandName = dto.IdBrandName,
                            TongSoSms = danhBaCount,
                            SmsSendSuccess = smsSuccess,
                            SmsSendFailed = smsFailedCount,
                            TrangThai = "Cancelled",
                            NoiDung = chienDich.NoiDung ?? string.Empty,
                            TongChiPhi = tongChiPhiDaGui,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        };
                        var danhBaSmsIdsAlreadySent = logChiTietDaGui.Select(x => x.IdDanhBaSms).ToHashSet();
                        var danhBaSmsNotSent = danhBaSmsList.Where(x => !danhBaSmsIdsAlreadySent.Contains(x.Id)).ToList();
                        _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLogTrangThaiGui);

                        var logChiTietChuaGuiList = danhBaSmsNotSent.Select(item => new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = chienDichDanhBa.IdDanhBa,
                            IdBrandName = dto.IdBrandName,
                            IdDanhBaSms = item.Id,
                            SoDienThoai = item.SoDienThoai,
                            NoiDungChiTiet = chienDich.NoiDung ?? string.Empty,
                            Price = 0,
                            Code = -1,
                            Message = "Hủy lệnh gửi",
                            TrangThai = "Cancelled",
                            SoLuongTinNhan = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        }).ToList();

                        await _smDbContext.GuiTinNhanLogChiTiets.AddRangeAsync(logChiTietChuaGuiList);
                    }
                    // Mode:List Số điện thoại 
                    else if (modeListSoDienThoai == true)
                    {

                        var chienDichListSoDienThoai = await _smDbContext.ChienDichListSoDienThoais
                            .FirstOrDefaultAsync(x => x.IdChienDich == dto.IdChienDich && !x.Deleted);

                        var danhSachSoDienThoai = new List<string>();
                        if (chienDichListSoDienThoai != null && !string.IsNullOrEmpty(chienDichListSoDienThoai.ListSoDienThoai))
                        {
                            var listFromDb = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(chienDichListSoDienThoai.ListSoDienThoai);
                            danhSachSoDienThoai = listFromDb?.Select(x => x.GetValueOrDefault("SoDienThoai", string.Empty)).Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
                        }

                        var logChiTietDaGui = await _smDbContext.GuiTinNhanLogChiTiets
                          .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                          .AsNoTracking()
                          .ToListAsync();
                        int tongChiPhiDaGui = logChiTietDaGui.Where(x => x.TrangThai == "Success").Sum(x => x.Price);
                        int smsSuccess = logChiTietDaGui.Count(x => x.TrangThai == "Success");
                        int smsFailed = logChiTietDaGui.Count(x => x.TrangThai == "Failed");
                        var smsCount = danhSachSoDienThoai.Count;
                        var smsFailedCount = smsCount - smsSuccess;
                        var chienDichLogTrangThaiGui = new domain.GuiTinNhan.ChienDichLogTrangThaiGui
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = null,
                            IdBrandName = dto.IdBrandName,
                            TongSoSms = smsCount,
                            SmsSendSuccess = smsSuccess,
                            SmsSendFailed = smsFailedCount,
                            TrangThai = "Cancelled",
                            NoiDung = chienDich.NoiDung ?? string.Empty,
                            TongChiPhi = tongChiPhiDaGui,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        };
                        _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLogTrangThaiGui);
                        var soDienThoaiAlreadySent = logChiTietDaGui.Select(x => x.SoDienThoai).ToHashSet();
                        var soDienThoaiNotSent = danhSachSoDienThoai.Where(x => !soDienThoaiAlreadySent.Contains(x)).ToList();

                        var logChiTietChuaGuiList = soDienThoaiNotSent.Select(sdt => new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = null,
                            IdBrandName = dto.IdBrandName,
                            IdDanhBaSms = null,
                            SoDienThoai = sdt,
                            NoiDungChiTiet = chienDich.NoiDung ?? string.Empty,
                            Price = 0,
                            Code = -1,
                            Message = "Hủy lệnh gửi",
                            TrangThai = "Cancelled",
                            SoLuongTinNhan = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        }).ToList();

                        await _smDbContext.GuiTinNhanLogChiTiets.AddRangeAsync(logChiTietChuaGuiList);
                    }

                    await _smDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"HuyChienDich - Cancelled DangGui job - idChienDich: {dto.IdChienDich}, jobId: {jobId}");
                }
                else if (chienDich.TrangThai == ChienDichConstants.LenLich)
                {
                    BackgroundJob.Delete(jobId);

                    chienDich.TrangThai = ChienDichConstants.Huy;

                    _smDbContext.ChienDiches.Update(chienDich);

                    chienDichBackgroundJob.Deleted = true;
                    chienDichBackgroundJob.DeletedDate = vietNamNow;
                    chienDichBackgroundJob.DeletedBy = currentUserId;

                    _smDbContext.ChienDichBackgroundJobs.Update(chienDichBackgroundJob);
                    var modeDanhBa = false;
                    var modeListSoDienThoai = false;
                    var chienDichDanhBa = await _smDbContext.ChienDichDanhBa.FirstOrDefaultAsync(x => x.IdChienDich == dto.IdChienDich && !x.Deleted);
                    if (chienDichDanhBa != null)
                    {
                        modeDanhBa = true;
                    }
                    else
                    {
                        modeListSoDienThoai = true;

                    }
                    //Mode: DanhBa
                    if (modeDanhBa == true)
                    {
                        var danhBaSmsList = await _smDbContext.DanhBaSms
                           .Where(x => x.IdDanhBa == chienDichDanhBa.IdDanhBa && !x.Deleted)
                           .OrderBy(x => x.Id)
                           .Select(x => new { x.Id, x.SoDienThoai })
                           .AsNoTracking()
                           .ToListAsync();
                        var logChiTietDaGui = await _smDbContext.GuiTinNhanLogChiTiets
                            .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                            .AsNoTracking()
                            .ToListAsync();

                        var danhBaCount = danhBaSmsList.Count;

                        var chienDichLogTrangThaiGui = new domain.GuiTinNhan.ChienDichLogTrangThaiGui
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = chienDichDanhBa.IdDanhBa,
                            IdBrandName = dto.IdBrandName,
                            TongSoSms = danhBaCount,
                            SmsSendSuccess = 0,
                            SmsSendFailed = danhBaCount,
                            TrangThai = "Cancelled",
                            NoiDung = chienDich.NoiDung ?? string.Empty,
                            TongChiPhi = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        };
                        _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLogTrangThaiGui);
                        var danhBaSmsIdsAlreadySent = logChiTietDaGui.Select(x => x.IdDanhBaSms).ToHashSet();
                        var danhBaSmsNotSent = danhBaSmsList.Where(x => !danhBaSmsIdsAlreadySent.Contains(x.Id)).ToList();

                        var logChiTietChuaGuiList = danhBaSmsNotSent.Select(item => new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = chienDichDanhBa.IdDanhBa,
                            IdBrandName = dto.IdBrandName,
                            IdDanhBaSms = item.Id,
                            SoDienThoai = item.SoDienThoai,
                            NoiDungChiTiet = chienDich.NoiDung ?? string.Empty,
                            Price = 0,
                            Code = -1,
                            Message = "Chiến dịch bị hủy",
                            TrangThai = "Cancelled",
                            SoLuongTinNhan = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        }).ToList();

                        await _smDbContext.GuiTinNhanLogChiTiets.AddRangeAsync(logChiTietChuaGuiList);
                    }
                    // Mode:List Số điện thoại 
                    else if (modeListSoDienThoai == true)
                    {
                  
                        var chienDichListSoDienThoai = await _smDbContext.ChienDichListSoDienThoais
                            .FirstOrDefaultAsync(x => x.IdChienDich == dto.IdChienDich && !x.Deleted);

                        var danhSachSoDienThoai = new List<string>();
                        if (chienDichListSoDienThoai != null && !string.IsNullOrEmpty(chienDichListSoDienThoai.ListSoDienThoai))
                        {
                            var listFromDb = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(chienDichListSoDienThoai.ListSoDienThoai);
                            danhSachSoDienThoai = listFromDb?.Select(x => x.GetValueOrDefault("SoDienThoai", string.Empty)).Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
                        }

                        var logChiTietDaGui = await _smDbContext.GuiTinNhanLogChiTiets
                          .Where(x => x.IdChienDich == dto.IdChienDich && !x.Deleted)
                          .AsNoTracking()
                          .ToListAsync();

                        var smsCount = danhSachSoDienThoai.Count;

                        var chienDichLogTrangThaiGui = new domain.GuiTinNhan.ChienDichLogTrangThaiGui
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = null,
                            IdBrandName = dto.IdBrandName,
                            TongSoSms = smsCount,
                            SmsSendSuccess = 0,
                            SmsSendFailed = smsCount,
                            TrangThai = "Cancelled",
                            NoiDung = chienDich.NoiDung ?? string.Empty,
                            TongChiPhi = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        };
                        _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLogTrangThaiGui);
                        var soDienThoaiAlreadySent = logChiTietDaGui.Select(x => x.SoDienThoai).ToHashSet();
                        var soDienThoaiNotSent = danhSachSoDienThoai.Where(x => !soDienThoaiAlreadySent.Contains(x)).ToList();

                        var logChiTietChuaGuiList = soDienThoaiNotSent.Select(sdt => new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = dto.IdChienDich,
                            IdDanhBa = null,
                            IdBrandName = dto.IdBrandName,
                            IdDanhBaSms = null,
                            SoDienThoai = sdt,
                            NoiDungChiTiet = chienDich.NoiDung ?? string.Empty,
                            Price = 0,
                            Code = -1,
                            Message = "Chiến dịch bị hủy",
                            TrangThai = "Cancelled",
                            SoLuongTinNhan = 0,
                            CreatedDate = vietNamNow,
                            CreatedBy = currentUserId
                        }).ToList();

                        await _smDbContext.GuiTinNhanLogChiTiets.AddRangeAsync(logChiTietChuaGuiList);
                    }

                    await _smDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"HuyChienDich - Cancelled LenLich job - idChienDich: {dto.IdChienDich}, jobId: {jobId}");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"HuyChienDich - ERROR - idChienDich: {dto.IdChienDich}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> StartGuiTinNhanJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung)
        {
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var vietNamNow = GetVietnamTime();
            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);
            await ValidateChienDichChuaGui(idChienDich);
            await ValidateSoLuongTinNhan(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
      
            
            await SaveThongTinChienDich(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
           
            var estimatedAmount = await GetChiPhiDuTruChienDich(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
            //var profileInfo = await _profileService.GetProfileStringeeInfor();
            //var amount = Convert.ToInt32(profileInfo?.Data?.Amount ?? 0);
            if (!isSuperAdmin)
            {
                var userCredit = await _smDbContext.UserCredits
                    .Where(x => x.UserId == currentUserId && !x.Deleted)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync();

                var hanMuc = Convert.ToInt32(userCredit?.HanMucCredit ?? "0");
                var daSuDung = Convert.ToInt32(userCredit?.CreditDaSuDung ?? "0");
                var creditHienTai = hanMuc - daSuDung;

                if (estimatedAmount > creditHienTai)
                {
                    await SendWarningToAdmin(idChienDich, estimatedAmount, creditHienTai);
                    throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorNotEnoughBalance);
                }
            }
            var chienDich = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich != null)
            {
                chienDich.TrangThai = ChienDichConstants.DangGui;
                _smDbContext.ChienDiches.Update(chienDich);
                await _smDbContext.SaveChangesAsync();
            }
            var jobId = _backgroundJobClient.Enqueue<IGuiTinNhanJobService>(x =>
               x.ProcessGuiTinNhanBackground(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung, currentUserId, isSuperAdmin));
            //var chienDichDaGui = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

            var chienDichJobId = new domain.GuiTinNhan.ChienDichBackgroundJob
            {
                IdChienDich = idChienDich,
                IdBackgroundJob = long.Parse(jobId),
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
                Deleted = false,

            };
             _smDbContext.ChienDichBackgroundJobs.Add(chienDichJobId);
            await _smDbContext.SaveChangesAsync();

            return jobId;
        }


        // Hàm dùng để gọi scheduler job lên lịch gửi tin nhắn 
        public async Task<string> StartGuiTinNhanSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung, DateTime lichGui)
        {
            var currentUserId = getCurrentUserId();
            var isSuperAdmin = IsSuperAdmin();
            var vietNamNow = GetVietnamTime();
            await ValidateInputSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung, lichGui);
            await ValidateChienDichChuaGui(idChienDich);
            await ValidateSoLuongTinNhanSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);

            await SaveThongTinChienDichCoLichGui(idChienDich, idDanhBa.Value, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung, lichGui);

            var estimatedAmount = await GetChiPhiDuTruChienDichSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung,lichGui);
            //var profileInfo = await _profileService.GetProfileStringeeInfor();
            //var amount = Convert.ToInt32(profileInfo?.Data?.Amount ?? 0);
            if (!isSuperAdmin)
            {
                var userCredit = await _smDbContext.UserCredits
                    .Where(x => x.UserId == currentUserId && !x.Deleted)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync();

                var hanMuc = Convert.ToInt32(userCredit?.HanMucCredit ?? "0");
                var daSuDung = Convert.ToInt32(userCredit?.CreditDaSuDung ?? "0");
                var creditHienTai = hanMuc - daSuDung;

                if (estimatedAmount > creditHienTai)
                {
                    await SendWarningToAdmin(idChienDich, estimatedAmount, creditHienTai);
                    throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorNotEnoughBalance);
                }
            }
            var chienDich = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);
            if (chienDich != null)
            {
                chienDich.TrangThai = ChienDichConstants.LenLich;
                chienDich.LichGui = lichGui;
                _smDbContext.ChienDiches.Update(chienDich);
                await _smDbContext.SaveChangesAsync();
            }
            var jobId = _backgroundJobClient.Schedule<IGuiTinNhanSchedulerJobService>(x =>
               x.ProcessGuiTinNhanBackgroundSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung, currentUserId, isSuperAdmin, lichGui), lichGui);
            //var chienDichDaGui = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

            var chienDichJobId = new domain.GuiTinNhan.ChienDichBackgroundJob
            {
                IdChienDich = idChienDich,
                IdBackgroundJob = long.Parse(jobId),
                CreatedBy = currentUserId,
                CreatedDate = vietNamNow,
                Deleted = false,

            };
            _smDbContext.ChienDichBackgroundJobs.Add(chienDichJobId);
            await _smDbContext.SaveChangesAsync();


            return jobId;
        }


        public async Task<object> GetSoLuongNguoiNhanVaTinNhan(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isFlashSms, bool isAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);

            int soLuongNguoiNhan = 0;
            int tongSoLuongTinNhan = 0;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                soLuongNguoiNhan = allRecords.Count;

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, isAccented);
                    var smsCount = CalculateSmsCount(personalizedText.Length, isAccented);
                    tongSoLuongTinNhan += smsCount;
                }
            }
            // Mode: List số điện thoại
            else
            {
                soLuongNguoiNhan = danhSachSoDienThoai.Count;
                var personalizedText = isAccented ? noiDung : RemoveAccents(noiDung);
                var smsCount = CalculateSmsCount(personalizedText.Length, isAccented);
                tongSoLuongTinNhan = soLuongNguoiNhan * smsCount;
            }

            return new
            {
                SoLuongNguoiNhan = soLuongNguoiNhan,
                TongSoLuongTinNhan = tongSoLuongTinNhan
            };
        }

        public async Task<object> GetSoLuongNguoiNhanVaTinNhanSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int idBrandName, bool isFlashSms, bool isAccented, string noiDung,DateTime lichGui)
        {
            await ValidateInputSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung,lichGui);
            var vietNamNow = GetVietnamTime();
            if (lichGui <= vietNamNow)
            {
                throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorLichGuiKhongHopLe);
            }
            int soLuongNguoiNhan = 0;
            int tongSoLuongTinNhan = 0;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                soLuongNguoiNhan = allRecords.Count;

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, isAccented);
                    var smsCount = CalculateSmsCount(personalizedText.Length, isAccented);
                    tongSoLuongTinNhan += smsCount;
                }
            }
            // Mode: List số điện thoại
            else
            {
                soLuongNguoiNhan = danhSachSoDienThoai.Count;
                var personalizedText = isAccented ? noiDung : RemoveAccents(noiDung);
                var smsCount = CalculateSmsCount(personalizedText.Length, isAccented);
                tongSoLuongTinNhan = soLuongNguoiNhan * smsCount;
            }

            return new
            {
                SoLuongNguoiNhan = soLuongNguoiNhan,
                TongSoLuongTinNhan = tongSoLuongTinNhan
            };
        }
        public async Task<object> GetPreviewMessage(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung, int currentIndex)
        {
            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);
            var brandName = await GetBrandNameByChienDich(idBrandName);

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                if (currentIndex < 1 || currentIndex > allRecords.Count)
                    return null;

                var currentRecord = allRecords[currentIndex - 1];
                var userData = await GetDanhBaDataForBatch(new List<int> { currentRecord.Id }, idChienDich);
                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);
                var formattedPhoneNumber = FormatPhoneNumber(currentRecord.SoDienThoai);
                var length = personalizedText.Length;

                int smsCount = CalculateSmsCount(length, IsAccented);

                return new
                {
                    IdDanhBaSms = currentRecord.Id,
                    SoDienThoai = formattedPhoneNumber,
                    BrandName = brandName ?? string.Empty,
                    PersonalizedText = personalizedText,
                    SmsCount = smsCount
                };
            }
            // Mode: List số điện thoại
            else
            {
                if (danhSachSoDienThoai == null || !danhSachSoDienThoai.Any())
                    return null;

                if (currentIndex < 1 || currentIndex > danhSachSoDienThoai.Count)
                    return null;

                var currentPhone = danhSachSoDienThoai[currentIndex - 1];
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);
                var formattedPhoneNumber = FormatPhoneNumber(currentPhone.SoDienThoai);
                var length = personalizedText.Length;

                int smsCount = CalculateSmsCount(length, IsAccented);

                return new
                {
                    IdDanhBaSms = (int?)null,
                    SoDienThoai = formattedPhoneNumber,
                    BrandName = brandName ?? string.Empty,
                    PersonalizedText = personalizedText,
                    SmsCount = smsCount
                };
            }
        }

        private async Task ValidateInput(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int? idBrandName, string noiDung)
        {
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            /*if (string.IsNullOrWhiteSpace(noiDung))
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest);
            }*/
            int? validIdDanhBa = (idDanhBa.HasValue && idDanhBa.Value > 0) ? idDanhBa : null;
            if (!idDanhBa.HasValue && (danhSachSoDienThoai == null || !danhSachSoDienThoai.Any()))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhSachSoDienThoaiRequired);
            }

            var chienDichExists = await _smDbContext.ChienDiches
                .AnyAsync(x => x.Id == idChienDich && !x.Deleted);

            if (!chienDichExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            if (validIdDanhBa.HasValue)
            {
                var danhBaExists = await _smDbContext.DanhBas
                    .AnyAsync(x => x.Id == validIdDanhBa.Value && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);
                if (!danhBaExists)
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                }
            }

            if (danhSachSoDienThoai != null && danhSachSoDienThoai.Any())
            {
                foreach (var item in danhSachSoDienThoai)
                {
                    var cleanedNumber = Regex.Replace(item.SoDienThoai ?? "", @"[^\d]", "");

                    if (cleanedNumber.Length != 10 && cleanedNumber.Length != 11)
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhSachSoDienThoaiInvalid, item.SoDienThoai);
                    }
                }
            }

            var brandNameExists = await _smDbContext.BrandName
                .AnyAsync(x => x.Id == idBrandName && !x.Deleted);
            /*if (!brandNameExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }*/
        }

        private async Task ValidateInputSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int? idBrandName, string noiDung, DateTime lichGui)
        {
            var isSuperAdmin = IsSuperAdmin();
            var currentUserId = getCurrentUserId();
            var vietnamNow = GetVietnamTime();
            /*if (string.IsNullOrWhiteSpace(noiDung))
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest);
            }*/

            var lichGuiVN = TimeZoneInfo.ConvertTimeFromUtc(lichGui, VietnamTimeZone);
            _logger.LogInformation($"DEBUG LICH GUI ------ {lichGui}");
            int? validIdDanhBa = (idDanhBa.HasValue && idDanhBa.Value > 0) ? idDanhBa : null;
            if (!idDanhBa.HasValue && (danhSachSoDienThoai == null || !danhSachSoDienThoai.Any()))
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhSachSoDienThoaiRequired);
            }

            var chienDichExists = await _smDbContext.ChienDiches
                .AnyAsync(x => x.Id == idChienDich && !x.Deleted);

            if (!chienDichExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            if (validIdDanhBa.HasValue)
            {
                var danhBaExists = await _smDbContext.DanhBas
                    .AnyAsync(x => x.Id == validIdDanhBa.Value && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);
                if (!danhBaExists)
                {
                    throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
                }
            }

            if (danhSachSoDienThoai != null && danhSachSoDienThoai.Any())
            {
                foreach (var item in danhSachSoDienThoai)
                {
                    var cleanedNumber = Regex.Replace(item.SoDienThoai ?? "", @"[^\d]", "");

                    if (cleanedNumber.Length != 10 && cleanedNumber.Length != 11)
                    {
                        throw new UserFriendlyException(ErrorCodes.DanhBaErrorDanhSachSoDienThoaiInvalid, item.SoDienThoai);
                    }
                }
            }

            var brandNameExists = await _smDbContext.BrandName
                .AnyAsync(x => x.Id == idBrandName && !x.Deleted);
            /*if (!brandNameExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }*/
        }
        private async Task ValidateChienDichChuaGui(int idChienDich)
        {
            var chienDich = await _smDbContext.ChienDiches
                .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            if (chienDich.TrangThai == ChienDichConstants.DaGui || chienDich.TrangThai == ChienDichConstants.LenLich || chienDich.TrangThai == ChienDichConstants.DangGui || chienDich.TrangThai == ChienDichConstants.Huy)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorTrangThaiTrue);
            }
        }
        private async Task ValidateSoLuongTinNhan(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            int maxSmsCount = IsAccented ? 402 : 612;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);

                    if (personalizedText.Length > maxSmsCount)
                    {
                        throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorSmsCountExceeded);
                    }
                }
            }
            // Mode: List số điện thoại
            else
            {
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);

                if (personalizedText.Length > maxSmsCount)
                {
                    throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorSmsCountExceeded);
                }
            }
        }


        private async Task ValidateSoLuongTinNhanSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            int maxSmsCount = IsAccented ? 402 : 612;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);

                    if (personalizedText.Length > maxSmsCount)
                    {
                        throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorSmsCountExceeded);
                    }
                }
            }
            // Mode: List số điện thoại
            else
            {
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);

                if (personalizedText.Length > maxSmsCount)
                {
                    throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorSmsCountExceeded);
                }
            }
        }
        public async Task SaveThongTinChienDich(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int? idBrandName, bool IsFlashSms, bool IsAccented, string? noiDung)
        {
            _logger.LogInformation($"{nameof(SaveThongTinChienDich)}");
            var vietnamNow = GetVietnamTime();

            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);
            await ValidateChienDichChuaGui(idChienDich);
            var currentUserId = getCurrentUserId(); 
            var vietNamNow = GetVietnamTime();

          

            var chienDichExisting = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);

            if (idBrandName.HasValue)
            {
                chienDichExisting.IdBrandName = idBrandName.Value;
            }

            chienDichExisting.IsFlashSms = IsFlashSms;
            chienDichExisting.NoiDung = noiDung;
            chienDichExisting.IsAccented = IsAccented;
            chienDichExisting.TrangThai = ChienDichConstants.Nhap;
            chienDichExisting.LichGui =  null;


            //Mode: Danh bạ
            if (idDanhBa.HasValue && idDanhBa.Value > 0)
            {
                var chienDichDanhBa = await _smDbContext.ChienDichDanhBa.FirstOrDefaultAsync(x => x.IdChienDich == idChienDich && x.IdDanhBa == idDanhBa.Value && !x.Deleted);
                var countDanhBa = await _smDbContext.DanhBaSms.Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted).CountAsync();
                chienDichExisting.SoLuongThueBao = countDanhBa;
                _smDbContext.ChienDiches.Update(chienDichExisting);
                if (chienDichDanhBa == null)
                {
                    chienDichDanhBa = new domain.GuiTinNhan.ChienDichDanhBa
                    {
                        IdChienDich = idChienDich,
                        IdDanhBa = idDanhBa.Value,
                    };
                    _smDbContext.ChienDichDanhBa.Add(chienDichDanhBa);
                }
            }
            //Mode: List số điện thoại
            else if (danhSachSoDienThoai != null && danhSachSoDienThoai.Count > 0)
            {
                var countSoDienThoai = danhSachSoDienThoai.Count;
                chienDichExisting.SoLuongThueBao = countSoDienThoai;
                _logger.LogInformation($"DEBUG ---------- LIST SỐ DIỆN THOẠI-----------------------{JsonSerializer.Serialize(danhSachSoDienThoai)}");
                _logger.LogInformation($"DEBUG ---------- COUNT DANH SÁCH SO DIEN THOAI------------------------------{danhSachSoDienThoai.Count}");
                var chienDichListSoDienThoaiExisted = await _smDbContext.ChienDichListSoDienThoais.FirstOrDefaultAsync(x => x.IdChienDich == idChienDich && !x.Deleted);
                if (chienDichListSoDienThoaiExisted == null)
                {
                    var chienDichListSoDienThoai = new domain.GuiTinNhan.ChienDichListSoDienThoai
                    {
                        IdChienDich = idChienDich,
                        ListSoDienThoai = System.Text.Json.JsonSerializer.Serialize(danhSachSoDienThoai),
                        CreatedBy = currentUserId,
                        CreatedDate = vietNamNow,
                    };
                    _smDbContext.ChienDichListSoDienThoais.Add(chienDichListSoDienThoai);
                }
                else
                {
                    chienDichListSoDienThoaiExisted.ListSoDienThoai = System.Text.Json.JsonSerializer.Serialize(danhSachSoDienThoai);
                    _smDbContext.ChienDichListSoDienThoais.Update(chienDichListSoDienThoaiExisted);
                }

            }
            _smDbContext.ChienDiches.Update(chienDichExisting);


            await _smDbContext.SaveChangesAsync();
        }

        //Save thông tin chiến dịch khi có lịch gửi 
        public async Task SaveThongTinChienDichCoLichGui(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int? idBrandName, bool IsFlashSms, bool IsAccented, string? noiDung, DateTime lichGui)
        {
            _logger.LogInformation($"{nameof(SaveThongTinChienDichCoLichGui)}");
            var vietnamNow = GetVietnamTime();
            var currentUserId = getCurrentUserId();
          
            await ValidateInputSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung, lichGui);
            await ValidateChienDichChuaGui(idChienDich);

            if (lichGui <= vietnamNow)
            {
                throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorLichGuiKhongHopLe);
            }

            var chienDichExisting = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);

            if (idBrandName.HasValue)
            {
                chienDichExisting.IdBrandName = idBrandName.Value;
            }

            chienDichExisting.IsFlashSms = IsFlashSms;
            chienDichExisting.NoiDung = noiDung;
            chienDichExisting.IsAccented = IsAccented;
            chienDichExisting.TrangThai = ChienDichConstants.Nhap;
            chienDichExisting.LichGui = lichGui ;


            //Mode: Danh bạ
            if (idDanhBa.HasValue && idDanhBa.Value > 0)
            {
                var chienDichDanhBa = await _smDbContext.ChienDichDanhBa.FirstOrDefaultAsync(x => x.IdChienDich == idChienDich && x.IdDanhBa == idDanhBa.Value && !x.Deleted);
                var countDanhBa = await _smDbContext.DanhBaSms.Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted).CountAsync();
                chienDichExisting.SoLuongThueBao = countDanhBa;
                _smDbContext.ChienDiches.Update(chienDichExisting);
                if (chienDichDanhBa == null)
                {
                    chienDichDanhBa = new domain.GuiTinNhan.ChienDichDanhBa
                    {
                        IdChienDich = idChienDich,
                        IdDanhBa = idDanhBa.Value,
                    };
                    _smDbContext.ChienDichDanhBa.Add(chienDichDanhBa);
                }
            }
            //Mode: List số điện thoại
            else if (danhSachSoDienThoai != null && danhSachSoDienThoai.Count > 0)
            {
                var countSoDienThoai = danhSachSoDienThoai.Count;
                chienDichExisting.SoLuongThueBao = countSoDienThoai;
                var chienDichListSoDienThoaiExisted = await _smDbContext.ChienDichListSoDienThoais.FirstOrDefaultAsync(x => x.IdChienDich == idChienDich && !x.Deleted);
                if (chienDichListSoDienThoaiExisted == null)
                {
                    var chienDichListSoDienThoai = new domain.GuiTinNhan.ChienDichListSoDienThoai
                    {
                        IdChienDich = idChienDich,
                        ListSoDienThoai = System.Text.Json.JsonSerializer.Serialize(danhSachSoDienThoai),
                        CreatedBy = currentUserId,
                        CreatedDate = vietnamNow,
                    };
                    _smDbContext.ChienDichListSoDienThoais.Add(chienDichListSoDienThoai);
                }
                else
                {
                    chienDichListSoDienThoaiExisted.ListSoDienThoai = System.Text.Json.JsonSerializer.Serialize(danhSachSoDienThoai);
                    _smDbContext.ChienDichListSoDienThoais.Update(chienDichListSoDienThoaiExisted);
                }
            }
            _smDbContext.ChienDiches.Update(chienDichExisting);
            await _smDbContext.SaveChangesAsync();
        }
        private async Task SendWarningToAdmin(int idChienDich, int estimatedAmount, int amount)
        {
            _logger.LogInformation($"{nameof(SendWarningToAdmin)}");
            var currentUserId = getCurrentUserId();
            var admins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var chienDich = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            var brandName = _smDbContext.BrandName.FirstOrDefault(x => x.TenBrandName == "HUCE" && !x.Deleted);
            var idBrandName = brandName.Id;
            var IsAccented = true;
            var amountNeeded = estimatedAmount - amount;

            
            var userCredit = await _smDbContext.UserCredits
                .Where(x => x.UserId == currentUserId && !x.Deleted)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
            var hanMuc = Convert.ToInt32(userCredit?.HanMucCredit ?? "0");
            var daSuDung = Convert.ToInt32(userCredit?.CreditDaSuDung ?? "0");
            var creditHienTai = hanMuc - daSuDung;

            
            var noiDung = $"Chiến dịch \"{chienDich.TenChienDich}\" của người dùng \"{currentUser?.FullName ?? currentUserId}\" yêu cầu vượt mức chi phí hiện có. Chi phí chiến dịch: {estimatedAmount:N0}VND, Credits hiện tại của user: {creditHienTai:N0}VND, Cần thêm: {amountNeeded:N0}VND. Xin vui lòng kiểm tra và xử lý. Xin cảm ơn!";

            var smsMessages = new List<object>();
            foreach (var admin in admins)
            {
                if (!string.IsNullOrEmpty(admin.PhoneNumber))
                {
                    var formattedPhoneNumber = FormatPhoneNumber(admin.PhoneNumber);
                    var smsObject = new
                    {
                        from = brandName.TenBrandName,
                        to = formattedPhoneNumber,
                        text = noiDung,
                    };
                    smsMessages.Add(smsObject);
                }
                else
                {
                    continue;
                }
            }
            if (smsMessages.Any())
            {
                await _sendSmsService.SendSmsAsync(smsMessages);
            }
        }
        public async Task<int> GetChiPhiDuTruChienDich(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);

            var networkCosts = new Dictionary<string, decimal>
            {
                ["Viettel"] = 420,
                ["Mobifone"] = 420,
                ["Vinaphone"] = 420,
                ["Vietnamobile"] = 700,
                ["Gmobile"] = 300
            };


            decimal totalCost = 0;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);
                    var formattedNumber = FormatPhoneNumber(record.SoDienThoai);
                    var network = GetNetworkByPhoneNumber(formattedNumber);
                    var smsCount = CalculateSmsCount(personalizedText.Length, IsAccented);

                    if (networkCosts.ContainsKey(network))
                    {
                        totalCost += networkCosts[network] * smsCount;

                    }
                }
            }
            // Mode: List số điện thoại
            else
            {
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);
                var length = personalizedText.Length;

                foreach (var item in danhSachSoDienThoai)
                {
                    var formattedNumber = FormatPhoneNumber(item.SoDienThoai);
                    var network = GetNetworkByPhoneNumber(formattedNumber);
                    var smsCount = CalculateSmsCount(length, IsAccented);

                    if (networkCosts.ContainsKey(network))
                    {
                        totalCost += networkCosts[network] * smsCount;

                    }
                }
            }

            return Convert.ToInt32(totalCost);

        }


        public async Task<int> GetChiPhiDuTruChienDichSchedulerJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiCoLichGuiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung, DateTime lichgui)
        {
            await ValidateInputSchedulerJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung,lichgui);

            var networkCosts = new Dictionary<string, decimal>
            {
                ["Viettel"] = 420,
                ["Mobifone"] = 420,
                ["Vinaphone"] = 420,
                ["Vietnamobile"] = 700,
                ["Gmobile"] = 300
            };


            decimal totalCost = 0;

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                var allRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .Select(x => new { x.Id, x.SoDienThoai })
                    .ToListAsync();

                var recordIds = allRecords.Select(x => x.Id).ToList();
                var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                foreach (var record in allRecords)
                {
                    var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                    var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);
                    var formattedNumber = FormatPhoneNumber(record.SoDienThoai);
                    var network = GetNetworkByPhoneNumber(formattedNumber);
                    var smsCount = CalculateSmsCount(personalizedText.Length, IsAccented);

                    if (networkCosts.ContainsKey(network))
                    {
                        totalCost += networkCosts[network] * smsCount;

                    }
                }
            }
            // Mode: List số điện thoại
            else
            {
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);
                var length = personalizedText.Length;

                foreach (var item in danhSachSoDienThoai)
                {
                    var formattedNumber = FormatPhoneNumber(item.SoDienThoai);
                    var network = GetNetworkByPhoneNumber(formattedNumber);
                    var smsCount = CalculateSmsCount(length, IsAccented);

                    if (networkCosts.ContainsKey(network))
                    {
                        totalCost += networkCosts[network] * smsCount;

                    }
                }
            }

            return Convert.ToInt32(totalCost);

        }
        private async Task<List<DanhBaDataInfoDto>> GetDanhBaDataForBatch(List<int> danhBaChiTietIds, int idChienDich)
        {
            var allDataCount = await _smDbContext.DanhBaDatas
                .Where(x => danhBaChiTietIds.Contains(x.IdDanhBaChiTiet) && !x.Deleted)
                .CountAsync();
            var result = await (from dbd in _smDbContext.DanhBaDatas
                                where danhBaChiTietIds.Contains(dbd.IdDanhBaChiTiet)
                                      && dbd.IdDanhBaChienDich == idChienDich
                                      && !dbd.Deleted
                                select new DanhBaDataInfoDto
                                {
                                    IdDanhBaChiTiet = dbd.IdDanhBaChiTiet,
                                    IdTruongData = dbd.IdTruongData,
                                    Data = dbd.Data
                                })
                              .ToListAsync();


            if (result.Count == 0 && allDataCount > 0)
            {
                result = await (from dbd in _smDbContext.DanhBaDatas
                                where danhBaChiTietIds.Contains(dbd.IdDanhBaChiTiet)
                                      && !dbd.Deleted
                                select new DanhBaDataInfoDto
                                {
                                    IdDanhBaChiTiet = dbd.IdDanhBaChiTiet,
                                    IdTruongData = dbd.IdTruongData,
                                    Data = dbd.Data
                                })
                              .ToListAsync();

            }
            return result;
        }
        private string ProcessTextContent(string textTemplate, List<DanhBaDataInfoDto> userData, Dictionary<int, string> truongDataMapping, bool IsAccented)
        {

            var processedText = textTemplate;
            var dataDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var data in userData)
            {
                if (truongDataMapping.TryGetValue(data.IdTruongData, out string tenTruong))
                {
                    var dataValue = data.Data ?? string.Empty;
                    var finalDataValue = IsAccented ? dataValue : RemoveAccents(dataValue);

                    dataDict[tenTruong] = finalDataValue;
                }
                else
                {
                }
            }
            var placeholderPattern = @"\[([^\]]+)\]";
            var matches = Regex.Matches(processedText, placeholderPattern);

            foreach (Match match in matches)
            {
                var placeholder = match.Value;
                var fieldName = match.Groups[1].Value;

                var dataValue = dataDict.FirstOrDefault(x =>
                    string.Equals(x.Key, fieldName, StringComparison.OrdinalIgnoreCase)).Value;

                if (dataValue != null)
                {
                    processedText = processedText.Replace(placeholder, dataValue);
                }
                else
                {
                    processedText = processedText.Replace(placeholder, string.Empty);
                }
            }
            var finalResult = IsAccented ? processedText : RemoveAccents(processedText);
            return finalResult;
        }

        private async Task<Dictionary<int, string>> GetTruongDataMapping(int idDanhBa)
        {
            var truongDataList = await _smDbContext.DanhBaTruongDatas
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.TenTruong })
                .ToListAsync();

            return truongDataList.ToDictionary(x => x.Id, x => x.TenTruong);
        }

        private string RemoveAccents(string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (Regex.IsMatch(text, @"^\d+$"))
            {
                return text;
            }
            var normalizedString = text.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            if (!Regex.IsMatch(result, @"^\d+$"))
            {
                result = result.Replace("đ", "d").Replace("Đ", "D");
            }
            else
            {
            }
            return result;
        }
        private int CalculateSmsCount(int length, bool isAccented)
        {
            if (isAccented)
            {
                if (length <= 70) return 1;
                else if (length <= 134) return 2;
                else if (length <= 201) return 3;
                else if (length <= 268) return 4;
                else if (length <= 335) return 5;
                else return (int)Math.Ceiling((double)length / 67);
            }
            else
            {
                if (length <= 160) return 1;
                else if (length <= 306) return 2;
                else if (length <= 459) return 3;
                else return (int)Math.Ceiling((double)length / 153);
            }
        }
        private string GetNetworkByPhoneNumber(string formattedNumber)
        {
            var prefix = formattedNumber.Length >= 4 ? formattedNumber.Substring(2, 2) : "";

            var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
            var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
            var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
            var vietnamobile = new[] { "92", "56", "58", "52" };
            var gmobile = new[] { "99", "59" };

            if (viettelPrefixes.Contains(prefix)) return "Viettel";
            else if (mobifone.Contains(prefix)) return "Mobifone";
            else if (vinaphone.Contains(prefix)) return "Vinaphone";
            else if (vietnamobile.Contains(prefix)) return "Vietnamobile";
            else if (gmobile.Contains(prefix)) return "Gmobile";

            return "Unknown";
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            var cleanedNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            if (cleanedNumber.StartsWith("0"))
            {
                return "84" + cleanedNumber.Substring(1);
            }

            return cleanedNumber;
        }
        private async Task<string> GetBrandNameByChienDich(int idBrandName)
        {
            var brandName = await (from bn in _smDbContext.BrandName
                                   where bn.Id == idBrandName && !bn.Deleted
                                   select bn.TenBrandName)
                                 .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(brandName))
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }

            return brandName;
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
