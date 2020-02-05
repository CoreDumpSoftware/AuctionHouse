using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using Flurl;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AuctionHouse.Common.Services
{
    public class WowheadService : IWowheadService
    {
        private const string questionMarkIcon = "inv_misc_questionMark";

        private readonly HttpClient _httpClient;
        private readonly string _wowheadUrl;
        private readonly IIconFactory _iconFactory;

        public WowheadService(HttpClient httpClient, IIconFactory iconFactory, string wowheadUrl)
        {
            _httpClient = httpClient;
            _wowheadUrl = wowheadUrl;
            _iconFactory = iconFactory;
        }

        public async Task<WowItem> GetItemAsync(int itemId)
        {
            var json = await GetItemJsonAsync(itemId);
            return json != null
                ? JsonConvert.DeserializeObject<WowItem>(json)
                : new NullWowItem()
                {
                    Id = itemId
                };
        }

        public async Task<string> GetItemJsonAsync(int itemId)
        {
            var url = Url.Combine(_wowheadUrl, $"/item={itemId}?xml");
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var xmlString = await response.Content.ReadAsStringAsync();
            var xmlRoot = XElement.Parse(xmlString);
            var value = xmlRoot.XPathSelectElement("/item/json")?.Value;
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var iconName = xmlRoot.XPathSelectElement("/item/icon")?.Value ?? questionMarkIcon;
            var icon = _iconFactory.GetIcon(iconName);
            var iconJson = JsonConvert.SerializeObject(icon);

            var builder = new StringBuilder();
            builder.Append('{');
            builder.Append(value);
            builder.Append($",\"icon\":{iconJson}");
            builder.Append('}');
            return builder.ToString();
        }
    }
}
