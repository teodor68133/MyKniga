namespace MyKniga.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class PurchaseCreateServiceModel : IMapWith<Purchase>
    {
        [Required]
        public string BookId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }
    }
}