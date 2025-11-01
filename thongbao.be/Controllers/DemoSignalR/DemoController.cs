using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.HttpRequest;
using thongbao.be.infrastructure.external.SignalR.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace thongbao.be.Controllers.Demo
{
    [Route("api/demo/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DemoController : BaseController
    {
        public DemoController(ILogger<DemoSignalRController> logger) : base(logger)
        {
            
        }

        [AllowAnonymous]
        [HttpPost("check-timeout")]
        public async Task<ApiResponse> CheckTimeout()
        {
            try
            {
                // Wait for 10 minutes (600,000 milliseconds)
                await Task.Delay(TimeSpan.FromMinutes(10));
                return new(new { success = true, message = "success" });
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}