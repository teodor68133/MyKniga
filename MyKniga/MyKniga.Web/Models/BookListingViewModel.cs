namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models.Book;

    public class BookListingViewModel : IMapWith<BookListingServiceModel>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}