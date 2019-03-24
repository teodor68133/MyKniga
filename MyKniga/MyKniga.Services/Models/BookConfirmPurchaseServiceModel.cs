namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BookConfirmPurchaseServiceModel : IMapWith<Book>
    {
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public decimal Price { get; set; }
        
        public string ImageUrl { get; set; }
    }
}