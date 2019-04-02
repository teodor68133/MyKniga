namespace MyKniga.Services.Models
{
    public class BookFilteringServiceModel
    {
        public string TagId { get; set; }
        public string PublisherId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}