using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.HttpRequest;
using thongbao.be.infrastructure.external.SignalR.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace thongbao.be.Controllers.Demo
{
    [Route("api/demo/signalr")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DemoSignalRController : BaseController
    {
        private readonly IDemoSignalRService _demoSignalRService;

        public DemoSignalRController(ILogger<DemoSignalRController> logger, IDemoSignalRService demoSignalRService) : base(logger)
        {
            _demoSignalRService = demoSignalRService;
        }

        /// <summary>
        /// Gửi thông báo tới tất cả clients
        /// </summary>
        [AllowAnonymous]
        [HttpPost("send-to-all")]
        public ApiResponse SendToAll([FromQuery] string title, [FromQuery] string message)
        {
            try
            {
                _demoSignalRService.SendNotificationToAll(title, message);
                return new(new { success = true, message = "Sent to all clients" });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Gửi thông báo tới group
        /// </summary>
        [AllowAnonymous]
        [HttpPost("send-to-group")]
        public ApiResponse SendToGroup([FromQuery] string groupName, [FromQuery] string title, [FromQuery] string message)
        {
            try
            {
                _demoSignalRService.SendNotificationToGroup(groupName, title, message);
                return new(new { success = true, message = $"Sent to group: {groupName}" });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Gửi thông báo tới user cụ thể
        /// </summary>
        [AllowAnonymous]
        [HttpPost("send-to-user")]
        public ApiResponse SendToUser([FromQuery] string userId, [FromQuery] string title, [FromQuery] string message)
        {
            try
            {
                _demoSignalRService.SendNotificationToUser(userId, title, message);
                return new(new { success = true, message = $"Sent to user: {userId}" });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy số lượng connection hiện tại
        /// </summary>
        [AllowAnonymous]
        [HttpGet("connection-count")]
        public async Task<ApiResponse> GetConnectionCount()
        {
            try
            {
                var count = await _demoSignalRService.GetConnectionCount();
                return new(new { connectionCount = count });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Test basic SignalR connection
        /// </summary>
        [AllowAnonymous]
        [HttpPost("test")]
        public ApiResponse TestSignalR()
        {
            try
            {
                _demoSignalRService.SendNotificationToAll("Test", "SignalR is working!");
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}