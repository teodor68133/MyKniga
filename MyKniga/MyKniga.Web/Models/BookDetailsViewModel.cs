namespace MyKniga.Web.Models
{
    using System.Collections.Generic;
    using Common.Mapping.Interfaces;
    using Services.Models.Book;

    public class BookDetailsViewModel : IMapWith<BookDetailsServiceModel>
    {
        public string Id { get; set; }

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

        public bool CanEdit { get; set; }

        public bool HasPurchased { get; set; }

        public IEnumerable<BookTagDisplayViewModel> BookTags { get; set; }

        public IEnumerable<TagDisplayViewModel> AllTags { get; set; }

        public PublisherDetailsViewModel Publisher { get; set; }
    }
}