using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.GuiTinNhan.Dtos;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.HttpRequest;

namespace thongbao.be.Controllers
{
    [Route("api/core/chien-dich")]
    [ApiController]
    public class ChienDichController : BaseController
    {
        private readonly IChienDichService _chienDichService;

        public ChienDichController(ILogger<ChienDichController> logger, IChienDichService chienDichService) : base(logger)
        {
            _chienDichService = chienDichService;
        }

        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingChienDichDto dto)
        {
            try
            {
                var data = _chienDichService.Find(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateChienDichDto dto)
        {
            try
            {
                _chienDichService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
