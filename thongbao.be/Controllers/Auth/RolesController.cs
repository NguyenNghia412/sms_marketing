using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.Controllers.Base;

namespace thongbao.be.Controllers.Auth
{
    [Route("api/app/roles")]
    [ApiController]
    public class RolesController : BaseController
    {
        public RolesController(ILogger<BaseController> logger) : base(logger)
        {
        }
    }
}
