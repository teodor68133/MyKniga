namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class TagDisplayServiceModel : IMapWith<Tag>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}