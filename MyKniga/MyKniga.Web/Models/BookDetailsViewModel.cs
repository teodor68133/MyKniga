namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class BookDetailsViewModel : IMapWith<BookDetailsServiceModel>
    {
        public string Id { get; set; }
        
        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }
    }
}