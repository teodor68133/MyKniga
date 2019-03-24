namespace MyKniga.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class TagCreateServiceModel : IMapWith<Tag>
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}