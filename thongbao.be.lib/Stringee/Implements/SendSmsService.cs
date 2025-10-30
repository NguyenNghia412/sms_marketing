using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace thongbao.be.lib.Stringee.Implements
{
    public class SendSmsService : ISendSmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SendSmsService> _logger;
        private readonly string _baseUrl;

        public SendSmsService(
            IConfiguration configuration,
            IAuthService authService,
            HttpClient httpClient,
            ILogger<SendSmsService> logger)
        {
            _configuration = configuration;
            _authService = authService;
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = _configuration["Stringee:BaseUrl"] ?? "";
        }

        public async Task<object> SendSmsAsync(List<object> smsMessages)
        {
            if (smsMessages == null || !smsMessages.Any())
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest);
            }

            var jwtToken = await _authService.GenerateJwtTokenAsync();

            var requestBody = new
            {
                sms = smsMessages
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-STRINGEE-AUTH", jwtToken);

           // _logger.LogInformation($"Sending SMS to Stringee API: {_baseUrl}, Message count: {smsMessages.Count}");

            var response = await _httpClient.PostAsync(_baseUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            /*if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Stringee API Error - StatusCode: {response.StatusCode}, Response: {responseContent}");
                throw new UserFriendlyException(ErrorCodes.InternalServerError);
            }*/

            //_logger.LogInformation($"Stringee API Success - Response: {responseContent}");

            var responseObject = JsonSerializer.Deserialize<object>(responseContent);
            return responseObject ?? "";
        }
    }
}