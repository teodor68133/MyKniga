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

    public class BooksService : BaseService, IBooksService
    {
        public BooksService(MyKnigaDbContext context) : base(context)
        {
        }

        public async Task<string> CreateBookAsync(BookCreateServiceModel serviceBook)
        {
            var dbBook = Mapper.Map<Book>(serviceBook);

            await this.Context.Books.AddAsync(dbBook);

            await this.Context.SaveChangesAsync();

            return dbBook.Id;
        }

        public async Task<IEnumerable<BookListingServiceModel>> GetAllBooksAsync()
        {
            var serviceBooks = await this.Context.Books
                .OrderBy(b => b.Title)
                .ProjectTo<BookListingServiceModel>()
                .ToArrayAsync();

            return serviceBooks;
        }

        public async Task<BookDetailsServiceModel> GetBookByIdAsync(string id)
        {
            var serviceBook = await this.Context.Books
                .ProjectTo<BookDetailsServiceModel>()
                .SingleOrDefaultAsync(b => b.Id == id);

            return serviceBook;
        }
    }
}