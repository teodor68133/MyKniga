namespace MyKniga.Web.Models
{
    using Common.Mapping.Interfaces;
    using Services.Models;

    public class BookFilteringBindingModel : IMapWith<BookFilteringServiceModel>
    {
        public string TagId { get; set; }
        public string PublisherId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}