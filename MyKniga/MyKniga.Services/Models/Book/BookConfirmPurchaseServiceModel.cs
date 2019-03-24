namespace MyKniga.Services.Models.Book
{
    public class BookConfirmPurchaseServiceModel : BaseBookServiceModel
    {
        public string Title { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}