using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class HopTrucTuyenController : BaseController
    {

        private readonly IHopTrucTuyenService _hopTrucTuyenService;
        //private static readonly Dictionary<string, string> _pkceStorage = new();
        public HopTrucTuyenController(ILogger<HopTrucTuyenController> logger, IHopTrucTuyenService hopTrucTuyenService) : base(logger)
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
        [Permission(PermissionKeys.HopTrucTuyenUpdate)]
        [HttpPut("")]

        public ApiResponse Update([FromQuery] int idCuocHop, [FromBody] UpdateCuochopDto dto)
        {
            try
            {
                _hopTrucTuyenService.Update(idCuocHop, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenDelete)]
        [HttpDelete("")]
        public ApiResponse Delete([FromQuery] int idCuocHop)
        {
            try
            {
                _hopTrucTuyenService.Delete(idCuocHop);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpGet("thong-tin-diem-danh")]
        public ApiResponse ThongTinDiemDanhPaging([FromQuery] int idCuocHop, [FromQuery] FindPagingThongTinDiemDanhDto dto)
        {
            try
            {
                var data = _hopTrucTuyenService.ThongTinDiemDanhPaging(idCuocHop, dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        /*[HttpGet("microsoft-auth-url")]
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
        }*/

        [HttpGet("user-info")]
        public async Task<ApiResponse> GetUserInfo([FromQuery] string userEmailHuce)
        {
            try
            {
                var userInfo = await _hopTrucTuyenService.GetUserInfo(userEmailHuce);

                return new(userInfo);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [HttpPost("thong-tin-cuoc-hop")]
        public async Task<ApiResponse> GetThongTinCuocHop([FromQuery] GraphApiGetThongTinCuocHopDto dto)
        {
            try
            {
                var userId = await _hopTrucTuyenService.GetUserIdByEmailAsync(dto.EmailOrganizer);

                var meetingInfo = await _hopTrucTuyenService.GetAndSaveMeetingInfo(
                    dto, userId
                );

                return new(meetingInfo);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenUpdate)]
        [HttpPatch("trang-thai-diem-danh")]
        public async Task<ApiResponse> UpdateTrangThaiDiemDanhAsync([FromQuery] int idCuocHop, [FromBody] UpdateTrangThaiDiemDanhDto dto)
        {
            try
            {
                await _hopTrucTuyenService.UpdateTrangThaiDiemDanh(idCuocHop, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpGet("export-excel")]
        public async Task<IActionResult> DownloadDanhSachDiemDanhExcel([FromQuery] int idCuocHop)
        {
            try
            {
                var excelData = await _hopTrucTuyenService.ExportDanhSachDiemDanhToExcel(idCuocHop);
                var fileName = $"Danh_sach_diem_danh_Ms_Team_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    excelData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpGet("thong-ke-diem-danh")]
        public ApiResponse ThongKeDiemDanh([FromQuery] ViewThongKeDiemDanhRequestDto dto)
        {
            try
            {
                var data = _hopTrucTuyenService.ThongKeDiemDanh(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenAdd)]
        [HttpPost("dot-diem-danh")]
        public ApiResponse CreateDotDiemDanh([FromQuery] int idCuocHop,[FromBody] CreateDotDiemDanhDto dto)
        {
            try
            {
                _hopTrucTuyenService.CreateDotDiemDanh(idCuocHop,dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
            
        }
        [Permission(PermissionKeys.HopTrucTuyenUpdate)]
        [HttpPut("dot-diem-danh")]
        public ApiResponse UpdateDotDiemDanh([FromQuery] int idCuocHop, [FromQuery] int idDotDiemDanh, [FromBody] UpdateDotDiemDanhDto dto)
        {
            try
            {
                _hopTrucTuyenService.UpdateDotDiemDanh(idCuocHop, idDotDiemDanh, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenDelete)]
        [HttpDelete("dot-diem-danh")]
        public ApiResponse DeleteDotDiemDanh([FromQuery] int idCuocHop, [FromQuery] int idDotDiemDanh)
        {
            try
            {
                _hopTrucTuyenService.DeleteDotDiemDanh(idCuocHop, idDotDiemDanh);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpPost("dot-diem-danh/{idDotDiemDanh}/qr-code")]
        public IActionResult GetQrCodeImage(int idDotDiemDanh)
        {
            try
            {
                var qrImageBytes = _hopTrucTuyenService.GenerateQrCodeImageForDiemDanh(idDotDiemDanh);
                return File(qrImageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
        [Permission(PermissionKeys.HopTrucTuyenView)]
        [HttpGet("dot-diem-danh/{idDotDiemDanh}/qr-code/download")]
        public IActionResult DownloadQrCodeImage(int idDotDiemDanh)
        {
            try
            {
                var qrImageBytes = _hopTrucTuyenService.GenerateQrCodeImageForDiemDanh(idDotDiemDanh);
                var fileName = $"QR_DiemDanh_{idDotDiemDanh}_{DateTime.Now:yyyyMMdd}.png";

                return File(qrImageBytes, "image/png", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }

    }
}
