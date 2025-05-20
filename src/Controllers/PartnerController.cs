using Microsoft.AspNetCore.Mvc;

namespace TreeJournalApi.Controllers
{
    [ApiController]
    [Route("api.user.partner")]
    public class PartnerController : ControllerBase
    {
        [HttpPost("rememberMe")]
        public IActionResult RememberMe([FromQuery] string code)
        {
            return Ok(new { message = $"Partner code received: {code}" });
        }
    }
}
