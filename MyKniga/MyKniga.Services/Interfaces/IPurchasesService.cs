namespace MyKniga.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models;

    public interface IPurchasesService
    {
        Task<bool> CreateAsync(PurchaseCreateServiceModel model);
    }
}