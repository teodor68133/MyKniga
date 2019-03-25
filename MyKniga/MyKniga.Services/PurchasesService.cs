namespace MyKniga.Services
{
    using System.Collections;
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

        public async Task<IEnumerable<PurchaseListingServiceModel>> GetPurchasesForUser(string userName)
        {
            var purchasedBooks = await this.Context.Purchases
                .Where(p => p.User.UserName == userName)
                .OrderByDescending(p => p.PurchaseDate)
                .ProjectTo<PurchaseListingServiceModel>()
                .ToArrayAsync();

            return purchasedBooks;
        }
    }
}