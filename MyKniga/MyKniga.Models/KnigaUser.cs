namespace MyKniga.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class KnigaUser : IdentityUser
    {
        public ICollection<Purchase> Purchases { get; set; }

        public Publisher Publisher { get; set; }
        public string PublisherId { get; set; }
    }
}