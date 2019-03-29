namespace MyKniga.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class PublisherCreateBindingModel : IMapWith<PublisherCreateServiceModel>
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