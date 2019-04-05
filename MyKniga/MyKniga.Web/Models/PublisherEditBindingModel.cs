namespace MyKniga.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using Services.Models.Publisher;

    public class PublisherEditBindingModel : IMapWith<PublisherEditServiceModel>
    {
        [Required]
        public string Id { get; set; }

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