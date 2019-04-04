namespace MyKniga.Web.Models
{
    using System;
    using Common.Mapping.Interfaces;
    using Services.Models;
    using Services.Models.Book;

    public class PurchaseListingViewModel : IMapWith<PurchaseListingServiceModel>
    {
        public BookListingServiceModel Book { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string BookDownloadUrl { get; set; }
    }
}