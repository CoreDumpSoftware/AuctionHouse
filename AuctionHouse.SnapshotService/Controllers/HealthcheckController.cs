using Microsoft.AspNetCore.Mvc;

namespace AuctionHouse.SnapshotService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthcheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}