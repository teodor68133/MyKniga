namespace MyKniga.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class PublisherCreateServiceModel : IMapWith<Publisher>
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string ImageUrl { get; set; }
    }
}