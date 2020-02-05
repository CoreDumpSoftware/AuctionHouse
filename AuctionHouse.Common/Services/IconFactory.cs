using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using Flurl;

namespace AuctionHouse.Common.Services
{
    public class IconFactory : IIconFactory
    {
        private const string baseUrl = "https://wow.zamimg.com/images/wow/icons/";
        private const string defaultExtension = "jpg";


        public Icon GetIcon(string iconName)
        {
            return new Icon
            {
                Name = iconName,
                LargeIconUrl = GetUrlForSize(iconName, "large"),
                MediumIconUrl = GetUrlForSize(iconName, "medium"),
                SmallIconUrl = GetUrlForSize(iconName, "small"),
            };
        }

        private string GetUrlForSize(string iconName, string size)
        {
            return Url.Combine(baseUrl, size, $"{iconName}.{defaultExtension}");
        }
    }
}
