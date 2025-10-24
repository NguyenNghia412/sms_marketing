using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

    namespace thongbao.be.lib.Stringee.Implements
    {
        public class SendSmsService : ISendSmsService
        {
            private readonly IConfiguration _configuration;
            private readonly IAuthService _authService;
            private readonly HttpClient _httpClient;
            private readonly string _baseUrl;

            public SendSmsService(
                IConfiguration configuration,
                IAuthService authService,
                HttpClient httpClient)
            {
                _configuration = configuration;
                _authService = authService;
                _httpClient = httpClient;
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

                var response = await _httpClient.PostAsync(_baseUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new UserFriendlyException(ErrorCodes.InternalServerError);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<object>(responseContent);

                return responseObject ?? "";
            }
     
    }

}
