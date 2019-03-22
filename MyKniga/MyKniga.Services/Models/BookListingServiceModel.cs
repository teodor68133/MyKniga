namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BookListingServiceModel : IMapWith<Book>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}