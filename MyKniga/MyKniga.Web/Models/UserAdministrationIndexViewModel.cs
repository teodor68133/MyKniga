namespace MyKniga.Web.Models
{
    using System.Collections.Generic;

    public class UserAdministrationIndexViewModel
    {
        public IEnumerable<UserListingViewModel> Users { get; set; }

        public IEnumerable<PublisherListingViewModel> Publishers { get; set; }
    }
}