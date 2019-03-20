namespace MyKniga.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Data;
    using Interfaces;
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
    }
}