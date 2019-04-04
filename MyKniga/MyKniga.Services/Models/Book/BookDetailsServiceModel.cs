namespace MyKniga.Services.Models.Book
{
    using System.Collections.Generic;

    public class BookDetailsServiceModel : BaseBookServiceModel
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public int Pages { get; set; }

        public string ImageUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string Isbn { get; set; }

        public string PublisherId { get; set; }

        public IEnumerable<BookTagDisplayServiceModel> BookTags { get; set; }
    }
}