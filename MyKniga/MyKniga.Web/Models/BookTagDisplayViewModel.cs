namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class BookTagDisplayViewModel : IMapWith<BookTagDisplayServiceModel>
    {
        public string TagId { get; set; }
        public TagDisplayViewModel Tag { get; set; }
    }
}