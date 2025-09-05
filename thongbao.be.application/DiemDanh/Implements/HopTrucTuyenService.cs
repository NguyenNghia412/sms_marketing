using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using thongbao.be.application.Base;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.application.DiemDanh.Interfaces;
using thongbao.be.infrastructure.data;
using thongbao.be.shared.HttpRequest.BaseRequest;


namespace thongbao.be.application.DiemDanh.Implements
{
    public class HopTrucTuyenService: BaseService, IHopTrucTuyenService

    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HopTrucTuyenService(
            SmDbContext smDbContext,
      
            ILogger<HopTrucTuyenService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            HttpClient httpClient,
            IConfiguration configuration

        ) 
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _httpClient = httpClient;
            _configuration = configuration;

        }

        public void Create(CreateCuocHopDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
   


            var cuocHop = new domain.DiemDanh.HopTrucTuyen
            {
                TenCuocHop = dto.TenCuocHop,
                MoTa = dto.MoTa,
                ThoiGianBatDau = dto.ThoiGianBatDau ?? DateTime.Now,
                ThoiGianKetThuc = dto.ThoiGianKetThuc ?? DateTime.Now,
                ThoiGianDiemDanh = dto.ThoiGianDiemDanh ?? DateTime.Now,
                LinkCuocHop = dto.LinkCuocHop,
                UserIdCreated = userId,

                CreatedDate = DateTime.Now,
                Deleted = false
            };
            _smDbContext.HopTrucTuyens.Add(cuocHop);
            _smDbContext.SaveChanges();
        }

        public BaseResponsePagingDto<ViewCuocHopDto> Find(FindPagingCuocHopDto dto)
        {
            _logger.LogInformation($"{nameof(Find)} dto={JsonSerializer.Serialize(dto)}");
            var query = from ch in _smDbContext.HopTrucTuyens
                        where ch.Deleted == false
                        orderby ch.CreatedDate descending
                        select ch;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewCuocHopDto>>(data);
            foreach (var item in items)
            {
                var cuocHop = data.FirstOrDefault(x => x.Id == item.Id);
                if (cuocHop?.UserIdCreated != null && !string.IsNullOrEmpty(cuocHop.UserIdCreated))
                {
                    var user = _smDbContext.Users.FirstOrDefault(u => u.Id == cuocHop.UserIdCreated);
                    if (user != null)
                    {
                        if (Guid.TryParse(user.Id, out Guid userIdGuid))
                        {
                            item.Item = new UserCreateCuocHopDto
                            {
                                Id = userIdGuid, 
                                UserName = user.UserName ?? "",
                                Email = user.Email ?? "",
                                FullName = user.FullName ?? ""
                            };
                            var userRole = _smDbContext.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                            if (userRole != null)
                            {
                                var role = _smDbContext.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                                if (role != null)
                                {
                                    if (Guid.TryParse(role.Id, out Guid roleIdGuid))
                                    {
                                        item.Item.Item = new RoleUserDto
                                        {
                                            Id = roleIdGuid, 
                                            Name = role.Name ?? ""
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new BaseResponsePagingDto<ViewCuocHopDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }

        public void Update(UpdateCuochopDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} dto={JsonSerializer.Serialize(dto)}");

        }
        public GraphApiAuthUrlResponseDto GenerateMicrosoftAuthUrl()
        {
            _logger.LogInformation($"{nameof(GenerateMicrosoftAuthUrl)} started");

            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var redirectUri = _configuration["AzureAd:RedirectUri"]; 

            // Tạo PKCE data
            var pkce = GeneratePKCE();

            // Các scope cần thiết để lấy thông tin user và meetings
            var scopes = new[]
            {
                "https://graph.microsoft.com/User.Read",
                "https://graph.microsoft.com/User.Read.All",
                "https://graph.microsoft.com/User.ReadBasic.All",
                "https://graph.microsoft.com/OnlineMeetings.Read",
                "https://graph.microsoft.com/OnlineMeetings.ReadWrite",
                "https://graph.microsoft.com/Chat.Read",
                "https://graph.microsoft.com/Chat.ReadWrite",
                "https://graph.microsoft.com/Calendars.Read",
                "https://graph.microsoft.com/Calendars.ReadWrite"
            };

            var scopeString = string.Join(" ", scopes);

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["client_id"] = clientId;
            queryParams["response_type"] = "code";
            queryParams["redirect_uri"] = redirectUri;
            queryParams["response_mode"] = "query";
            queryParams["scope"] = scopeString;
            queryParams["state"] = pkce.State;
            queryParams["prompt"] = "select_account";
            queryParams["code_challenge"] = pkce.CodeChallenge;
            queryParams["code_challenge_method"] = "S256";

            var authorizationUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize?{queryParams}";

            _logger.LogInformation($"{nameof(GenerateMicrosoftAuthUrl)} generated URL successfully");

            return new GraphApiAuthUrlResponseDto
            {
                AuthUrl = authorizationUrl,
                State = pkce.State,
                CodeVerifier = pkce.CodeVerifier
            };
        }

        public async Task<GraphApiTokenResponseDto> HandleMicrosoftCallback(GraphApiCallbackDto dto)
        {
            _logger.LogInformation($"{nameof(HandleMicrosoftCallback)} dto={JsonSerializer.Serialize(dto)}");
            var tokenResponse = await ExchangeCodeForToken(dto.Code, dto.CodeVerifier);
            return tokenResponse;
        }

        private async Task<GraphApiTokenResponseDto> ExchangeCodeForToken(string authorizationCode, string codeVerifier = null)
        {
            _logger.LogInformation($"{nameof(ExchangeCodeForToken)} started");

            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];
            var redirectUri = _configuration["AzureAd:RedirectUri"];

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var requestData = new List<KeyValuePair<string, string>>
            {
                new("client_id", clientId),
                new("scope", "https://graph.microsoft.com/User.Read https://graph.microsoft.com/User.Read.All https://graph.microsoft.com/User.ReadBasic.All https://graph.microsoft.com/OnlineMeetings.Read https://graph.microsoft.com/OnlineMeetings.ReadWrite https://graph.microsoft.com/Chat.Read https://graph.microsoft.com/Chat.ReadWrite https://graph.microsoft.com/Calendars.Read https://graph.microsoft.com/Calendars.ReadWrite"),
                new("code", authorizationCode),
                new("redirect_uri", redirectUri),
                new("grant_type", "authorization_code")
            };

            // Với Web app type, cần có client_secret
            if (!string.IsNullOrEmpty(codeVerifier))
            {
                requestData.Add(new("code_verifier", codeVerifier));
            }

            // Luôn thêm client_secret cho Web app type
            if (!string.IsNullOrEmpty(clientSecret))
            {
                requestData.Add(new("client_secret", clientSecret));
            }

            var requestContent = new FormUrlEncodedContent(requestData);

            // Không cần thêm Origin header cho Web app type - server-to-server request
            _httpClient.DefaultRequestHeaders.Clear();

            var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var tokenResponse = new GraphApiTokenResponseDto
            {
                AccessToken = tokenData.GetProperty("access_token").GetString() ?? "",
                TokenType = tokenData.GetProperty("token_type").GetString() ?? "",
                ExpiresIn = tokenData.GetProperty("expires_in").GetInt32(),
                Scope = tokenData.TryGetProperty("scope", out var scopeElement) ? scopeElement.GetString() ?? "" : "",
                RefreshToken = tokenData.TryGetProperty("refresh_token", out var refreshElement) ? refreshElement.GetString() ?? "" : ""
            };
            return tokenResponse;
        }
        public async Task<GraphApiUserInforResponseDto> GetUserInfo(string accessToken)
        {
            _logger.LogInformation($"{nameof(GetUserInfo)} started");
            var graphEndpoint = "https://graph.microsoft.com/v1.0/me";

            try
            {
                // Clear headers và thêm Authorization header
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(graphEndpoint);
                var responseContent = await response.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var userInfo = new GraphApiUserInforResponseDto
                {
                    Id = userData.TryGetProperty("id", out var idElement) ? idElement.GetString() ?? "" : "",
                    DisplayName = userData.TryGetProperty("displayName", out var displayNameElement) ? displayNameElement.GetString() ?? "" : "",
                    GivenName = userData.TryGetProperty("givenName", out var givenNameElement) ? givenNameElement.GetString() ?? "" : "",
                    Surname = userData.TryGetProperty("surname", out var surnameElement) ? surnameElement.GetString() ?? "" : "",
                    Mail = userData.TryGetProperty("mail", out var mailElement) ? mailElement.GetString() ?? "" : "",
                    UserPrincipalName = userData.TryGetProperty("userPrincipalName", out var upnElement) ? upnElement.GetString() ?? "" : "",
                    JobTitle = userData.TryGetProperty("jobTitle", out var jobTitleElement) ? jobTitleElement.GetString() ?? "" : "",
                    OfficeLocation = userData.TryGetProperty("officeLocation", out var officeElement) ? officeElement.GetString() ?? "" : "",
                    MobilePhone = userData.TryGetProperty("mobilePhone", out var mobileElement) ? mobileElement.GetString() ?? "" : ""
                };
                return userInfo;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private PKCEDto GeneratePKCE()
        {
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = Guid.NewGuid().ToString();

            return new PKCEDto
            {
                CodeVerifier = codeVerifier,
                CodeChallenge = codeChallenge,
                State = state
            };
        }

        private string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            var bytes = Encoding.UTF8.GetBytes(codeVerifier);
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
            }
        }
    }
}

