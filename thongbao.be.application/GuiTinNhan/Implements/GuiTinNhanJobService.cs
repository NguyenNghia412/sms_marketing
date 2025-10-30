using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.VariantTypes;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Targets.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using thongbao.be.application.Base;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.infrastructure.data;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class GuiTinNhanJobService : BaseService, IGuiTinNhanJobService
    {

        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IProfileService _profileService;
        private readonly ISendSmsService _sendSmsService;
        private readonly UserManager<AppUser> _userManager;
        private const int BATCH_SIZE = 500;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public GuiTinNhanJobService(
            SmDbContext smDbContext,
            ILogger<GuiTinNhanJobService> logger,
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

        public async Task<List<object>> StartGuiTinNhanJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, noiDung);
            await ValidateChienDichChuaGui(idChienDich);
            await ValidateSoLuongTinNhan(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
            if (idDanhBa.HasValue)
            {
                await SaveThongTinChienDich(idChienDich, idDanhBa.Value,  danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
            }
            var estimatedAmount = await GetChiPhiDuTruChienDich(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
            var profileInfo = await _profileService.GetProfileStringeeInfor();
            var amount = Convert.ToInt32(profileInfo?.Data?.Amount ?? 0); 
            if (estimatedAmount > amount)
            {
                await SendWarningToAdmin(idChienDich, estimatedAmount, amount);
                throw new UserFriendlyException(ErrorCodes.GuiTinNhanErrorNotEnoughBalance);
            }
            var result = await ProcessGuiTinNhanJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung);
            return result;
        }
        private async Task SendWarningToAdmin(int idChienDich,int estimatedAmount,int amount)
        {
            _logger.LogInformation($"{nameof(SendWarningToAdmin)}");
            var admins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var chienDich =  _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted);
            var brandName = _smDbContext.BrandName.FirstOrDefault(x => x.TenBrandName == "HUCE" && !x.Deleted);
            var idBrandName = brandName.Id;
            var IsAccented = true;
            var amountNeeded = estimatedAmount - amount;
            var noiDung = $"Chiến dịch \"{chienDich.TenChienDich}\" yêu cầu vượt mức chi phí hiện có từ Stringee. Xin vui lòng chuyển khoản thêm vào tài khoản Stringee số tiền là {amountNeeded:N0}VND để khách hàng thực hiện tiếp dịch vụ. Xin cảm ơn!";
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
            if(smsMessages.Any())
            {
                await _sendSmsService.SendSmsAsync(smsMessages);
            }
        }
        public async Task SaveThongTinChienDich(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int? idBrandName, bool IsFlashSms, bool IsAccented, string? noiDung)
        {
            _logger.LogInformation($"{nameof(SaveThongTinChienDich)}");
            var vietnamNow = GetVietnamTime();

            await ValidateInput(idChienDich, idDanhBa, null, idBrandName, noiDung);
            await ValidateChienDichChuaGui(idChienDich);

            var chienDichExisting = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);

            if (idBrandName.HasValue)
            {
                chienDichExisting.IdBrandName = idBrandName.Value;
            }

            chienDichExisting.IsFlashSms = IsFlashSms;
            chienDichExisting.NoiDung = noiDung;
            chienDichExisting.IsAccented = IsAccented;
            chienDichExisting.TrangThai = false;

      
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
            else if(danhSachSoDienThoai != null && danhSachSoDienThoai.Count > 0)
            {
                var countSoDienThoai = danhSachSoDienThoai.Count;
                chienDichExisting.SoLuongThueBao = countSoDienThoai;
            }
            _smDbContext.ChienDiches.Update(chienDichExisting);
            await _smDbContext.SaveChangesAsync();
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
        public async Task SendSmsLog(object smsResponse, int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isAccented, string noiDung)
        {
            _logger.LogInformation($"{nameof(SendSmsLog)} - idChienDich: {idChienDich}, idDanhBa: {idDanhBa}");
            try
            {

                var isSuperAdmin = IsSuperAdmin();
                var currentUserId = getCurrentUserId();
                var responseJson = JObject.Parse(smsResponse.ToString());
                var smsSent = responseJson["smsSent"].Value<int>();
                var resultArray = responseJson["result"].ToArray();

                var vietnamNow = GetVietnamTime();
                int smsSuccess = 0;
                int smsFailed = 0;

                foreach (var item in resultArray)
                {
                    var code = item["r"].Value<int>();
                    var message = item["msg"].Value<string>();

                    if (code == 0 && string.Equals(message, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        smsSuccess++;
                    }
                    else
                    {
                        smsFailed++;
                    }
                }
                string trangThaiChienDich = resultArray.Any() ? "Success" : "Failed";
                //Mode: Danh bạ
                if (idDanhBa.HasValue)
                {
                    var danhBaSmsList = await _smDbContext.DanhBaSms
                        .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.SoDienThoai })
                        .ToListAsync();

                    var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                    var recordIds = danhBaSmsList.Select(x => x.Id).ToList();
                    var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                    var networkCosts = new Dictionary<string, int>
                    {
                        ["Viettel"] = 800,
                        ["Mobifone"] = 800,
                        ["Vinaphone"] = 800,
                        ["Vietnamobile"] = 800,
                        ["Gmobile"] = 800
                    };

                    var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
                    var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
                    var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
                    var vietnamobile = new[] { "92", "56", "58", "52" };
                    var gmobile = new[] { "99", "59" };
                    int tongChiPhi = 0;

                    for (int i = 0; i < resultArray.Length && i < danhBaSmsList.Count; i++)
                    {
                        var resultItem = resultArray[i];
                        var danhBaSms = danhBaSmsList[i];

                        var userData = allUserData.Where(x => x.IdDanhBaChiTiet == danhBaSms.Id).ToList();
                        var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, isAccented);

                        var formattedNumber = FormatPhoneNumber(danhBaSms.SoDienThoai);
                        var prefix = formattedNumber.Length >= 4 ? formattedNumber.Substring(2, 2) : "";

                        string network = "Unknown";
                        if (viettelPrefixes.Contains(prefix)) network = "Viettel";
                        else if (mobifone.Contains(prefix)) network = "Mobifone";
                        else if (vinaphone.Contains(prefix)) network = "Vinaphone";
                        else if (vietnamobile.Contains(prefix)) network = "Vietnamobile";
                        else if (gmobile.Contains(prefix)) network = "Gmobile";

                        var length = personalizedText.Length;
                        int smsCount;

                        if (isAccented)
                        {
                            if (length <= 70) smsCount = 1;
                            else if (length <= 134) smsCount = 2;
                            else if (length <= 201) smsCount = 3;
                            else if (length <= 268) smsCount = 4;
                            else if (length <= 335) smsCount = 5;
                            else smsCount = (int)Math.Ceiling((double)length / 67);
                        }
                        else
                        {
                            if (length <= 160) smsCount = 1;
                            else if (length <= 306) smsCount = 2;
                            else if (length <= 459) smsCount = 3;
                            else smsCount = (int)Math.Ceiling((double)length / 153);
                        }

                        int calculatedPrice = 0;
                        if (networkCosts.ContainsKey(network))
                        {
                            calculatedPrice = networkCosts[network] * smsCount;
                        }
                        var code = resultItem["r"].Value<int>();
                        var message = resultItem["msg"].Value<string>();
                        string trangThaiChiTiet = (code == 0 && string.Equals(message, "Success", StringComparison.OrdinalIgnoreCase)) ? "Success" : "Failed";
                        if (trangThaiChiTiet == "Success")
                        {
                            tongChiPhi += calculatedPrice;
                        }

                        var logChiTiet = new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = idChienDich,
                            IdDanhBa = idDanhBa,
                            IdBrandName = idBrandName,
                            IdDanhBaSms = danhBaSms.Id,
                            SoDienThoai = danhBaSms.SoDienThoai,
                            NoiDungChiTiet = personalizedText,
                            Price = calculatedPrice,
                            Code = code,
                            Message = message,
                            TrangThai = trangThaiChiTiet,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId
                        };

                        _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);
                    }
                    var chienDichLog = new ChienDichLogTrangThaiGui
                    {
                        IdChienDich = idChienDich,
                        IdDanhBa = idDanhBa,
                        IdBrandName = idBrandName,
                        TongSoSms = smsSent,
                        SmsSendSuccess = smsSuccess,
                        SmsSendFailed = smsFailed,
                        TrangThai = trangThaiChienDich,
                        NoiDung = noiDung,
                        TongChiPhi = tongChiPhi,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };

                    _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
                    if (smsSuccess > 0)
                    {
                        var chienDich = await _smDbContext.ChienDiches
                            .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                        if (chienDich != null)
                        {
                            chienDich.TrangThai = true;
                            _smDbContext.ChienDiches.Update(chienDich);
                        }
                    }

                    await _smDbContext.SaveChangesAsync();
                }
                //Mode: List số điện thoại
                else
                {
                    var networkCosts = new Dictionary<string, int>
                    {
                        ["Viettel"] = 800,
                        ["Mobifone"] = 800,
                        ["Vinaphone"] = 800,
                        ["Vietnamobile"] = 800,
                        ["Gmobile"] = 800
                    };

                    var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
                    var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
                    var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
                    var vietnamobile = new[] { "92", "56", "58", "52" };
                    var gmobile = new[] { "99", "59" };
                    int tongChiPhi = 0;
                    for (int i = 0; i < resultArray.Length && i < danhSachSoDienThoai.Count; i++)
                    {
                        var resultItem = resultArray[i];
                        var SoDienThoai = danhSachSoDienThoai[i];



                        var personalizedText = ProcessTextContentForListSoDienThoai(noiDung, isAccented);

                        var formattedNumber = FormatPhoneNumber(SoDienThoai.SoDienThoai ?? "");
                        var prefix = formattedNumber.Length >= 4 ? formattedNumber.Substring(2, 2) : "";

                        string network = "Unknown";
                        if (viettelPrefixes.Contains(prefix)) network = "Viettel";
                        else if (mobifone.Contains(prefix)) network = "Mobifone";
                        else if (vinaphone.Contains(prefix)) network = "Vinaphone";
                        else if (vietnamobile.Contains(prefix)) network = "Vietnamobile";
                        else if (gmobile.Contains(prefix)) network = "Gmobile";

                        var length = personalizedText.Length;
                        int smsCount;

                        if (isAccented)
                        {
                            if (length <= 70) smsCount = 1;
                            else if (length <= 134) smsCount = 2;
                            else if (length <= 201) smsCount = 3;
                            else if (length <= 268) smsCount = 4;
                            else if (length <= 335) smsCount = 5;
                            else smsCount = (int)Math.Ceiling((double)length / 67);
                        }
                        else
                        {
                            if (length <= 160) smsCount = 1;
                            else if (length <= 306) smsCount = 2;
                            else if (length <= 459) smsCount = 3;
                            else smsCount = (int)Math.Ceiling((double)length / 153);
                        }

                        int calculatedPrice = 0;
                        if (networkCosts.ContainsKey(network))
                        {
                            calculatedPrice = networkCosts[network] * smsCount;
                        }
                        var code = resultItem["r"].Value<int>();
                        var message = resultItem["msg"].Value<string>();
                        string trangThaiChiTiet = (code == 0 && string.Equals(message, "Success", StringComparison.OrdinalIgnoreCase)) ? "Success" : "Failed";
                        if (trangThaiChiTiet == "Success")
                        {
                            tongChiPhi += calculatedPrice;
                        }

                        var logChiTiet = new GuiTinNhanLogChiTiet
                        {
                            IdChienDich = idChienDich,
                            IdDanhBa = idDanhBa,
                            IdBrandName = idBrandName,
                            //IdDanhBaSms = danhBaSms.Id,
                            SoDienThoai = SoDienThoai.SoDienThoai ?? "",
                            NoiDungChiTiet = personalizedText,
                            Price = calculatedPrice,
                            Code = code,
                            Message = message,
                            TrangThai = trangThaiChiTiet,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId
                        };

                        _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);
                    }
                    var chienDichLog = new ChienDichLogTrangThaiGui
                    {
                        IdChienDich = idChienDich,
                        //IdDanhBa = idDanhBa,
                        IdBrandName = idBrandName,
                        TongSoSms = smsSent,
                        SmsSendSuccess = smsSuccess,
                        SmsSendFailed = smsFailed,
                        TrangThai = trangThaiChienDich,
                        NoiDung = noiDung,
                        TongChiPhi = tongChiPhi,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };

                    _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
                    if (smsSuccess > 0)
                    {
                        var chienDich = await _smDbContext.ChienDiches
                            .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                        if (chienDich != null)
                        {
                            chienDich.TrangThai = true;
                            _smDbContext.ChienDiches.Update(chienDich);
                        }
                    }

                    await _smDbContext.SaveChangesAsync();
                }

            }
            catch (JsonReaderException ex)
            {
                _logger.LogError($"[SendSmsLog ERROR] JsonReaderException - Cannot parse response. Error: {ex.Message}, Response: {smsResponse}");
                //throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"[SendSmsLog ERROR] Exception - Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                //throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
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
        private async Task ValidateChienDichChuaGui(int idChienDich)
        {
            var chienDich = await _smDbContext.ChienDiches
                .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

            if (chienDich == null)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            if (chienDich.TrangThai == true)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorTrangThaiTrue);
            }
        }
        private async Task<List<object>> ProcessGuiTinNhanJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            var brandName = await GetBrandNameByChienDich(idBrandName);
            var allSmsMessages = new List<object>();

            // Mode: Danh bạ
            if (idDanhBa.HasValue)
            {
                var totalRecords = await _smDbContext.DanhBaSms
                    .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                    .CountAsync();

                if (IsFlashSms)
                {
                    var allMessages = await ProcessAllData(idChienDich, idDanhBa.Value, noiDung, brandName, IsAccented);
                    allSmsMessages.AddRange(allMessages);
                }
                else
                {
                    var totalBatches = (int)Math.Ceiling((double)totalRecords / BATCH_SIZE);

                    for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                    {
                        var batchMessages = await ProcessBatch(idChienDich, idDanhBa.Value, noiDung, batchIndex, brandName, IsAccented);
                        allSmsMessages.AddRange(batchMessages);
                    }
                }
            }
            // Mode: List số điện thoại
            else
            {
                var personalizedText = IsAccented ? noiDung : RemoveAccents(noiDung);

                foreach (var item in danhSachSoDienThoai)
                {
                    var formattedPhoneNumber = FormatPhoneNumber(item.SoDienThoai);

                    var smsObject = new
                    {
                        from = brandName,
                        to = formattedPhoneNumber,
                        text = personalizedText
                    };

                    allSmsMessages.Add(smsObject);
                }
            }

            return allSmsMessages;
        }

        private async Task<List<object>> ProcessAllData(int idChienDich, int idDanhBa, string noiDung, string brandName, bool IsAccented)
        {
            var danhBaChiTiets = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Select(x => new { x.Id, x.SoDienThoai })
                .ToListAsync();

            if (!danhBaChiTiets.Any())
                return new List<object>();

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var danhBaChiTietIds = danhBaChiTiets.Select(x => x.Id).ToList();
            var danhBaData = await GetDanhBaDataForBatch(danhBaChiTietIds, idChienDich);

            var smsMessages = new List<object>();

            foreach (var danhBaChiTiet in danhBaChiTiets)
            {
                var userData = danhBaData
                    .Where(x => x.IdDanhBaChiTiet == danhBaChiTiet.Id)
                    .ToList();

                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);

                var formattedPhoneNumber = FormatPhoneNumber(danhBaChiTiet.SoDienThoai);

                var smsObject = new
                {
                    from = brandName,
                    to = formattedPhoneNumber,
                    text = personalizedText
                };

                smsMessages.Add(smsObject);
            }

            return smsMessages;
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
        private async Task<List<object>> ProcessBatch(int idChienDich, int idDanhBa, string noiDung, int batchIndex, string brandName, bool IsAccented)
        {
            var danhBaChiTiets = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Skip(batchIndex * BATCH_SIZE)
                .Take(BATCH_SIZE)
                .Select(x => new { x.Id, x.SoDienThoai })
                .ToListAsync();

            if (!danhBaChiTiets.Any())
                return new List<object>();

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var danhBaChiTietIds = danhBaChiTiets.Select(x => x.Id).ToList();
            var danhBaData = await GetDanhBaDataForBatch(danhBaChiTietIds, idChienDich);

            var smsMessages = new List<object>();

            foreach (var danhBaChiTiet in danhBaChiTiets)
            {
                var userData = danhBaData
                    .Where(x => x.IdDanhBaChiTiet == danhBaChiTiet.Id)
                    .ToList();

                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);

                var formattedPhoneNumber = FormatPhoneNumber(danhBaChiTiet.SoDienThoai);

                var smsObject = new
                {
                    from = brandName,
                    to = formattedPhoneNumber,
                    text = personalizedText
                };

                smsMessages.Add(smsObject);
            }

            return smsMessages;
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

        private async Task<Dictionary<int, string>> GetTruongDataMapping(int idDanhBa)
        {
            var truongDataList = await _smDbContext.DanhBaTruongDatas
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.TenTruong })
                .ToListAsync();

            return truongDataList.ToDictionary(x => x.Id, x => x.TenTruong);
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

        private string ProcessTextContentForListSoDienThoai(string textTemplate, bool IsAccented)
        {

            var processedText = textTemplate;
            var dataDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

           
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

        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}