namespace MyKniga.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Models;

    public class PurchasesService : BaseService, IPurchasesService
    {
        public PurchasesService(MyKnigaDbContext context) : base(context)
        {
        }

        public async Task<bool> CreateAsync(PurchaseCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);

            // Verify that the user exists, the book exists, and that the book has not already been purchased by the user
            if (user == null ||
                !await this.Context.Books.AnyAsync(b => b.Id == model.BookId) ||
                await this.Context.Purchases.AnyAsync(bt => bt.BookId == model.BookId && bt.UserId == user.Id))
            {
                return false;
            }

            var purchase = Mapper.Map<Purchase>(model);
            purchase.UserId = user.Id;

            await this.Context.Purchases.AddAsync(purchase);

            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PurchaseListingServiceModel>> GetPurchasesForUserAsync(string userName)
        {
            var purchasedBooks = await this.Context.Purchases
                .Where(p => p.User.UserName == userName)
                .OrderByDescending(p => p.PurchaseDate)
                .ProjectTo<PurchaseListingServiceModel>()
                .ToArrayAsync();

            return purchasedBooks;
        }

        public async Task<bool> UserHasPurchasedBookAsync(string bookId, string userName)
        {
            if (userName == null || bookId == null)
            {
                return false;
            }

            var isPurchased = await this.Context.Purchases.AnyAsync(p =>
                p.BookId == bookId && p.User.UserName == userName);

            return isPurchased;
        }
    }
}