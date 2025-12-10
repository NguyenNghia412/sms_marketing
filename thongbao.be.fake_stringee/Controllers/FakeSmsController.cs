using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace thongbao.be.fake_stringee.Controllers
{
    [Route("sms")]
    [ApiController]
    public class FakeSmsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendSmsFake([FromBody] object dto)
        {
            await Task.Delay(60000); // 60 seconds

            Random rnd = new Random();
            int number = rnd.Next(1, 11);

            if (number <= 3)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, "The server timed out while processing the request.");
            }

            var result = new object[] { new { r = 0, msg = "Success" } };

            return Ok(new
            {
                result
            });
        }
    }
}
