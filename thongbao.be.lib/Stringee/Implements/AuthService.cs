using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using thongbao.be.infrastructure.data;
using thongbao.be.lib.CdsConnect.Dtos.SvValidate;
using thongbao.be.lib.Stringee.Dtos.Base;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.lib.Stringee.Implements
{
    public class ProfileService : BaseService, IProfileService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _exchangeApiUrl;

        public ProfileService(
            SmDbContext smDbContext,
            ILogger<BaseService> logger,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IAuthService authService,
            HttpClient httpClient) : base(smDbContext, logger, httpContextAccessor)
        {
            _configuration = configuration;
            _authService = authService;
            _httpClient = httpClient;
            _baseUrl = _configuration["Stringee:ProfileBaseUrl"] ?? "";
            _exchangeApiUrl = _configuration["ExchangeApi:Url"] ?? "";
        }

        public async Task<BaseResponseProfile?> GetProfileStringeeInfor()
        {
            var isSuperAdmin = IsSuperAdmin();
            var jwtToken = await _authService.GenerateAccountJwtTokenAsync();

            _logger.LogInformation($"[GetProfileStringeeInfor] START - IsSuperAdmin={isSuperAdmin}, URL={_baseUrl}");
            _logger.LogInformation($"[GetProfileStringeeInfor] Token Generated: {jwtToken}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-STRINGEE-AUTH", jwtToken);

            _logger.LogInformation($"[GetProfileStringeeInfor] Sending request to Stringee API...");

            var response = await httpClient.GetAsync(_baseUrl);

            _logger.LogInformation($"[GetProfileStringeeInfor] Response received: StatusCode={response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"[GetProfileStringeeInfor] FAILED - StatusCode={response.StatusCode}, ErrorResponse={errorContent}");
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"[GetProfileStringeeInfor] Success Response: {responseContent}");

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var account = jsonResponse.GetProperty("account");
            var amountUsd = decimal.Parse(account.GetProperty("amount").GetString() ?? "0",
                           CultureInfo.InvariantCulture);

            if (isSuperAdmin)
            {
                var vndRate = await GetExchangeRate();
                var amount = amountUsd * Convert.ToDecimal(vndRate.ExchangeRate);

                _logger.LogInformation($"[GetProfileStringeeInfor] Calculated Amount: USD={amountUsd}, VND={amount}, Rate={vndRate.ExchangeRate}");

                return new BaseResponseProfile
                {
                    Code = jsonResponse.GetProperty("r").GetInt32(),
                    Data = new Account
                    {
                        Id = account.GetProperty("id").GetInt32(),
                        FirstName = account.GetProperty("firstname").GetString() ?? "",
                        LastName = account.GetProperty("lastname").GetString() ?? "",
                        Email = account.GetProperty("email").GetString() ?? "",
                        PhoneNumber = account.GetProperty("phone_number").GetString() ?? "",
                        CountryNumber = account.GetProperty("country_number").GetString() ?? "",
                        Amount = amount,
                    }
                };
            }
            else
            {
                _logger.LogWarning($"[GetProfileStringeeInfor] User is not SuperAdmin, returning null");
                return null;
            }
        }

        public async Task<ResponseGetExchangeApiDto?> GetExchangeRate()
        {
            var isSuperAdmin = IsSuperAdmin();

            _logger.LogInformation($"[GetExchangeRate] START - IsSuperAdmin={isSuperAdmin}, URL={_exchangeApiUrl}");

            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(_exchangeApiUrl);

            _logger.LogInformation($"[GetExchangeRate] Response received: StatusCode={response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"[GetExchangeRate] FAILED - StatusCode={response.StatusCode}, ErrorResponse={errorContent}");
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"[GetExchangeRate] Success Response: {responseContent}");

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var date = jsonResponse.GetProperty("date").GetDateTime();
            var exchangeRate = jsonResponse.GetProperty("usd");
            var vndRate = exchangeRate.GetProperty("vnd").GetDouble();

            if (isSuperAdmin)
            {
                _logger.LogInformation($"[GetExchangeRate] SUCCESS - Date={date}, VNDRate={vndRate}");

                return new ResponseGetExchangeApiDto
                {
                    Date = date,
                    ExchangeRate = vndRate
                };
            }
            else
            {
                _logger.LogWarning($"[GetExchangeRate] User is not SuperAdmin, returning null");
                return null;
            }
        }
    }
}