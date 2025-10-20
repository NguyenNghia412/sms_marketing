using AutoMapper;
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
using thongbao.be.application.Base;
using thongbao.be.infrastructure.data;
using thongbao.be.lib.CdsConnect.Dtos.Base;
using thongbao.be.lib.CdsConnect.Dtos.SvValidate;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.lib.Stringee.Implements
{
    public class ProfileService:BaseService, IProfileService
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
            IMapper mapper,
            IConfiguration configuration,
            IAuthService authService,
            HttpClient httpClient): base(smDbContext, logger, httpContextAccessor, mapper)
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
           // var httpContent = new StringContent("application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-STRINGEE-AUTH", jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var account = jsonResponse.GetProperty("account");
            var amountUsd = decimal.Parse(account.GetProperty("amount").GetString() ?? "0",
                           CultureInfo.InvariantCulture);
   

            if (isSuperAdmin)
            {
                var vndRate = await GetExchangeRate();
                var amount = amountUsd * Convert.ToDecimal(vndRate.ExchangeRate);
                return new BaseResponseProfile
                {
                    Code = jsonResponse.GetProperty("r").GetInt32(),
                    //Message = response.Message,
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
                return null;
            }
        }
        public async Task<ResponseGetExchangeApiDto?> GetExchangeRate()
        {
            var isSuperAdmin = IsSuperAdmin();
            _httpClient.DefaultRequestHeaders.Clear();
            var response = await _httpClient.GetAsync(_exchangeApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var date = jsonResponse.GetProperty("date").GetDateTime();
            var exchangeRate = jsonResponse.GetProperty("usd");
            var vndRate = exchangeRate.GetProperty("vnd").GetDouble() ;
            if (isSuperAdmin)
            {
                return new ResponseGetExchangeApiDto
                {
                    Date = date,
                    ExchangeRate = vndRate
                };
            }
            else
            {
                return null;
            }
            
        }
    }
}
