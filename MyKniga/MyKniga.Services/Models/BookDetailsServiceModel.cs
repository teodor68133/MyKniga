namespace MyKniga.Services.Models
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public class BookDetailsServiceModel : IMapWith<Book>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }
        
        public decimal Price { get; set; }
        
        public int Year { get; set; }
        
        public string Description { get; set; }
        
        public string ShortDescription { get; set; }
        
        public int Pages { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string Isbn { get; set; }
    }
}