namespace MyKniga.Models
{
    public class BookTag
    {
        public Book Book { get; set; }
        public string BookId { get; set; }

        public Tag Tag { get; set; }
        public string TagId { get; set; }
    }
}