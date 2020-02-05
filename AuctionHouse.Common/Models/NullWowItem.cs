using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Models
{
    public class NullWowItem : WowItem
    {
        public const string NullItemName = "DNE";

        public NullWowItem() : base()
        {
            Name = NullItemName;
            QualityId = -1;
            SubclassId = -1;
            ClassId = -1;
        }
    }
}
