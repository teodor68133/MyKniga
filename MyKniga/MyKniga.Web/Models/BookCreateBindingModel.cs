namespace MyKniga.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using Services.Models.Book;

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

        [Required]
        [Range(0, int.MaxValue)]
        public int Year { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Pages { get; set; }

        [Required]
        [MaxLength(150)]
        [Url]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(150)]
        [Url]
        [Display(Name = "Download URL")]
        public string DownloadUrl { get; set; }

        [Required]
        [MaxLength(13)]
        [RegularExpression(@"^\d{13}$")]
        [Display(Name = "ISBN")]
        public string Isbn { get; set; }
    }
}