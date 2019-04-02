namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Models.Book;

    public interface IBooksService
    {
        Task<string> CreateBookAsync(BookCreateServiceModel model);
        Task<IEnumerable<T>> GetAllBooksAsync<T>() where T : BaseBookServiceModel;
        Task<T> GetBookByIdAsync<T>(string id) where T : BaseBookServiceModel;
        Task<bool> AddTagToBookAsync(string bookId, string tagId);
        Task<bool> RemoveTagFromBookAsync(string bookId, string tagId);
        Task<bool> DeleteBookAsync(string bookId);
        Task<IEnumerable<T>> GetBooksByFilteringAsync<T>(BookFilteringServiceModel model)
            where T : BaseBookServiceModel;
    }
}