using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.Controllers.Base;
using thongbao.be.lib.Stringee.Interfaces;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.ProfileStringee
{
    [Route("api/core/profile-stringee")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        public ProfileController(
            ILogger<ProfileController> logger,
            IProfileService profileService) : base(logger)
        {
            _profileService = profileService;
        }
    
        [HttpGet("")]
        public async Task<ApiResponse> GetProfileStringeeInfor()
        {
            try
            {
                var data = await _profileService.GetProfileStringeeInfor();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [HttpGet("exchange-rate-vnd")]
        public async Task<ApiResponse> GetExchangeRate()
            {
            try
            {
                var data = await _profileService.GetExchangeRate();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
