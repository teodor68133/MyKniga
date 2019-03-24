namespace MyKniga.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public ICollection<BookTag> BookTags { get; set; }
    }
}