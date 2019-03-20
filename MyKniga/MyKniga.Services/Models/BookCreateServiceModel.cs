namespace MyKniga.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BookCreateServiceModel : IMapWith<Book>
    {
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string Author { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "10000")]
        public decimal Price { get; set; }
    }
}