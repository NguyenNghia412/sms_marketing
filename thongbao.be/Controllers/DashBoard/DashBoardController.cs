using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.DashBoard.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.DashBoard
{
    [Route("api/core/dash-board")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashBoardController : BaseController
    {
        private readonly IDashBoardService _dashBoardService;
        private readonly ILogger<DashBoardController> _logger;

        public DashBoardController(ILogger<DashBoardController> logger, IDashBoardService dashBoardService) : base(logger)
        {
            _dashBoardService = dashBoardService;
            _logger = logger;
        }
        [Permission(PermissionKeys.DashBoardView)]
        [HttpGet("thong-ke-credits-theo-nam")]
        public ApiResponse GetStatisticsUserCreditsTheoNam([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
        {
            try
            {
                var result = _dashBoardService.GetStatisticsUserCreditsTheoNam(tuNgay, denNgay);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DashBoardView)]
        [HttpGet("thong-ke-tong-so-tin-nhan-da-gui-theo-nam")]
        public ApiResponse GetStatisticsTongSoTinNhanDaGuiTheoNam([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
        {
            try
            {
                var result = _dashBoardService.GetStatisticsTongSoTinNhanDaGuiTheoNam(tuNgay, denNgay);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DashBoardView)]
        [HttpGet("thong-ke-tong-quan")]
        public ApiResponse GetStatisticsDashBoard()
        {
            try
            {
                var result = _dashBoardService.GetStatisticsDashBoard();
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.DashBoardView)]
        [HttpGet("thong-ke-credits-theo-thang-by-user")]
        public ApiResponse GetStatisticsUserCreditsTheoThangByUser([FromQuery] string userId, [FromQuery] DateTime tuThang, [FromQuery] DateTime denThang)
        {
            try
            {
                var result = _dashBoardService.GetStatisticsUserCreditsTheoThangByUser(userId, tuThang, denThang);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}