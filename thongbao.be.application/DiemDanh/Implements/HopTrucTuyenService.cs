using AutoMapper;
using Azure.Identity;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using thongbao.be.shared.Constants.DiemDanh;
using thongbao.be.shared.HttpRequest.BaseRequest;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.application.DiemDanh.Implements
{
    public class HopTrucTuyenService : BaseService,  IHopTrucTuyenService 
    {
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphServiceClient;
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public HopTrucTuyenService(
            SmDbContext smDbContext,
            GraphServiceClient graphServiceClient,
            ILogger<HopTrucTuyenService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration
        )
            : base(smDbContext, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
            _graphServiceClient = graphServiceClient;
        }

        public void Create(CreateCuocHopDto dto)
        {
            _logger.LogInformation($"{nameof(Create)} dto={JsonSerializer.Serialize(dto)}");

            var vietnamNow = GetVietnamTime();

            var existingCuocHop = _smDbContext.HopTrucTuyens
                .FirstOrDefault(h => h.TenCuocHop.ToLower() == dto.TenCuocHop.ToLower() && !h.Deleted);

            if (existingCuocHop != null)
            {
                throw new UserFriendlyException(409, "Tên cuộc họp đã tồn tại");
            }


            var thoiGianBatDau = dto.ThoiGianBatDau ?? vietnamNow;
            var thoiGianKetThuc = dto.ThoiGianKetThuc ?? vietnamNow;

            if (thoiGianKetThuc < thoiGianBatDau)
            {
                throw new UserFriendlyException(400, "Thời gian kết thúc cuộc họp phải lớn hơn hoặc bằng thời gian bắt đầu cuộc họp");
            }

            var cuocHop = new domain.DiemDanh.HopTrucTuyen
            {
                TenCuocHop = dto.TenCuocHop,
                MoTa = dto.MoTa,
                ThoiGianBatDau = thoiGianBatDau,
                ThoiGianKetThuc = thoiGianKetThuc,
                CreatedDate = vietnamNow,
                Deleted = false
            };

            _smDbContext.HopTrucTuyens.Add(cuocHop);
            _smDbContext.SaveChanges();

            _logger.LogInformation($"Đã tạo cuộc họp thành công: {dto.TenCuocHop}");
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



            return new BaseResponsePagingDto<ViewCuocHopDto>
            {
                Items = items,
                TotalItems = query.Count()
            };
        }

        public void Update(int idCuocHop,UpdateCuochopDto dto)
        {
            _logger.LogInformation($"{nameof(Update)} dto={JsonSerializer.Serialize(dto)}");

            var vietnamNow = GetVietnamTime();
            var existingCuocHop = _smDbContext.HopTrucTuyens
                .FirstOrDefault(h => h.Id == idCuocHop && !h.Deleted);
            if (existingCuocHop == null)
            {
                throw new UserFriendlyException(404,"Cuộc họp không tồn tại.");
            }

            var existingTenCuocHop = _smDbContext.HopTrucTuyens
                .FirstOrDefault(h => h.TenCuocHop.ToLower() == dto.TenCuocHop.ToLower() && !h.Deleted);

            if (existingTenCuocHop != null)
            {
                throw new UserFriendlyException(409, "Tên cuộc họp đã tồn tại");
            }


            var thoiGianBatDau = dto.ThoiGianBatDau ?? vietnamNow;
            var thoiGianKetThuc = dto.ThoiGianKetThuc ?? vietnamNow;

            if (thoiGianKetThuc < thoiGianBatDau)
            {
                throw new UserFriendlyException(400, "Thời gian kết thúc cuộc họp phải lớn hơn hoặc bằng thời gian bắt đầu cuộc họp");
            }
            existingCuocHop.TenCuocHop = dto.TenCuocHop;
            existingCuocHop.MoTa = dto.MoTa;
            existingCuocHop.ThoiGianBatDau = dto.ThoiGianBatDau;
            existingCuocHop.ThoiGianKetThuc = dto.ThoiGianKetThuc;

            _smDbContext.SaveChanges();

        }
        public void Delete(int idCuocHop)
        {
            _logger.LogInformation($"{nameof(Delete)}");
            var vietNamNow = GetVietnamTime();
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingCuocHop = _smDbContext.HopTrucTuyens
                .FirstOrDefault(h => h.Id == idCuocHop && !h.Deleted);

            if( existingCuocHop == null)
            {
                throw new UserFriendlyException(400, "Cuộc họp không tồn tại");
            }
            existingCuocHop.Deleted = true;
            existingCuocHop.DeletedDate = vietNamNow;


            var thongTinDiemDanhList =_smDbContext.ThongTinDiemDanhs
                .Where(h => h.IdHopTrucTuyen == idCuocHop && !h.Deleted)
                .ToList();

            foreach( var thongtinDiemDanh in thongTinDiemDanhList)
            {
                thongtinDiemDanh.Deleted = true;
                thongtinDiemDanh.DeletedDate = vietNamNow;
            }

            var tinNhanHopTrucTuyenList = _smDbContext.TinNhanHopTrucTuyens
                .Where(h => h.CuocHopId == idCuocHop && !h.Deleted)
                .ToList() ;

            foreach(var tinNhanHopTrucTuyen in tinNhanHopTrucTuyenList)
            {
                tinNhanHopTrucTuyen.Deleted = true;
                tinNhanHopTrucTuyen.DeletedDate = vietNamNow;
            }
            _smDbContext.SaveChanges();
        }

       /* public GraphApiAuthUrlResponseDto GenerateMicrosoftAuthUrl()
        {
            _logger.LogInformation($"{nameof(GenerateMicrosoftAuthUrl)} started");

            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var redirectUri = _configuration["AzureAd:RedirectUri"];

            var pkce = GeneratePKCE();

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
            var tokenResponse = await ExchangeCodeForTokenAsync(dto.Code, dto.CodeVerifier);
            return tokenResponse;
        }

        public async Task<GraphApiUserInforResponseDto> GetUserInfo(string accessToken)
        {
            _logger.LogInformation($"{nameof(GetUserInfo)} started");

            try
            {
                var userGraphClient = CreateUserGraphClient(accessToken);
                var user = await userGraphClient.Me.GetAsync();

                var userInfo = new GraphApiUserInforResponseDto
                {
                    Id = user?.Id ?? "",
                    DisplayName = user?.DisplayName ?? "",
                    GivenName = user?.GivenName ?? "",
                    Surname = user?.Surname ?? "",
                    Mail = user?.Mail ?? "",
                    UserPrincipalName = user?.UserPrincipalName ?? "",
                    JobTitle = user?.JobTitle ?? "",
                    OfficeLocation = user?.OfficeLocation ?? "",
                    MobilePhone = user?.MobilePhone ?? ""
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetUserInfo)}");
                throw;
            }
        }*/
        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            _logger.LogInformation($"{nameof(GetUserIdByEmailAsync)} email={email}");

            try
            {
                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"];

                var scopes = new[]
                {
            "https://graph.microsoft.com/.default"
        };

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                var users = await graphClient.Users
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"mail eq '{email}' or userPrincipalName eq '{email}'";
                        requestConfiguration.QueryParameters.Select = new[] { "id" };
                        requestConfiguration.QueryParameters.Top = 1;
                    });

                if (users?.Value != null && users.Value.Any())
                {
                    return users.Value.First().Id ?? string.Empty;
                }

                throw new UserFriendlyException(404, $"Không tìm thấy user với email: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetUserIdByEmailAsync)} for email: {email}");
                throw;
            }
        }
        public async Task<MettingIdDto> GetAndSaveMeetingInfo(GraphApiGetThongTinCuocHopDto dto, string userId)
        {
            var meetingData = await GetThongTinCuocHop(dto, userId);
            await SaveMeetingInfoAsync(dto, meetingData);
            return meetingData;
        }
        public async Task<MettingIdDto> GetThongTinCuocHop(GraphApiGetThongTinCuocHopDto dto, string userId)
        {
            _logger.LogInformation($"{nameof(GetThongTinCuocHop)} dto={JsonSerializer.Serialize(dto)}, userId={userId}");

            try
            {
                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"];

                var scopes = new[]
                {
            "https://graph.microsoft.com/.default"
        };
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var userGraphClient = new GraphServiceClient(clientSecretCredential, scopes);

                if (!IsValidJoinWebUrl(dto.JoinWebUrl))
                {
                    throw new UserFriendlyException(400, "Invalid Join Web URL");
                }

                var encodedUrl = HttpUtility.UrlEncode(dto.JoinWebUrl);
                var filter = $"JoinWebUrl eq '{encodedUrl}'";

                var meetings = await userGraphClient.Users[userId].OnlineMeetings
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = filter;
                    });

                var result = MapToMeetingDto(meetings);

                if (result.Value == null || result.Value.Count == 0)
                {
                    throw new UserFriendlyException(404, "Meeting not found");
                }

                foreach (var meeting in result.Value)
                {
                    if (!string.IsNullOrEmpty(meeting.ChatInfo?.ThreadId))
                    {
                        var chatMembers = await GetMeetingChatMembersAsync(meeting.ChatInfo.ThreadId);

                        foreach (var member in chatMembers)
                        {
                            var memberGuid = ExtractRealUserIdFromChatMemberId(member.Identity?.User?.Id);
                            bool isOrganizer = meeting.Participants?.Organizer?.Identity?.User?.Id == memberGuid;

                            if (member.Identity?.User != null)
                            {
                                var userMessages = await GetChatMessagesForUserAsync(meeting.ChatInfo.ThreadId, memberGuid, member.Identity.User.Id);
                                member.Identity.User.ChatMessages = userMessages;
                            }

                            if (isOrganizer)
                            {
                                if (meeting.Participants?.Organizer?.Identity?.User != null &&
                                    string.IsNullOrEmpty(meeting.Participants.Organizer.Identity.User.DisplayName))
                                {
                                    meeting.Participants.Organizer.Identity.User.DisplayName = member.Identity?.User?.DisplayName;
                                }

                                if (meeting.Participants?.Organizer?.Identity?.User != null)
                                {
                                    meeting.Participants.Organizer.Identity.User.ChatMessages = member.Identity?.User?.ChatMessages ?? new List<ChatMessageDto>();
                                    meeting.Participants.Organizer.Identity.User.SurName = member.Identity?.User?.SurName;
                                    meeting.Participants.Organizer.Identity.User.GivenName = member.Identity?.User?.GivenName;
                                    meeting.Participants.Organizer.Identity.User.MobilePhone = member.Identity?.User?.MobilePhone;
                                    meeting.Participants.Organizer.Identity.User.JobTitle = member.Identity?.User?.JobTitle;
                                    meeting.Participants.Organizer.Identity.User.OfficeLocation = member.Identity?.User?.OfficeLocation;
                                }

                                meeting.Participants?.Attendees.Add(member);
                            }
                            else
                            {
                                meeting.Participants?.Attendees.Add(member);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetThongTinCuocHop)}");
                throw;
            }
        }

        private async Task<List<IdentitySetDto>> GetMeetingChatMembersAsync(string threadId)
        {
            try
            {

                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"];

                var scopes = new[]
                {
                    "https://graph.microsoft.com/.default"
                };
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var userGraphClient = new GraphServiceClient(clientSecretCredential, scopes);
                var chatMembers = await userGraphClient.Chats[threadId].Members.GetAsync();
                var participants = new List<IdentitySetDto>();

                if (chatMembers?.Value != null)
                {
                    foreach (var member in chatMembers.Value)
                    {
                        var participant = new IdentitySetDto
                        {
                            Identity = new IdentityDto
                            {
                                User = new UserInforDto
                                {
                                    Id = member.Id ?? string.Empty,
                                    DisplayName = member.DisplayName
                                }
                            }
                        };

                        if (member.Roles != null && member.Roles.Any())
                        {
                            participant.Role = member.Roles.First()?.ToString();
                        }

                        if (!string.IsNullOrEmpty(participant.Identity.User.Id))
                        {
                            try
                            {
                                var realUserId = ExtractRealUserIdFromChatMemberId(participant.Identity.User.Id);

                                if (!string.IsNullOrEmpty(realUserId))
                                {
                                    var userDetails = await userGraphClient.Users[realUserId]
                                        .GetAsync(requestConfiguration =>
                                        {
                                            requestConfiguration.QueryParameters.Select = new[]
                                            {
                                                "id", "displayName", "userPrincipalName", "surname",
                                                "givenName", "mobilePhone", "jobTitle", "officeLocation"
                                            };
                                        });

                                    if (userDetails != null)
                                    {
                                        participant.Upn = userDetails.UserPrincipalName;
                                        participant.Identity.User.DisplayName = userDetails.DisplayName;
                                        participant.Identity.User.SurName = userDetails.Surname;
                                        participant.Identity.User.GivenName = userDetails.GivenName;
                                        participant.Identity.User.MobilePhone = userDetails.MobilePhone;
                                        participant.Identity.User.JobTitle = userDetails.JobTitle;
                                        participant.Identity.User.OfficeLocation = userDetails.OfficeLocation;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"Failed to get user details for member {participant.Identity.User.Id}");
                            }
                        }

                        participants.Add(participant);
                    }
                }

                return participants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat members for thread {threadId}");
                return new List<IdentitySetDto>();
            }
        }

        private async Task<List<ChatMessageDto>> GetChatMessagesForUserAsync(string threadId, string memberGuid, string memberId)
        {
            try
            {
                _logger.LogInformation($"GetChatMessages - ThreadId: {threadId}, MemberGuid: {memberGuid}, MemberId: {memberId}");

                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"];

                var scopes = new[]
                {
                    "https://graph.microsoft.com/.default"
                };
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var userGraphClient = new GraphServiceClient(clientSecretCredential, scopes);
                var messages = await userGraphClient.Chats[threadId].Messages
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Top = 50;
                        requestConfiguration.QueryParameters.Orderby = new[] { "createdDateTime desc" };
                        // REMOVED Select parameter - this is causing the error
                    });

                var chatMessages = new List<ChatMessageDto>();

                if (messages?.Value != null)
                {
                    _logger.LogInformation($"Total messages found: {messages.Value.Count}");

                    foreach (var message in messages.Value)
                    {
                        try
                        {
                            if (message.From?.User?.Id == null)
                            {
                                continue;
                            }

                            bool isUserMessage = message.From?.User?.Id == memberGuid || message.From?.User?.Id == memberId;

                            if (isUserMessage)
                            {
                                var chatMessage = new ChatMessageDto
                                {
                                    Id = message.Id ?? string.Empty,
                                    MessageType = message.MessageType?.ToString(),
                                    CreatedDateTime = message.CreatedDateTime?.DateTime != null ?
                                        TimeZoneInfo.ConvertTimeFromUtc(message.CreatedDateTime.Value.DateTime, VietnamTimeZone) : null,
                                    Subject = message.Subject
                                };

                                if (message.Body != null)
                                {
                                    chatMessage.Body = new ChatMessageBodyDto
                                    {
                                        ContentType = message.Body.ContentType?.ToString(),
                                        Content = message.Body.Content
                                    };
                                }

                                chatMessages.Add(chatMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Error processing message {message.Id}");
                        }
                    }
                }

                return chatMessages.OrderBy(msg => msg.CreatedDateTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat messages for user: {ex.Message}");
                return new List<ChatMessageDto>();
            }
        }

        public async Task<List<ChatMessageDto>> GetChatMessagesAsync(string threadId)
        {
            try
            {
                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"];

                var scopes = new[]
                {
                    "https://graph.microsoft.com/.default"
                };
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var userGraphClient = new GraphServiceClient(clientSecretCredential, scopes);
                var messages = await userGraphClient.Chats[threadId].Messages
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Top = 50;
                        requestConfiguration.QueryParameters.Orderby = new[] { "createdDateTime desc" };
                    });

                var chatMessages = new List<ChatMessageDto>();

                if (messages?.Value != null)
                {
                    foreach (var message in messages.Value)
                    {
                        var chatMessage = new ChatMessageDto
                        {
                            Id = message.Id ?? string.Empty,
                            MessageType = message.MessageType?.ToString(),
                            CreatedDateTime = message.CreatedDateTime?.DateTime != null ?
                                TimeZoneInfo.ConvertTimeFromUtc(message.CreatedDateTime.Value.DateTime, VietnamTimeZone) : null,
                            Subject = message.Subject
                        };

                        if (message.Body != null)
                        {
                            chatMessage.Body = new ChatMessageBodyDto
                            {
                                ContentType = message.Body.ContentType?.ToString(),
                                Content = message.Body.Content
                            };
                        }

                        chatMessages.Add(chatMessage);
                    }
                }

                return chatMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat messages for thread {threadId}");
                return new List<ChatMessageDto>();
            }
        }

        /*private async Task<GraphApiTokenResponseDto> ExchangeCodeForTokenAsync(string authorizationCode, string codeVerifier = null)
        {
            _logger.LogInformation($"{nameof(ExchangeCodeForTokenAsync)} started");

            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];
            var redirectUri = _configuration["AzureAd:RedirectUri"];

            using var httpClient = new HttpClient();
            var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var requestData = new List<KeyValuePair<string, string>>
            {
                new("client_id", clientId),
                new("scope", "https://graph.microsoft.com/User.Read https://graph.microsoft.com/User.Read.All https://graph.microsoft.com/User.ReadBasic.All https://graph.microsoft.com/OnlineMeetings.Read https://graph.microsoft.com/OnlineMeetings.ReadWrite https://graph.microsoft.com/Chat.Read https://graph.microsoft.com/Chat.ReadWrite https://graph.microsoft.com/Calendars.Read https://graph.microsoft.com/Calendars.ReadWrite"),
                new("code", authorizationCode),
                new("redirect_uri", redirectUri),
                new("grant_type", "authorization_code")
            };

            if (!string.IsNullOrEmpty(codeVerifier))
            {
                requestData.Add(new("code_verifier", codeVerifier));
            }

            if (!string.IsNullOrEmpty(clientSecret))
            {
                requestData.Add(new("client_secret", clientSecret));
            }

            var requestContent = new FormUrlEncodedContent(requestData);

            var response = await httpClient.PostAsync(tokenEndpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Token exchange failed: {responseContent}");
            }

            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseContent);

            return new GraphApiTokenResponseDto
            {
                AccessToken = tokenData.GetProperty("access_token").GetString() ?? "",
                TokenType = tokenData.GetProperty("token_type").GetString() ?? "",
                ExpiresIn = tokenData.GetProperty("expires_in").GetInt32(),
                Scope = tokenData.TryGetProperty("scope", out var scopeElement) ? scopeElement.GetString() ?? "" : "",
                RefreshToken = tokenData.TryGetProperty("refresh_token", out var refreshElement) ? refreshElement.GetString() ?? "" : ""
            };
        }*/

        private static DateTime? ParseAndConvertDateTime(DateTimeOffset? dateTimeOffset)
        {
            try
            {
                if (dateTimeOffset == null)
                    return null;

                var utcDateTime = dateTimeOffset.Value.UtcDateTime;
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsValidJoinWebUrl(string url)
        {
            return !string.IsNullOrWhiteSpace(url) &&
                   Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   uri.Host.Contains("teams.microsoft.com");
        }

        private static MettingIdDto MapToMeetingDto(OnlineMeetingCollectionResponse? meetings)
        {
            var result = new MettingIdDto();

            if (meetings?.Value == null)
            {
                return result;
            }

            foreach (var meeting in meetings.Value)
            {
                var onlineMeeting = new OnlineMeetingDto
                {
                    Id = meeting.Id ?? string.Empty,
                    Subject = meeting.Subject,
                    JoinWebUrl = meeting.JoinWebUrl ?? string.Empty,
                    CreationDateTime = ParseAndConvertDateTime(meeting.CreationDateTime),
                    StartDateTime = ParseAndConvertDateTime(meeting.StartDateTime),
                    EndDateTime = ParseAndConvertDateTime(meeting.EndDateTime)
                };

                if (meeting.ChatInfo != null)
                {
                    onlineMeeting.ChatInfo = new ChatInforDto
                    {
                        ThreadId = meeting.ChatInfo.ThreadId ?? string.Empty
                    };
                }

                if (meeting.Participants != null)
                {
                    onlineMeeting.Participants = new MeetingParticipantsDto();

                    if (meeting.Participants.Organizer != null)
                    {
                        onlineMeeting.Participants.Organizer = MapIdentitySet(meeting.Participants.Organizer);
                    }

                    if (meeting.Participants.Attendees != null)
                    {
                        foreach (var attendee in meeting.Participants.Attendees)
                        {
                            var mappedAttendee = MapIdentitySet(attendee);
                            if (mappedAttendee != null)
                                onlineMeeting.Participants.Attendees.Add(mappedAttendee);
                        }
                    }
                }

                result.Value.Add(onlineMeeting);
            }

            return result;
        }

        private static IdentitySetDto? MapIdentitySet(MeetingParticipantInfo participant)
        {
            if (participant?.Identity?.User == null) return null;

            return new IdentitySetDto
            {
                Upn = participant.Upn,
                Role = participant.Role.ToString(),
                Identity = new IdentityDto
                {
                    User = new UserInforDto
                    {
                        Id = participant.Identity.User.Id ?? string.Empty,
                        DisplayName = participant.Identity.User.DisplayName
                    }
                }
            };
        }

        private static string ExtractRealUserIdFromChatMemberId(string complexId)
        {
            try
            {
                if (string.IsNullOrEmpty(complexId))
                    return null;

                var decodedBytes = Convert.FromBase64String(complexId);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);

                var parts = decodedString.Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var lastPart = parts[parts.Length - 1];
                    if (Guid.TryParse(lastPart, out _))
                    {
                        return lastPart;
                    }
                }
            }
            catch
            {
                // Return null on any error
            }

            return null;
        }
        public async Task SaveMeetingInfoAsync(GraphApiGetThongTinCuocHopDto dto, MettingIdDto meetingData)
        {
            _logger.LogInformation($"{nameof(SaveMeetingInfoAsync)} started for IdCuocHop: {dto.IdCuocHop}");
            var vietnamNow = GetVietnamTime();
            var existingMeeting = await _smDbContext.HopTrucTuyens
                .FirstOrDefaultAsync(h => h.Id == dto.IdCuocHop && !h.Deleted);

            if (existingMeeting == null)
            {
                throw new UserFriendlyException(404,$"Cuộc họp không tồn tại");
            }

            var meeting = meetingData.Value?.FirstOrDefault();
            if (meeting == null)
            {
                throw new UserFriendlyException(404,"Không tìm thấy thông tin meeting");
            }

            existingMeeting.IdCuocHop = meeting.Id;
            existingMeeting.LinkCuocHop = HttpUtility.UrlDecode(dto.JoinWebUrl);
            existingMeeting.ThoiGianTaoCuocHop = meeting.CreationDateTime;
            existingMeeting.IdTinNhanChung = meeting.ChatInfo?.ThreadId ?? string.Empty;
            existingMeeting.UserIdCreated = meeting.Participants?.Organizer?.Identity.User.Id ?? string.Empty;

            var existingDiemDanhRecords = await _smDbContext.ThongTinDiemDanhs
                .Where(t => t.IdHopTrucTuyen == dto.IdCuocHop && !t.Deleted)
                .ToListAsync();

            foreach (var record in existingDiemDanhRecords)
            {
                record.Deleted = true;
                record.DeletedDate = vietnamNow;
            }

            var existingTinNhanRecords = await _smDbContext.TinNhanHopTrucTuyens
                .Where(t => t.CuocHopId == dto.IdCuocHop && !t.Deleted)
                .ToListAsync();

            foreach (var record in existingTinNhanRecords)
            {
                record.Deleted = true;
                record.DeletedDate = vietnamNow;
            }
            if (meeting.Participants?.Attendees != null)
            {
                foreach (var attendee in meeting.Participants.Attendees)
                {
                    if (attendee?.Identity?.User != null && !string.IsNullOrEmpty(attendee.Upn))
                    {
                        var diemDanh = new domain.DiemDanh.ThongTinDiemDanh
                        {
                            IdHopTrucTuyen = dto.IdCuocHop,
                            MaSoSinhVien = GetMaSoFromUpn(attendee.Upn).ToString(),
                            HoVaTen = BuildFullName(attendee.Identity.User.SurName, attendee.Identity.User.GivenName),
                            HoDem = attendee.Identity.User.SurName ?? string.Empty,
                            Ten = attendee.Identity.User.GivenName ?? string.Empty,
                            Khoa = attendee.Identity.User.OfficeLocation ?? string.Empty,
                            LopQuanLy = GetLopFromDisplayName(attendee.Identity.User.DisplayName),
                            EmailHuce = attendee.Upn,
                            SoDienThoai = attendee.Identity.User.MobilePhone ?? string.Empty,
                            TrangThaiDiemDanh = ThongTinDiemDanh.ChuaDiemDanh,
                            CreatedDate = vietnamNow,
                            Deleted = false
                        };

                        _smDbContext.ThongTinDiemDanhs.Add(diemDanh);
                        await _smDbContext.SaveChangesAsync();
                        if (attendee.Identity?.User?.ChatMessages != null && attendee.Identity.User.ChatMessages.Any())
                        {
                            foreach (var chatMessage in attendee.Identity.User.ChatMessages)
                            {
                                if (chatMessage.Body?.Content != null && !string.IsNullOrWhiteSpace(chatMessage.Body.Content))
                                {
                                    var tinNhan = new domain.DiemDanh.TinNhanHopTrucTuyen
                                    {
                                        CuocHopId = dto.IdCuocHop,
                                        ThongTinDiemDanhId = diemDanh.Id,
                                        NoiDung = CleanHtmlContent(chatMessage.Body.Content),
                                        ThoiGianGui = chatMessage.CreatedDateTime ?? vietnamNow,
                                        CreatedDate = vietnamNow,
                                        Deleted = false
                                    };

                                    _smDbContext.TinNhanHopTrucTuyens.Add(tinNhan);
                                }
                            }
                        }
                    }
                }
            }

            await _smDbContext.SaveChangesAsync();
            _logger.LogInformation($"Successfully saved meeting data for IdCuocHop: {dto.IdCuocHop}");
        }
        public async Task UpdateTrangThaiDiemDanh(int idCuocHop, UpdateTrangThaiDiemDanhDto dto)
        {
            _logger.LogInformation($"{nameof(UpdateTrangThaiDiemDanh)} started");

            var vietnamNow = GetVietnamTime();

            var cuocHop = await _smDbContext.HopTrucTuyens
                .FirstOrDefaultAsync(h => h.Id == idCuocHop && !h.Deleted);

            if (cuocHop == null)
            {
                throw new UserFriendlyException(404, $"Cuộc họp  không tồn tại");
            }

            cuocHop.BatDauDiemDanh = dto.BatDauDiemDanh;

            var diemDanhData = await (from ttdd in _smDbContext.ThongTinDiemDanhs
                                      where ttdd.IdHopTrucTuyen == idCuocHop && !ttdd.Deleted
                                      where ttdd.IdHopTrucTuyen == idCuocHop && !ttdd.Deleted
                                      select new
                                      {
                                          DiemDanh = ttdd,
                                          TinNhan = _smDbContext.TinNhanHopTrucTuyens
                                              .Where(tn => tn.ThongTinDiemDanhId == ttdd.Id && !tn.Deleted)
                                              .OrderBy(tn => tn.ThoiGianGui)
                                              .ToList()
                                      }).ToListAsync();

            var thoiGianBatDau = dto.BatDauDiemDanh;
            var thoiGianKetThuc = dto.KetThucDiemDanh;
            var updateTasks = diemDanhData.Select(async item =>
            {
                var diemDanh = item.DiemDanh;
                var tinNhanList = item.TinNhan;
                var expectedMessage = $"{diemDanh.MaSoSinhVien} đã điểm danh";
                var validMessages = tinNhanList.Where(tn =>
                    !string.IsNullOrEmpty(tn.NoiDung) &&
                    tn.NoiDung.Trim().Equals(expectedMessage, StringComparison.OrdinalIgnoreCase)).ToList();
                var hasValidMessage = validMessages.Any();
                if (!hasValidMessage)
                {
                    diemDanh.TrangThaiDiemDanh = ThongTinDiemDanh.VangMat;
                    return;
                }
                var validTimeMessages = tinNhanList.Where(tn =>
                    !string.IsNullOrEmpty(tn.NoiDung) &&
                    tn.NoiDung.Trim().Equals(expectedMessage, StringComparison.OrdinalIgnoreCase) &&
                    tn.ThoiGianGui > thoiGianBatDau &&
                    tn.ThoiGianGui <= thoiGianKetThuc).ToList();

                var validTimeMessage = validTimeMessages.Any();
                if (validTimeMessage)
                {
                    diemDanh.TrangThaiDiemDanh = ThongTinDiemDanh.DaDiemDanh;
                }
                else
                {
                    diemDanh.TrangThaiDiemDanh = ThongTinDiemDanh.VangMat;
                }
            });
            await Task.WhenAll(updateTasks);
            await _smDbContext.SaveChangesAsync();
        }


        public BaseResponsePagingDto<ViewThongTinDiemDanhDto> ThongTinDiemDanhPaging (int idCuocHop,FindPagingThongTinDiemDanhDto dto)
        {
            _logger.LogInformation($"{nameof(ThongTinDiemDanh)} dto={JsonSerializer.Serialize(dto)}");
            var query = from ttdd in _smDbContext.ThongTinDiemDanhs
                        where ttdd.IdHopTrucTuyen == idCuocHop
                        where ttdd.Deleted == false
                        orderby ttdd.Id descending
                        select ttdd;
            var data = query.Paging(dto).ToList();
            var items = _mapper.Map<List<ViewThongTinDiemDanhDto>>(data);
            return new BaseResponsePagingDto<ViewThongTinDiemDanhDto>
            {
                Items = items,
                TotalItems = query.Count()
            };

        }
        public async Task<byte[]> ExportDanhSachDiemDanhToExcel(int idCuocHop)
        {
            _logger.LogInformation($"{nameof(ExportDanhSachDiemDanhToExcel)} started for IdCuocHop: {idCuocHop}");

            var cuocHopInfo = await (from ch in _smDbContext.HopTrucTuyens
                                     where ch.Id == idCuocHop && !ch.Deleted
                                     select ch).FirstOrDefaultAsync();

            if (cuocHopInfo == null)
            {
                throw new UserFriendlyException(404, $"Cuộc họp không tồn tại");
            }

            var danhSachDiemDanh = await (from ttdd in _smDbContext.ThongTinDiemDanhs
                                          where ttdd.IdHopTrucTuyen == idCuocHop && !ttdd.Deleted
                                          orderby ttdd.Id
                                          select ttdd).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách điểm danh");

            worksheet.Cell(1, 1).Value = $"DANH SÁCH ĐIỂM DANH - {cuocHopInfo.TenCuocHop}";
            worksheet.Range(1, 1, 1, 15).Merge();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;


            worksheet.Cell(2, 1).Value = $"Thời gian tạo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Range(2, 1, 2, 15).Merge();


            var headers = new string[]
            {
                "STT", "MSSV", "Họ Tên", "Họ đệm", "Tên", "Khoa",
                "Lớp quản lý", "Email Huce", "Điện thoại",
                "Trạng thái điểm danh", "Link meeting", "Thời gian Bắt Đầu Điểm Danh",
                "Thời Gian Kết Thúc Điểm Danh", "Thời Gian Bắt Đầu Cuộc Họp", "Thời Gian Kết Thúc Cuộc Họp"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            int rowIndex = 5;
            int stt = 1;

            foreach (var item in danhSachDiemDanh)
            {
                worksheet.Cell(rowIndex, 1).Value = stt;
                worksheet.Cell(rowIndex, 2).Value = item.MaSoSinhVien;
                worksheet.Cell(rowIndex, 3).Value = item.HoVaTen;
                worksheet.Cell(rowIndex, 4).Value = item.HoDem;
                worksheet.Cell(rowIndex, 5).Value = item.Ten;
                worksheet.Cell(rowIndex, 6).Value = item.Khoa;
                worksheet.Cell(rowIndex, 7).Value = item.LopQuanLy;
                worksheet.Cell(rowIndex, 8).Value = item.EmailHuce;
                worksheet.Cell(rowIndex, 9).Value = item.SoDienThoai;
                worksheet.Cell(rowIndex, 10).Value = ConvertTrangThaiDiemDanh(item.TrangThaiDiemDanh);
                worksheet.Cell(rowIndex, 11).Value = cuocHopInfo.LinkCuocHop ?? "";
                worksheet.Cell(rowIndex, 12).Value = cuocHopInfo.BatDauDiemDanh?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                worksheet.Cell(rowIndex, 13).Value = cuocHopInfo.KetThucDiemDanh?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                worksheet.Cell(rowIndex, 14).Value = cuocHopInfo.ThoiGianBatDau?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                worksheet.Cell(rowIndex, 15).Value = cuocHopInfo.ThoiGianKetThuc?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";

                for (int col = 1; col <= headers.Length; col++)
                {
                    var cell = worksheet.Cell(rowIndex, col);
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (col == 1 || col == 10)
                    {
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }


                    if (col == 10)
                    {
                        switch (item.TrangThaiDiemDanh)
                        {
                            case ThongTinDiemDanh.DaDiemDanh:
                                cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                                break;
                            case ThongTinDiemDanh.VangMat:
                                cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                                break;
                            case ThongTinDiemDanh.ChuaDiemDanh:
                                cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                                break;
                        }
                    }
                }

                rowIndex++;
                stt++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            worksheet.Column(1).Width = 5;  
            worksheet.Column(2).Width = 12; 
            worksheet.Column(3).Width = 25;  
            worksheet.Column(8).Width = 30;  
            worksheet.Column(10).Width = 18;
            worksheet.Column(11).Width = 50; 

            // Thống kê
            var tongSo = danhSachDiemDanh.Count;
            var daDiemDanh = danhSachDiemDanh.Count(x => x.TrangThaiDiemDanh == ThongTinDiemDanh.DaDiemDanh);
            var vangMat = danhSachDiemDanh.Count(x => x.TrangThaiDiemDanh == ThongTinDiemDanh.VangMat);
            var chuaDiemDanh = danhSachDiemDanh.Count(x => x.TrangThaiDiemDanh == ThongTinDiemDanh.ChuaDiemDanh);

            int statsRow = rowIndex + 2;
            worksheet.Cell(statsRow, 1).Value = "THỐNG KÊ:";
            worksheet.Cell(statsRow, 1).Style.Font.Bold = true;
            worksheet.Range(statsRow, 1, statsRow, 3).Merge();

            statsRow++;
            worksheet.Cell(statsRow, 1).Value = $"Tổng số sinh viên: {tongSo}";
            worksheet.Cell(statsRow, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            worksheet.Range(statsRow, 1, statsRow, 3).Merge();

            statsRow++;
            worksheet.Cell(statsRow, 1).Value = $"Đã điểm danh: {daDiemDanh}";
            worksheet.Cell(statsRow, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            worksheet.Range(statsRow, 1, statsRow, 3).Merge();

            statsRow++;
            worksheet.Cell(statsRow, 1).Value = $"Vắng mặt: {vangMat}";
            worksheet.Cell(statsRow, 1).Style.Fill.BackgroundColor = XLColor.LightPink;
            worksheet.Range(statsRow, 1, statsRow, 3).Merge();

            statsRow++;
            worksheet.Cell(statsRow, 1).Value = $"Chưa điểm danh: {chuaDiemDanh}";
            worksheet.Cell(statsRow, 1).Style.Fill.BackgroundColor = XLColor.LightYellow;
            worksheet.Range(statsRow, 1, statsRow, 3).Merge();

            // Freeze panes
            worksheet.SheetView.FreezeRows(4);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            _logger.LogInformation($"{nameof(ExportDanhSachDiemDanhToExcel)} completed for IdCuocHop: {idCuocHop}");
            return stream.ToArray();
        }
        public ViewThongKeDiemDanhResponseDto ThongKeDiemDanh(ViewThongKeDiemDanhRequestDto dto)
        {
            _logger.LogInformation($"{nameof(ThongKeDiemDanh)} dto={JsonSerializer.Serialize(dto)}");

            var cuocHop = _smDbContext.HopTrucTuyens
                .FirstOrDefault(h => h.Id == dto.IdCuocHop && !h.Deleted);

            if (cuocHop == null)
            {
                throw new UserFriendlyException(404, "Cuộc họp không tồn tại");
            }

            var query = _smDbContext.ThongTinDiemDanhs
                .Where(ttdd => ttdd.IdHopTrucTuyen == dto.IdCuocHop && !ttdd.Deleted);

            if (!string.IsNullOrWhiteSpace(dto.Filter))
            {
                var filterValue = dto.Filter.Trim();
                query = query.Where(ttdd =>
                    ttdd.LopQuanLy.Contains(filterValue) ||
                    ttdd.Khoa.Contains(filterValue));
            }

            var danhSachDiemDanh = query.ToList();

            var tongSoSinhVienThamGia = danhSachDiemDanh.Count(x => x.TrangThaiDiemDanh == ThongTinDiemDanh.DaDiemDanh);
            var tongSoSinhVienVang = danhSachDiemDanh.Count(x => x.TrangThaiDiemDanh == ThongTinDiemDanh.VangMat);

            return new ViewThongKeDiemDanhResponseDto
            {
                TongSoSinhVienThamGia = tongSoSinhVienThamGia,
                TongSoSinhVienVang = tongSoSinhVienVang
            };
        }
        private string ConvertTrangThaiDiemDanh(int trangThai)
        {
            return trangThai switch
            {
                ThongTinDiemDanh.DaDiemDanh => "Đã điểm danh",
                ThongTinDiemDanh.VangMat => "Vắng mặt",
                ThongTinDiemDanh.ChuaDiemDanh => "Chưa điểm danh",
                _ => "Không xác định"
            };
        }

        private static string BuildFullName(string surName, string givenName)
        {
            var fullName = new StringBuilder();

            if (!string.IsNullOrEmpty(surName))
                fullName.Append(surName);

            if (!string.IsNullOrEmpty(givenName))
            {
                if (fullName.Length > 0)
                    fullName.Append(" ");
                fullName.Append(givenName);
            }

            return fullName.ToString();
        }
        private string GetMaSoFromUpn(string upn)
        {
            if (string.IsNullOrWhiteSpace(upn))
                return "0";

            var parts = upn.Split('@');
            return parts.Length > 0 ? parts[0] : "0";
        }

        private string GetLopFromDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) return string.Empty;

            var parts = displayName.Split('-');
            return parts.Length >= 2 ? parts[parts.Length - 1].Trim() : string.Empty;
        }


        private string CleanHtmlContent(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent)) return string.Empty;

            var withoutHtml = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<.*?>", string.Empty);

            var decoded = System.Net.WebUtility.HtmlDecode(withoutHtml);

            decoded = decoded.Replace("&nbsp;", " ")
                            .Replace("&amp;", "&")
                            .Replace("&lt;", "<")
                            .Replace("&gt;", ">")
                            .Replace("&quot;", "\"");
            return decoded.Trim();
        }
       /* private GraphServiceClient CreateUserGraphClient(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return new GraphServiceClient(httpClient);
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
        }*/
        private static DateTime GetVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}