using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class GuiTinNhanJobService :BaseService, IGuiTinNhanJobService
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

        public async Task<List<object>> StartGuiTinNhanJob(int idChienDich, int idDanhBa,bool IsFlashSms,int idBrandName,bool IsAccented, string textNoiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, textNoiDung);
            await SaveThongTinChienDich( idChienDich,  idDanhBa,  idBrandName,  IsFlashSms,  IsAccented,  textNoiDung);
            var result = await ProcessGuiTinNhanJob(idChienDich, idDanhBa, idBrandName, IsFlashSms, IsAccented, textNoiDung);
            return result;
        }
        public async Task SaveThongTinChienDich(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, bool IsAccented,string textNoiDung)
        {
            _logger.LogInformation($"{nameof(SaveThongTinChienDich)}");
            var vietnamNow = GetVietnamTime();
            await ValidateInput(idChienDich, idDanhBa, idBrandName, textNoiDung);

            var chienDichExisting = _smDbContext.ChienDiches.FirstOrDefault(x => x.Id == idChienDich && !x.Deleted)
                ?? throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            chienDichExisting.IdBrandName = idBrandName;
            chienDichExisting.IsFlashSms = IsFlashSms;
            chienDichExisting.NoiDung = textNoiDung;
            chienDichExisting.IsAccented = IsAccented;

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
        public async Task<object> GetPreviewMessage(int idChienDich, int idDanhBa, bool IsFlashSms, int idBrandName, bool IsAccented, string textNoiDung, int currentDanhBaSmsId)
        {
            await ValidateInput(idChienDich, idDanhBa, idBrandName, textNoiDung);

            var brandName = await GetBrandNameByChienDich(idBrandName);
            var truongDataMapping = await GetTruongDataMapping(idDanhBa);

            var currentRecord = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted && x.Id == currentDanhBaSmsId)
                .Select(x => new { x.Id, x.MaSoNguoiDung, x.SoDienThoai })
                .FirstOrDefaultAsync();

            if (currentRecord == null)
                return null;
            var userData = await GetDanhBaDataForBatch(new List<int> { currentRecord.Id }, idChienDich);
            var personalizedText = ProcessTextContent(textNoiDung, userData, truongDataMapping, currentRecord.MaSoNguoiDung, IsAccented);
            var formattedPhoneNumber = FormatPhoneNumber(currentRecord.SoDienThoai);
            return new
            {
                IdDanhBaSms = currentRecord.Id,
                SoDienThoai = formattedPhoneNumber,
                PersonalizedText = personalizedText
            };
        }

        private async Task<List<object>> ProcessGuiTinNhanJob(int idChienDich, int idDanhBa, int idBrandName, bool IsFlashSms, bool IsAccented, string textNoiDung)
        {
            var brandName = await GetBrandNameByChienDich(idBrandName);

            var totalRecords = await _smDbContext.DanhBaSms
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .CountAsync();

            var allSmsMessages = new List<object>();

            if (IsFlashSms)
            {
                var allMessages = await ProcessAllData(idChienDich, idDanhBa, textNoiDung, brandName, IsAccented);
                allSmsMessages.AddRange(allMessages);
            }
            else
            {
                var totalBatches = (int)Math.Ceiling((double)totalRecords / BATCH_SIZE);

                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var batchMessages = await ProcessBatch(idChienDich, idDanhBa, textNoiDung, batchIndex, brandName, IsAccented);
                    allSmsMessages.AddRange(batchMessages);
                }
            }

            return allSmsMessages;
        }

        private async Task<List<object>> ProcessAllData(int idChienDich, int idDanhBa, string textNoiDung, string brandName, bool IsAccented)
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

                var personalizedText = ProcessTextContent(textNoiDung, userData, truongDataMapping, danhBaChiTiet.MaSoNguoiDung, IsAccented);

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

        private async Task<List<object>> ProcessBatch(int idChienDich, int idDanhBa, string textNoiDung, int batchIndex, string brandName, bool IsAccented)
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

                var personalizedText = ProcessTextContent(textNoiDung, userData, truongDataMapping, danhBaChiTiet.MaSoNguoiDung, IsAccented);

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

        private async Task ValidateInput(int idChienDich, int idDanhBa,int idBrandName, string textNoiDung)
        {
            if (string.IsNullOrWhiteSpace(textNoiDung))
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
                .AnyAsync( x => x.Id == idBrandName && !x.Deleted);
            if (!brandNameExists){
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorBrandNameNotFound);
            }
          
        }

        private async Task<string> GetBrandNameByChienDich(int idBrandName)
        {
            var brandName = await (from bn in _smDbContext.BrandName
                                   where bn.Id == idBrandName  && !bn.Deleted
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