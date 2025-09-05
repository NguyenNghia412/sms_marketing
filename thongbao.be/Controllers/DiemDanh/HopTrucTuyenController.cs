using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.DiemDanh.Dtos;
using thongbao.be.application.DiemDanh.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;
using thongbao.be.shared.HttpRequest.Exception;

namespace thongbao.be.Controllers.DiemDanh
{
    [Route("api/core/hop-truc-tuyen")]
    [ApiController]
    [Authorize]

    public class HopTrucTuyenController : BaseController
    {

       private readonly IHopTrucTuyenService _hopTrucTuyenService;
        private static readonly Dictionary<string, string> _pkceStorage = new();
        public HopTrucTuyenController(ILogger<HopTrucTuyenController> logger, IHopTrucTuyenService hopTrucTuyenService): base(logger)
        {
            _hopTrucTuyenService = hopTrucTuyenService;
        }


        [Permission(PermissionKeys.HopTrucTuyenAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateCuocHopDto dto)
        {
            try
            {
                 _hopTrucTuyenService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingCuocHopDto dto)
        {
            try
            {
                var data = _hopTrucTuyenService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [HttpGet("microsoft-auth-url")]
        public ApiResponse GetMicrosoftAuthUrl()
        {
            try
            {
                var authResponse = _hopTrucTuyenService.GenerateMicrosoftAuthUrl();

                _pkceStorage[authResponse.State] = authResponse.CodeVerifier;
                return new(new
                {
                    authUrl = authResponse.AuthUrl,
                    state = authResponse.State
                });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [HttpGet("callback")]
        [AllowAnonymous] 
        public async Task<ApiResponse> MicrosoftCallback([FromQuery] GraphApiCallbackDto dto)
        {
            try
            {
                if (!_pkceStorage.TryGetValue(dto.State, out var codeVerifier))
                {
                    throw new UserFriendlyException(400, "Tham số state không hợp lệ hoặc hết hạn");
                }

                _pkceStorage.Remove(dto.State);

                var callbackDto = new GraphApiCallbackDto
                {
                    Code = dto.Code,
                    State = dto.State,
                    CodeVerifier = codeVerifier 
                };

                var tokenResponse = await _hopTrucTuyenService.HandleMicrosoftCallback(callbackDto);
                return new(tokenResponse);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("user-info")]
        public async Task<ApiResponse> GetUserInfo([FromBody] GraphApiGetUserInfoRequestDto dto)
        {
            try
            {
                var userInfo = await _hopTrucTuyenService.GetUserInfo(dto.AccessToken);

                return new(userInfo);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("thong-tin-cuoc-hop")]
        public async Task<ApiResponse> GetThongTinCuocHop([FromQuery] GraphApiGetThongTinCuocHopDto dto, [FromBody] GraphApiGetUserInfoRequestDto input)
        {
            try
            {
                var meetingInfo = await _hopTrucTuyenService.GetAndSaveMeetingInfo(
                    new GraphApiGetThongTinCuocHopDto
                    {
                        JoinWebUrl = dto.JoinWebUrl,
                        IdCuocHop = dto.IdCuocHop
                    },
                    input.AccessToken
                );
                return new(meetingInfo);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenUpdate)]
        [HttpPut("trang-thai-diem-danh")]
        public async Task<ApiResponse> UpdateTrangThaiDiemDanhAsync([FromBody] UpdateTrangThaiDiemDanhDto dto)
        {
            try
            {
                await _hopTrucTuyenService.UpdateTrangThaiDiemDanh(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        }
}
