using AuctionHouse.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouse.ItemService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        // GET: api/Item
        [HttpGet]
        public async Task<IEnumerable<WowItem>> GetAsync([FromQuery(Name = "itemIds")]string csvItemIds)
        {
            var itemIds = csvItemIds.Split(',').ToArray();
            return Enumerable.Empty<WowItem>();
        }

        [HttpGet("{itemId}", Name = "refresh")]
        public async Task<WowItem> Get(int itemId)
        {
            throw new System.Exception("Asdf");
        }
    }
}
