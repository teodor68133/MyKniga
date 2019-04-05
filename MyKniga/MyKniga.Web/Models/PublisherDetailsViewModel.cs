namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models.Publisher;

    public class PublisherDetailsViewModel : IMapWith<PublisherDetailsServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}