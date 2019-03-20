namespace MyKniga.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models;

    public interface IBooksService
    {
        Task<string> CreateBookAsync(BookCreateServiceModel model);
    }
}