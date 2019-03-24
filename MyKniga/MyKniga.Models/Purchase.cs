namespace MyKniga.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Purchase
    {
        public string BookId { get; set; }
        public Book Book { get; set; }

        public string UserId { get; set; }
        public KnigaUser User { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }
    }
}