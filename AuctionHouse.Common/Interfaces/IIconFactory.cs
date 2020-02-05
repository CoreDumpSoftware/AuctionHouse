using AuctionHouse.Common.Models;

namespace AuctionHouse.Common.Interfaces
{
    public interface IIconFactory
    {
        Icon GetIcon(string iconName);
    }
}
