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

        public async Task<bool> AddTagToBookAsync(string bookId, string tagId)
        {
            if (!await this.Context.Tags.AnyAsync(t => t.Id == tagId) ||
                !await this.Context.Books.AnyAsync(b => b.Id == bookId) ||
                await this.Context.BookTags.AnyAsync(bt => bt.BookId == bookId && bt.TagId == tagId))
            {
                return false;
            }

            var bookTag = new BookTag
            {
                TagId = tagId,
                BookId = bookId
            };

            await this.Context.BookTags.AddAsync(bookTag);
            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task RemoveTagFromBookAsync(string bookId, string tagId)
        {
            var bookTag = await this.Context.BookTags.SingleOrDefaultAsync(bt =>
                bt.BookId == bookId && bt.TagId == tagId);

            if (bookTag == null)
            {
                return;
            }

            this.Context.BookTags.Remove(bookTag);
            await this.Context.SaveChangesAsync();
        }
    }
}