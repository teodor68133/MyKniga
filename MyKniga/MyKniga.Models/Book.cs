namespace MyKniga.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Book
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string Author { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Year { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(200)]
        public string ShortDescription { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Pages { get; set; }

        [Required]
        [MaxLength(150)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(13)]
        public string Isbn { get; set; }

        public ICollection<BookTag> BookTags { get; set; }

        public ICollection<Purchase> Purchases { get; set; }

        [Required]
        public string PublisherId { get; set; }

        public Publisher Publisher { get; set; }
    }
}