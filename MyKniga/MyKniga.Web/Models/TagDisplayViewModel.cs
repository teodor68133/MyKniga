namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class TagDisplayViewModel : IMapWith<TagDisplayServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}