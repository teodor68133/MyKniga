namespace MyKniga.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Publisher
    {
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

        public ICollection<KnigaUser> Users { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}