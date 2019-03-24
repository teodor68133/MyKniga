namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IBooksService
    {
        Task<string> CreateBookAsync(BookCreateServiceModel model);
        Task<IEnumerable<BookListingServiceModel>> GetAllBooksAsync();
        Task<BookDetailsServiceModel> GetBookByIdAsync(string id);
        Task<bool> AddTagToBookAsync(string bookId, string tagId);
        Task RemoveTagFromBookAsync(string bookId, string tagId);
    }
}