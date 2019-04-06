namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IPurchasesService
    {
        Task<bool> CreateAsync(PurchaseCreateServiceModel model);
        Task<IEnumerable<PurchaseListingServiceModel>> GetPurchasesForUserAsync(string userName);
        Task<bool> UserHasPurchasedBookAsync(string bookId, string userName);
        Task<IEnumerable<PurchaseAdminListingServiceModel>> GetPurchasesForPublisherAsync(string publisherId);
        Task<IEnumerable<PurchaseAdminListingServiceModel>> GetAllPurchasesAsync();
    }
}