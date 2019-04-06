namespace MyKniga.Web.Models
{
    using System;
    using Common.Mapping.Interfaces;
    using Services.Models;
    using Services.Models.Book;

    public class PurchaseAdminListingViewModel : IMapWith<PurchaseAdminListingServiceModel>
    {
        public BookListingViewModel Book { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string UserEmail { get; set; }
    }
}