namespace MyKniga.Services.Models.Book
{
    public class BookListingServiceModel : BaseBookServiceModel
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}