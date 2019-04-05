namespace MyKniga.Services.Models.Publisher
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BasePublisherServiceModel : IMapWith<Publisher>
    {
        public string Id { get; set; }
    }
}