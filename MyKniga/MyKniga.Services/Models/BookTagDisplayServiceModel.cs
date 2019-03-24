namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BookTagDisplayServiceModel : IMapWith<BookTag>
    {
        public string TagId { get; set; }
        public TagDisplayServiceModel Tag { get; set; }
    }
}