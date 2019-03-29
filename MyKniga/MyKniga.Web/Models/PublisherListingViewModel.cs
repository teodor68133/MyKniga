namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class PublisherListingViewModel : IMapWith<PublisherListingServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}