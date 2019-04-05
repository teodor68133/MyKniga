namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models.Publisher;

    public class PublisherListingViewModel : IMapWith<PublisherListingServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}