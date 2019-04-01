namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Book;

    public interface IBooksService
    {
        Task<string> CreateBookAsync(BookCreateServiceModel model);
        Task<IEnumerable<T>> GetAllBooksAsync<T>() where T : BaseBookServiceModel;
        Task<T> GetBookByIdAsync<T>(string id) where T : BaseBookServiceModel;
        Task<bool> AddTagToBookAsync(string bookId, string tagId);
        Task RemoveTagFromBookAsync(string bookId, string tagId);
        Task<bool> DeleteBookAsync(string bookId);
    }
}