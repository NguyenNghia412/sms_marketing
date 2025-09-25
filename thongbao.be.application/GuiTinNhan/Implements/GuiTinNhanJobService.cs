using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class GuiTinNhanJobService : BaseService, IGuiTinNhanJobService
    {

        private readonly IBackgroundJobClient _backgroundJobClient;
        private const int BATCH_SIZE = 1000;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public GuiTinNhanJobService(
            SmDbContext smDbContext,
            ILogger<GuiTinNhanJobService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IBackgroundJobClient backgroundJobClient) : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<List<object>> StartGuiTinNhanJob(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, noiDung);
            await SaveThongTinChienDich(idChienDich, idDanhBa, idBrandName, IsFlashSms, IsAccented, noiDung);
            var result = await ProcessGuiTinNhanJob(idChienDich, idDanhBa, idBrandName, IsFlashSms, IsAccented, noiDung);
            return result;
        }
        public async Task SaveThongTinChienDich(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            _logger.LogInformation($"{nameof(SaveThongTinChienDich)}");
            var vietnamNow = GetVietnamTime();
            await ValidateInput(idChienDich, idDanhBa, idBrandName, noiDung);

            var chienDichExisting = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            chienDichExisting.IdBrandName = idBrandName;
            chienDichExisting.IsFlashSms = IsFlashSms;
            chienDichExisting.NoiDung = noiDung;
            chienDichExisting.IsAccented = IsAccented;
            chienDichExisting.TrangThai = false;

            _smDbContext.ChienDiches.Update(chienDichExisting);
            _smDbContext.SaveChanges();
            var chienDichDanhBa = await _smDbContext.ChienDichDanhBa.FirstOrDefaultAsync(x => x.IdChienDich == idChienDich && x.IdDanhBa == idDanhBa && !x.Deleted);
            if (chienDichDanhBa == null)
            {
                chienDichDanhBa = new domain.GuiTinNhan.ChienDichDanhBa
                {
                    IdChienDich = idChienDich,
                    IdDanhBa = idDanhBa,
                };
                _smDbContext.ChienDichDanhBa.Add(chienDichDanhBa);
            }

            _smDbContext.SaveChanges();

        }
        public async Task<object> GetPreviewMessage(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, bool IsAccented, string noiDung, int currentIndex)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, noiDung);
            var brandName = await GetBrandNameByChienDich(idBrandName);
            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var allRecords = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
                .ToListAsync();
            if (currentIndex < 1 || currentIndex > allRecords.Count)
                return null;
            var currentRecord = allRecords[currentIndex - 1];

            var userData = await GetDanhBaDataForBatch(new List<int> { currentRecord.Id }, idChienDich);
            var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, currentRecord.MaSoNguoiDung, IsAccented);
            var formattedPhoneNumber = FormatPhoneNumber(currentRecord.SoDienThoai);
            var length = personalizedText.Length;

            int smsCount;
            if (IsAccented)
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

            return new
            {
                IdDanhBaSms = currentRecord.Id,
                SoDienThoai = formattedPhoneNumber,
                PersonalizedText = personalizedText,
                SmsCount = smsCount
            };
        }
        public async Task<object> GetChiPhiDuTruChienDich(int idChienDich, int idDanhBa, int idBrandName, bool isFlashSms, bool isAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, noiDung);

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var allRecords = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
                .ToListAsync();

            var recordIds = allRecords.Select(x => x.Id).ToList();
            var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

            var networkCosts = new Dictionary<string, decimal>
            {
                ["Viettel"] = 420,
                ["Mobifone"] = 420,
                ["Vinaphone"] = 420,
                ["Vietnamobile"] = 700,
                ["Gmobile"] = 300
            };

            decimal totalCost = 0;

            foreach (var record in allRecords)
            {
                var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, record.MaSoNguoiDung, isAccented);

                var formattedNumber = FormatPhoneNumber(record.SoDienThoai);
                var prefix = formattedNumber.Length >= 4 ? formattedNumber.Substring(2, 2) : "";

                var viettelPrefixes = new[] { "96", "97", "98", "86", "32", "33", "34", "35", "36", "37", "38", "39" };
                var mobifone = new[] { "90", "93", "89", "70", "76", "77", "78", "79" };
                var vinaphone = new[] { "91", "94", "88", "81", "82", "83", "84", "85", "80" };
                var vietnamobile = new[] { "92", "56", "58", "52" };
                var gmobile = new[] { "99", "59" };

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

                if (networkCosts.ContainsKey(network))
                {
                    totalCost += networkCosts[network] * smsCount;
                }
            }

            return new { TotalCost = totalCost };
        }
        public async Task SendSmsLog(object smsResponse, int idChienDich, int idDanhBa, int idBrandName, bool isAccented, string noiDung)
        {
            _logger.LogInformation($"{nameof(SendSmsLog)} - idChienDich: {idChienDich}, idDanhBa: {idDanhBa}");

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

            var danhBaSmsList = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Select(x => new { x.Id, x.SoDienThoai })
                .ToListAsync();

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
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

            for (int i = 0; i < resultArray.Length && i < danhBaSmsList.Count; i++)
            {
                var resultItem = resultArray[i];
                var danhBaSms = danhBaSmsList[i];

                var userData = allUserData.Where(x => x.IdDanhBaChiTiet == danhBaSms.Id).ToList();
                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, "", isAccented);

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
                    Price = calculatedPrice,
                    Code = code,
                    Message = message,
                    TrangThai = trangThaiChiTiet,
                    CreatedDate = vietnamNow
                };

                _smDbContext.GuiTinNhanLogChiTiets.Add(logChiTiet);
            }
            var chienDichLog = new ChienDichLogTrangThaiGui
            {
                IdChienDich = idChienDich,
                IdDanhBa = idDanhBa,
                IdBrandName = idBrandName,
                SmsSendSuccess = smsSuccess,
                SmsSendFailed = smsFailed,
                TrangThai = trangThaiChienDich,
                TongChiPhi = tongChiPhi,
                CreatedDate = vietnamNow
            };

            _smDbContext.ChienDichLogTrangThaiGuis.Add(chienDichLog);
            if (smsSuccess > 0)
            {
                var chienDich = await _smDbContext.ChienDiches
                    .FirstOrDefaultAsync(x => x.Id == idChienDich && !x.Deleted);

                if (chienDich != null)
                {
                    chienDich.TrangThai = true;
                    _smDbContext.ChienDiches.Update(chienDich);
                }
            }

            await _smDbContext.SaveChangesAsync();
        }
        public async Task<object> GetSoLuongNguoiNhanVaTinNhan(int idChienDich, int idDanhBa, int idBrandName, bool isFlashSms, bool isAccented, string noiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, noiDung);

            var truongDataMapping = await GetTruongDataMapping(idDanhBa);
            var allRecords = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
                .ToListAsync();

            var recordIds = allRecords.Select(x => x.Id).ToList();
            var allUserData = await GetDanhBaDataForBatch(recordIds, idChienDich);

            int soLuongNguoiNhan = allRecords.Count;
            int tongSoLuongTinNhan = 0;

            foreach (var record in allRecords)
            {
                var userData = allUserData.Where(x => x.IdDanhBaChiTiet == record.Id).ToList();
                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, record.MaSoNguoiDung, isAccented);

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

                tongSoLuongTinNhan += smsCount;
            }

            return new
            {
                SoLuongNguoiNhan = soLuongNguoiNhan,
                TongSoLuongTinNhan = tongSoLuongTinNhan
            };
        }
        private async Task<List<object>> ProcessGuiTinNhanJob(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, bool IsAccented, string noiDung)
        {
            var brandName = await GetBrandNameByChienDich(idBrandName);

            var totalRecords = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .CountAsync();

            var allSmsMessages = new List<object>();

            if (IsFlashSms)
            {
                var allMessages = await ProcessAllData(idChienDich, idDanhBa, noiDung, brandName, IsAccented);
                allSmsMessages.AddRange(allMessages);
            }
            else
            {
                var totalBatches = (int)Math.Ceiling((double)totalRecords / BATCH_SIZE);

                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var batchMessages = await ProcessBatch(idChienDich, idDanhBa, noiDung, batchIndex, brandName, IsAccented);
                    allSmsMessages.AddRange(batchMessages);
                }
            }

            return allSmsMessages;
        }

        private async Task<List<object>> ProcessAllData(int idChienDich, int idDanhBa, string noiDung, string brandName, bool IsAccented)
        {
            var danhBaChiTiets = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
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

                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, danhBaChiTiet.MaSoNguoiDung, IsAccented);

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

        private async Task<List<object>> ProcessBatch(int idChienDich, int idDanhBa, string noiDung, int batchIndex, string brandName, bool IsAccented)
        {
            var danhBaChiTiets = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .OrderBy(x => x.Id)
                .Skip(batchIndex * BATCH_SIZE)
                .Take(BATCH_SIZE)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
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

                var personalizedText = ProcessTextContent(noiDung, userData, truongDataMapping, danhBaChiTiet.MaSoNguoiDung, IsAccented);

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

        private async Task ValidateInput(int idChienDich, int idDanhBa, int idBrandName, string noiDung)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest);
            }

            var chienDichExists = await _smDbContext.ChienDiches
                .AnyAsync(x => x.Id == idChienDich && !x.Deleted);

            if (!chienDichExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            var danhBaExists = await _smDbContext.DanhBas
                .AnyAsync(x => x.Id == idDanhBa && !x.Deleted);

            if (!danhBaExists)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
            }
            var brandNameExists = await _smDbContext.BrandName
                .AnyAsync(x => x.Id == idBrandName && !x.Deleted);
            if (!brandNameExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }

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

        private string ProcessTextContent(string textTemplate, List<DanhBaDataInfoDto> userData, Dictionary<int, string> truongDataMapping, string maSoNguoiDung, bool IsAccented)
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