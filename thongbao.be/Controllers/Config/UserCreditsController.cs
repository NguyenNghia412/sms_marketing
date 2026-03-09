using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.Config.Dtos.NhaMang;
using thongbao.be.application.Config.Dtos.UserCredits;
using thongbao.be.application.Config.Implements;
using thongbao.be.application.Config.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers.Config
{
    [Route("api/core/user-credits")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserCreditsController: BaseController
    {
        private readonly IUserCreditsService _userCreditsService;

        public UserCreditsController(ILogger<UserCreditsController> logger, IUserCreditsService userCreditsService) : base(logger)
        {
            _userCreditsService = userCreditsService;
        }

        [Permission(PermissionKeys.UserCreditsAdd)]
        [HttpPost("")]
        public ApiResponse AddNhaMang(AddUserCreditsDto dto)
        {
            try
            {
                _userCreditsService.AddUserCredits(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.UserCreditsUpdate)]
        [HttpPut("")]
        public ApiResponse UpdateNhaMang(UpdateUserCreditsDto dto)
        {
            try
            {
                _userCreditsService.UpdateUserCredits(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsDelete)]
        [HttpDelete("{id}")]
        public ApiResponse DeleteNhaMang([FromRoute] int id)
        {
            try
            {
                _userCreditsService.DeleteUserCredits(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsView)]
        [HttpGet("")]
        public ApiResponse FindPaging([FromQuery] FindPagingDto dto)
        {
            try
            {
                var data = _userCreditsService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsView)]
        [HttpGet("by-id/{id}")]
        public ApiResponse GetById([FromRoute] int id)
        {
            try
            {
                var data = _userCreditsService.FindById(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsView)]
        [HttpGet("{id}/don-vi")]
        public ApiResponse GetDonVi([FromRoute] int id)
        {
            try
            {
                var data = _userCreditsService.GetDonVi(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsViewForUser)]
        [HttpGet("user")]
        public ApiResponse FindPagingByUserId([FromQuery] FindPagingByUserIdDto dto)
        {
            try
            {
                var data = _userCreditsService.FindPagingByUserId(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [HttpGet("user-credits")]
        public ApiResponse GetCurrentUserCredits()
        {
            try
            {
                var data = _userCreditsService.GetCurrentUserCredits();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.UserCreditsView)]
        [HttpGet("{id}/toi-da-han-muc-credits-gia-han")]
        public ApiResponse GetToiDaHanMucCreditsGiaHan([FromRoute] int id)
        {
            try
            {
                var data = _userCreditsService.GetToiDaHanMucCreditsGiaHan(id);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }


        [Permission(PermissionKeys.UserCreditsUpdate)]
        [HttpPut("toi-da-han-muc-credits-gia-han")]
        public ApiResponse UpdateToiDaHanMucCreditsGiaHan([FromBody] UpdateToiDaHanMucCreditsGiaHanDto dto)
        {
            try
            {
                _userCreditsService.UpdateToiDaHanMucCreditsGiaHan(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
