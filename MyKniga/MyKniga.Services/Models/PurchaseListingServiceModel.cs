namespace MyKniga.Services.Models
{
    using System;
    using Book;
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class PurchaseListingServiceModel : IMapWith<Purchase>
    {
        public BookListingServiceModel Book { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string BookDownloadUrl { get; set; }
    }
}