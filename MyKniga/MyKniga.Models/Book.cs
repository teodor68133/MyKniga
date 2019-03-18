namespace MyKniga.Models
{
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
    }
}