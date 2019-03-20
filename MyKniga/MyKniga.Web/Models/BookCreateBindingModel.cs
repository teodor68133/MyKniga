namespace MyKniga.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class BookCreateBindingModel : IMapWith<BookCreateServiceModel>
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