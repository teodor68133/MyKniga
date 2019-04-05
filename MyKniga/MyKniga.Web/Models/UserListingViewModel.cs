namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class UserListingViewModel : IMapWith<KnigaUser>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PublisherId { get; set; }

        public string PublisherName { get; set; }

        public bool IsAdmin { get; set; }
    }
}