namespace MyKniga.Services.Models.Book
{
    using Common.Mapping.Interfaces;
    using MyKniga.Models;

    public abstract class BaseBookServiceModel : IMapWith<Book>
    {
        public string Id { get; set; }
    }
}