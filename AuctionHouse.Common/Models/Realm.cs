using System;

namespace AuctionHouse.Common.Models
{
    public class Realm
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Realm realm &&
                   Name == realm.Name &&
                   Slug == realm.Slug;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Slug);
        }
    }
}
