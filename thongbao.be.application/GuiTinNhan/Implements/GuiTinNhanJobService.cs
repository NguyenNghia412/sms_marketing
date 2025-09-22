using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.GuiTinNhan.Implements
{
    public class GuiTinNhanJobService : IGuiTinNhanJobService
    {
        private readonly SmDbContext _dbContext;
        private readonly ILogger<GuiTinNhanJobService> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMapper _mapper;
        private const int BATCH_SIZE = 500;

        public GuiTinNhanJobService(
            SmDbContext smDbContext,
            ILogger<GuiTinNhanJobService> logger,
            IBackgroundJobClient backgroundJobClient,
            IMapper mapper)
        {
            _dbContext = smDbContext;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _mapper = mapper;
        }

        public async Task<List<object>> StartGuiTinNhanJob(int idChienDich, int idDanhBa, string textNoiDung)
        {
            await ValidateInput(idChienDich, idDanhBa, textNoiDung);
            var result = await ProcessGuiTinNhanJob(idChienDich, idDanhBa, textNoiDung);
            return result;
        }

        private async Task<List<object>> ProcessGuiTinNhanJob(int idChienDich, int idDanhBa, string textNoiDung)
        {
            var brandName = await GetBrandNameByChienDich(idChienDich);

            var totalRecords = await _dbContext.DanhBaChiTiets
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .CountAsync();

            var allSmsMessages = new List<object>();
            var totalBatches = (int)Math.Ceiling((double)totalRecords / BATCH_SIZE);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchMessages = await ProcessBatch(idChienDich, idDanhBa, textNoiDung, batchIndex, brandName);
                allSmsMessages.AddRange(batchMessages);
            }

            return allSmsMessages;
        }

        private async Task<List<object>> ProcessBatch(int idChienDich, int idDanhBa, string textNoiDung, int batchIndex, string brandName)
        {
            var danhBaChiTiets = await _dbContext.DanhBaChiTiets
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

                var personalizedText = ProcessTextContent(textNoiDung, userData, truongDataMapping, danhBaChiTiet.MaSoNguoiDung);

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

        private async Task ValidateInput(int idChienDich, int idDanhBa, string textNoiDung)
        {
            if (string.IsNullOrWhiteSpace(textNoiDung))
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest);
            }

            var chienDichExists = await _dbContext.ChienDiches
                .AnyAsync(x => x.Id == idChienDich && !x.Deleted);

            if (!chienDichExists)
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            var danhBaExists = await _dbContext.DanhBas
                .AnyAsync(x => x.Id == idDanhBa && !x.Deleted);

            if (!danhBaExists)
            {
                throw new UserFriendlyException(ErrorCodes.DanhBaErrorNotFound);
            }
        }

        private async Task<string> GetBrandNameByChienDich(int idChienDich)
        {
            var brandName = await (from cd in _dbContext.ChienDiches
                                   join bn in _dbContext.BrandName on cd.IdBrandName equals bn.Id
                                   where cd.Id == idChienDich && !cd.Deleted && !bn.Deleted
                                   select bn.TenBrandName)
                                 .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(brandName))
            {
                throw new UserFriendlyException(ErrorCodes.ChienDichErrorNotFound);
            }

            return brandName;
        }

        private async Task<Dictionary<int, string>> GetTruongDataMapping(int idDanhBa)
        {
            var truongDataList = await _dbContext.DanhBaTruongDatas
                .Where(x => x.IdDanhBa == idDanhBa && !x.Deleted)
                .Select(x => new { x.Id, x.TenTruong })
                .ToListAsync();

            return truongDataList.ToDictionary(x => x.Id, x => x.TenTruong);
        }

        private async Task<List<DanhBaDataInfoDto>> GetDanhBaDataForBatch(List<int> danhBaChiTietIds, int idChienDich)
        {
            var allDataCount = await _dbContext.DanhBaDatas
                .Where(x => danhBaChiTietIds.Contains(x.IdDanhBaChiTiet) && !x.Deleted)
                .CountAsync();

            var result = await (from dbd in _dbContext.DanhBaDatas
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
                result = await (from dbd in _dbContext.DanhBaDatas
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

        private string ProcessTextContent(string textTemplate, List<DanhBaDataInfoDto> userData, Dictionary<int, string> truongDataMapping, string maSoNguoiDung)
        {
            var processedText = textTemplate;
            var dataDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var data in userData)
            {
                if (truongDataMapping.TryGetValue(data.IdTruongData, out string tenTruong))
                {
                    dataDict[tenTruong] = data.Data ?? string.Empty;
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

            return processedText;
        }
    }
}