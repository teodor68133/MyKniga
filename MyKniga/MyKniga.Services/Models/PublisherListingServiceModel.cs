namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class PublisherListingServiceModel : IMapWith<Publisher>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}