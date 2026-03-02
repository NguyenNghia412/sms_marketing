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
using thongbao.be.shared.Constants.ChienDich;
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
        private const int BATCH_SIZE = 400;
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

        public async Task ProcessGuiTinNhanBackground(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung, string currentUserId, bool isSuperAdmin)
        {
            _logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - START - idChienDich: {idChienDich}, idDanhBa: {idDanhBa}");

            bool hasProcessedSuccessfully = false;

            try
            {
                //_logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - DELAYING 1 minute - idChienDich: {idChienDich}");
                //await Task.Delay(TimeSpan.FromSeconds(60));
                var chienDichTrangThai = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);
                if (chienDichTrangThai.TrangThai == ChienDichConstants.Huy)
                {
                    return;
                }
                else
                {
                    _logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - DEBUG - idChienDich: {idChienDich}");
                    var result = await ProcessGuiTinNhanJob(idChienDich, idDanhBa, danhSachSoDienThoai, idBrandName, IsFlashSms, IsAccented, noiDung, currentUserId, isSuperAdmin);

                    hasProcessedSuccessfully = true;

                    _logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - COMPLETED - idChienDich: {idChienDich}, Total SMS: {result.Count}");

                    
                    if (!isSuperAdmin)
                    {
                        var chiPhiThucTe = await _smDbContext.ChienDichLogTrangThaiGuis
                            .Where(x => x.IdChienDich == idChienDich && !x.Deleted)
                            .SumAsync(x => x.TongChiPhi);

                        var userCredit = await _smDbContext.UserCredits
                            .Where(x => x.UserId == currentUserId && !x.Deleted)
                            .OrderByDescending(x => x.CreatedDate)
                            .FirstOrDefaultAsync();

                        if (userCredit != null)
                        {
                            var creditDaSuDung = Convert.ToInt32(userCredit.CreditDaSuDung ?? "0");
                            var hanMucCredit = Convert.ToInt32(userCredit.HanMucCredit ?? "0");
                            var tongCreditDaSuDung = creditDaSuDung + chiPhiThucTe;

                            userCredit.CreditDaSuDung = tongCreditDaSuDung.ToString();
                            userCredit.CreditChuaSuDung = (hanMucCredit - tongCreditDaSuDung).ToString();
                            _smDbContext.UserCredits.Update(userCredit);
                            _logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - Updated CreditDaSuDung for user {currentUserId}, chiPhiThucTe: {chiPhiThucTe}");
                        }
                    }

                    var chienDich = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);
                    if (chienDich != null && chienDich.TrangThai != ChienDichConstants.Huy)
                    {
                        chienDich.TrangThai = ChienDichConstants.DaGui;
                        _smDbContext.ChienDiches.Update(chienDich);
                    }

                    await _smDbContext.SaveChangesAsync();
                    //_logger.LogInformation($"{nameof(ProcessGuiTinNhanBackground)} - Updated status to DaGui - idChienDich: {idChienDich}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ProcessGuiTinNhanBackground)} - ERROR - idChienDich: {idChienDich}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");

                if (!hasProcessedSuccessfully)
                {
                    try
                    {
                        var chienDich = await _smDbContext.ChienDiches.FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);
                        if (chienDich != null)
                        {
                            chienDich.TrangThai = ChienDichConstants.Nhap;
                            _smDbContext.ChienDiches.Update(chienDich);
                            await _smDbContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError($"{nameof(ProcessGuiTinNhanBackground)} - ROLLBACK ERROR - idChienDich: {idChienDich}, Error: {rollbackEx.Message}");
                    }

                    throw;
                }
            }
        }
        public async Task SendSmsLog(object smsResponse, int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool isAccented, string noiDung, string currentUserId, bool isSuperAdmin)
        {
            _logger.LogInformation($"{nameof(SendSmsLog)} - idChienDich: {idChienDich}, idDanhBa: {idDanhBa}");
            try
            {

                //var isSuperAdmin = IsSuperAdmin();
                //var currentUserId = getCurrentUserId();
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
                    var danhBaCount = danhBaSmsList.Count;
                    var truongDataMapping = await GetTruongDataMapping(idDanhBa.Value);
                    var recordIds = danhBaSmsList.Select(x => x.Id).ToList();
                    var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

                    var networkCosts = new Dictionary<string, int>
                    {
                        ["Viettel"] = 420,
                        ["Mobifone"] = 420,
                        ["Vinaphone"] = 420,
                        ["Vietnamobile"] = 700,
                        ["Gmobile"] = 300
                    };

                    var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
                    var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
                    var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
                    var vietnamobile = new[] { "92", "56", "58", "52" };
                    var gmobile = new[] { "99", "59" };
                    int tongChiPhi = 0;

                    
                    var chienDichLog = new ChienDichLogTrangThaiGui
                    {
                        IdChienDich = idChienDich,
                        IdBrandName = idBrandName,
                        TongSoSms = danhBaCount,
                        SmsSendSuccess = 0,
                        SmsSendFailed = 0,
                        TrangThai = "Đang thực hiện lệnh gửi",
                        NoiDung = noiDung,
                        TongChiPhi = 0,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };
                    _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
                    await _smDbContext.SaveChangesAsync();
                    

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
                            SoLuongTinNhan = smsCount,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId
                        };

                        _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);

                        
                        await _smDbContext.SaveChangesAsync();
                        
                    }

                    
                    chienDichLog.SmsSendSuccess = smsSuccess;
                    chienDichLog.SmsSendFailed = smsFailed;
                    chienDichLog.TrangThai = trangThaiChienDich;
                    chienDichLog.TongChiPhi = tongChiPhi;
                    _smDbContext.ChienDichLogTrangThaiGuis.Update(chienDichLog);
                    

                    var chienDich = await _smDbContext.ChienDiches
                        .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                    if (chienDich != null)
                    {
                        chienDich.TrangThai = ChienDichConstants.DaGui;
                        _smDbContext.ChienDiches.Update(chienDich);
                    }

                    await _smDbContext.SaveChangesAsync();
                }
                //Mode: List số điện thoại
                else
                {
                    var networkCosts = new Dictionary<string, int>
                    {
                        ["Viettel"] = 420,
                        ["Mobifone"] = 420,
                        ["Vinaphone"] = 420,
                        ["Vietnamobile"] = 700,
                        ["Gmobile"] = 300
                    };
                    var listsmsCount = danhSachSoDienThoai?.Count ?? 0;
                    var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
                    var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
                    var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
                    var vietnamobile = new[] { "92", "56", "58", "52" };
                    var gmobile = new[] { "99", "59" };
                    int tongChiPhi = 0;

                    
                    var chienDichLog = new ChienDichLogTrangThaiGui
                    {
                        IdChienDich = idChienDich,
                        //IdDanhBa = idDanhBa,
                        IdBrandName = idBrandName,
                        TongSoSms = listsmsCount,
                        SmsSendSuccess = 0,
                        SmsSendFailed = 0,
                        TrangThai = "Đang thực hiện lệnh gửi",
                        NoiDung = noiDung,
                        TongChiPhi = 0,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };
                    _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
                    await _smDbContext.SaveChangesAsync();
                    

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
                            SoLuongTinNhan = smsCount,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId
                        };

                        _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);

                        
                        await _smDbContext.SaveChangesAsync();
                        
                    }

                    
                    chienDichLog.SmsSendSuccess = smsSuccess;
                    chienDichLog.SmsSendFailed = smsFailed;
                    chienDichLog.TrangThai = trangThaiChienDich;
                    chienDichLog.TongChiPhi = tongChiPhi;
                    _smDbContext.ChienDichLogTrangThaiGuis.Update(chienDichLog);
                    

                    if (smsSuccess > 0)
                    {
                        var chienDich = await _smDbContext.ChienDiches
                            .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                        if (chienDich != null)
                        {
                            chienDich.TrangThai = ChienDichConstants.DaGui;
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

        private async Task<List<object>> ProcessGuiTinNhanJob(int idChienDich, int? idDanhBa, List<ListSoDienThoaiDto> danhSachSoDienThoai, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung, string currentUserId, bool isSuperAdmin)
        {
            var brandName = await GetBrandNameByChienDich(idBrandName);
            var allSmsMessages = new List<object>();

            try
            {
                // Mode: Danh bạ
                if (idDanhBa.HasValue)
                {
                    var totalRecords = await _smDbContext.DanhBaSms
                        .Where(x => x.IdDanhBa == idDanhBa.Value && !x.Deleted)
                        .CountAsync();

                    if (IsFlashSms)
                    {
                        if (await IsChienDichCancelled(idChienDich)) return allSmsMessages;
                        var allMessages = await ProcessAllData(idChienDich, idDanhBa.Value, noiDung, brandName, IsAccented);

                        if (allMessages.Any())
                        {
                            try
                            {
                                var result = await _sendSmsService.SendSmsAsync(allMessages);
                                await SendSmsLog(result, idChienDich, idDanhBa, null, idBrandName, IsAccented, noiDung, currentUserId, isSuperAdmin);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"[ProcessGuiTinNhanJob ERROR] FlashSMS - idChienDich: {idChienDich}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                            }
                        }
                    }
                    else
                    {
                        var totalBatches = (int)Math.Ceiling((double)totalRecords / BATCH_SIZE);

                        int totalSuccessAll = 0;
                        int totalFailedAll = 0;
                        int totalCostAll = 0;

                        
                        var vietnamNow = GetVietnamTime();
                        var chienDichLog = new ChienDichLogTrangThaiGui
                        {
                            IdChienDich = idChienDich,
                            IdDanhBa = idDanhBa,
                            IdBrandName = idBrandName,
                            TongSoSms = 0,
                            SmsSendSuccess = 0,
                            SmsSendFailed = 0,
                            TrangThai = "Đang thực hiện lệnh gửi",
                            NoiDung = noiDung,
                            TongChiPhi = 0,
                            CreatedDate = vietnamNow,
                            CreatedBy = currentUserId
                        };
                        _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
                        await _smDbContext.SaveChangesAsync();
                        

                        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                        {
                            if (await IsChienDichCancelled(idChienDich))
                            {
                                _logger.LogInformation($"[ProcessGuiTinNhanJob] CANCELLED at batch {batchIndex} - idChienDich: {idChienDich}");
                                break;
                            }
                            try
                            {
                                var (batchMessages, batchSuccess, batchFailed, batchCost) = await ProcessBatch(idChienDich, idDanhBa.Value, noiDung, batchIndex, brandName, IsAccented, idBrandName, currentUserId, isSuperAdmin);
                                allSmsMessages.AddRange(batchMessages);

                                totalSuccessAll += batchSuccess;
                                totalFailedAll += batchFailed;
                                totalCostAll += batchCost;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"[ProcessGuiTinNhanJob ERROR] Batch {batchIndex} - idChienDich: {idChienDich}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                                continue;
                            }
                        }

                        if (!await IsChienDichCancelled(idChienDich) && (totalSuccessAll > 0 || totalFailedAll > 0))
                        {
                            
                            chienDichLog.TongSoSms = totalSuccessAll + totalFailedAll;
                            chienDichLog.SmsSendSuccess = totalSuccessAll;
                            chienDichLog.SmsSendFailed = totalFailedAll;
                            chienDichLog.TrangThai = totalSuccessAll > 0 ? "Success" : "Failed";
                            chienDichLog.TongChiPhi = totalCostAll;
                            _smDbContext.ChienDichLogTrangThaiGuis.Update(chienDichLog);
                            

                            if (totalSuccessAll > 0)
                            {
                                var chienDich = await _smDbContext.ChienDiches
                                    .FirstOrDefaultAsync(x => x.Id == idChienDich && (isSuperAdmin || x.CreatedBy == currentUserId) && !x.Deleted);

                                if (chienDich != null)
                                {
                                    chienDich.TrangThai = ChienDichConstants.DaGui;
                                    _smDbContext.ChienDiches.Update(chienDich);
                                }
                            }

                            await _smDbContext.SaveChangesAsync();
                        }
                    }
                }
                // Mode: List số điện thoại
                else
                {
                    if (await IsChienDichCancelled(idChienDich)) return allSmsMessages;
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

                    if (allSmsMessages.Any())
                    {
                        if (await IsChienDichCancelled(idChienDich)) return allSmsMessages;
                        try
                        {
                            var result = await _sendSmsService.SendSmsAsync(allSmsMessages);
                            await SendSmsLog(result, idChienDich, null, danhSachSoDienThoai, idBrandName, IsAccented, noiDung, currentUserId, isSuperAdmin);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"[ProcessGuiTinNhanJob ERROR] ListSoDienThoai - idChienDich: {idChienDich}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ProcessGuiTinNhanJob ERROR] idChienDich: {idChienDich}, idDanhBa: {idDanhBa}, Error: {ex.Message}, StackTrace: {ex.StackTrace}");
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

        private async Task<(List<object> messages, int success, int failed, int cost)> ProcessBatch(int idChienDich, int idDanhBa, string noiDung, int batchIndex, string brandName, bool IsAccented, int idBrandName, string currentUserId, bool isSuperAdmin)
        {
            var danhBaChiTiets = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Skip(batchIndex * BATCH_SIZE)
                .Take(BATCH_SIZE)
                .Select(x => new { x.Id, x.SoDienThoai })
                .ToListAsync();

            if (!danhBaChiTiets.Any())
                return (new List<object>(), 0, 0, 0);

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var danhBaChiTietIds = danhBaChiTiets.Select(x => x.Id).ToList();
            var danhBaData = await GetDanhBaDataForBatch(danhBaChiTietIds, idChienDich);

            //var currentUserId = getCurrentUserId();
            var vietnamNow = GetVietnamTime();

            var networkCosts = new Dictionary<string, int>
            {
                ["Viettel"] = 420,
                ["Mobifone"] = 420,
                ["Vinaphone"] = 420,
                ["Vietnamobile"] = 700,
                ["Gmobile"] = 300
            };

            var smsMessages = new List<object>();
            int totalSuccess = 0;
            int totalFailed = 0;
            int totalCost = 0;

            foreach (var danhBaChiTiet in danhBaChiTiets)
            {
                string personalizedText = "";
                int calculatedPrice = 0;
                int smsCount = 0;
                try
                {
                    var userData = danhBaData.Where(x => x.IdDanhBaChiTiet == danhBaChiTiet.Id).ToList();
                    personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, IsAccented);
                    var formattedPhoneNumber = FormatPhoneNumber(danhBaChiTiet.SoDienThoai);
                    var network = GetNetworkByPhoneNumber(formattedPhoneNumber);
                    var length = personalizedText.Length;
                    smsCount = CalculateSmsCount(length, IsAccented);

                    _logger.LogInformation($"[DEBUG] Phone: {danhBaChiTiet.SoDienThoai}, smsCount: {smsCount}");
                    if (networkCosts.ContainsKey(network))
                    {
                        calculatedPrice = networkCosts[network] * smsCount;
                    }

                    var smsObject = new
                    {
                        from = brandName,
                        to = formattedPhoneNumber,
                        text = personalizedText
                    };

                    var singleSms = new List<object> { smsObject };
                    var result = await _sendSmsService.SendSmsAsync(singleSms);

                    string trangThai = "Failed";
                    int code = -1;
                    string message = "Unknown Error";

                    if (result != null)
                    {
                        try
                        {
                            var responseJson = JObject.Parse(result.ToString());
                            var resultArray = responseJson["result"]?.ToArray();

                            if (resultArray != null && resultArray.Length > 0)
                            {
                                code = resultArray[0]["r"]?.Value<int>() ?? -1;
                                message = resultArray[0]["msg"]?.Value<string>() ?? "Unknown";

                                if (code == 0 && string.Equals(message, "Success", StringComparison.OrdinalIgnoreCase))
                                {
                                    trangThai = "Success";
                                    totalSuccess++;
                                    totalCost += calculatedPrice;
                                }
                                else
                                {
                                    totalFailed++;
                                }
                            }
                            else
                            {
                                totalFailed++;
                            }
                        }
                        catch (JsonReaderException ex)
                        {
                            _logger.LogError($"[ProcessBatch ERROR] Parse response failed for {formattedPhoneNumber}. Error: {ex.Message}");
                            trangThai = "Failed";
                            message = "Parse Response Error";
                            totalFailed++;
                        }
                    }
                    else
                    {
                        trangThai = "Failed";
                        message = "SendSmsService returned null";
                        totalFailed++;
                    }

                    var logChiTiet = new GuiTinNhanLogChiTiet
                    {
                        IdChienDich = idChienDich,
                        IdDanhBa = idDanhBa,
                        IdBrandName = idBrandName,
                        IdDanhBaSms = danhBaChiTiet.Id,
                        SoDienThoai = danhBaChiTiet.SoDienThoai,
                        NoiDungChiTiet = personalizedText,
                        Price = trangThai == "Success" ? calculatedPrice : 0,
                        Code = code,
                        Message = message,
                        TrangThai = trangThai,
                        SoLuongTinNhan = smsCount,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };

                    _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);

                    
                    await _smDbContext.SaveChangesAsync();
                    

                    smsMessages.Add(smsObject);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"[ProcessBatch ERROR] SMS to {danhBaChiTiet.SoDienThoai} failed. Error: {ex.Message}");

                    var logChiTiet = new GuiTinNhanLogChiTiet
                    {
                        IdChienDich = idChienDich,
                        IdDanhBa = idDanhBa,
                        IdBrandName = idBrandName,
                        IdDanhBaSms = danhBaChiTiet.Id,
                        SoDienThoai = danhBaChiTiet.SoDienThoai,
                        NoiDungChiTiet = personalizedText,
                        Price = 0,
                        Code = -1,
                        Message = $"Exception: {ex.Message}",
                        TrangThai = "Failed",
                        SoLuongTinNhan = 0,
                        CreatedDate = vietnamNow,
                        CreatedBy = currentUserId
                    };

                    _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);

                    
                    await _smDbContext.SaveChangesAsync();
                    

                    totalFailed++;

                    continue;
                }
            }

            return (smsMessages, totalSuccess, totalFailed, totalCost);
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
        private async Task<bool> IsChienDichCancelled(int idChienDich)
        {
            var chienDich = await _smDbContext.ChienDiches
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

            return chienDich == null || chienDich.TrangThai == ChienDichConstants.Huy;
        }
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}