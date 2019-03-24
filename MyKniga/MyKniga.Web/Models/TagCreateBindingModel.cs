namespace MyKniga.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TagCreateBindingModel
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}