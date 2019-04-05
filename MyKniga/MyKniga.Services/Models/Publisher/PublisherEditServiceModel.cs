namespace MyKniga.Services.Models.Publisher
{
    using System.ComponentModel.DataAnnotations;

    public class PublisherEditServiceModel : BasePublisherServiceModel
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        [Url]
        public string ImageUrl { get; set; }
    }
}