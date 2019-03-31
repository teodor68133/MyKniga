namespace MyKniga.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;
    using Services.Models.Book;
    using Xunit;

    public class BooksServiceTests : BaseTests
    {
        [Fact]
        public async Task CreateBookAsync_WithCorrectModel_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var booksService = new BooksService(context);

            var serviceModel = new BookCreateServiceModel
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Price = 1,
                Year = 2000,
                Description = "TestDescription",
                ShortDescription = "TestShortDescription",
                Pages = 1,
                ImageUrl = "http://example.com/favicon.ico",
                Isbn = "0000005000000",
                PublisherId = Guid.NewGuid().ToString()
            };

            // Act
            var createdId = await booksService.CreateBookAsync(serviceModel);

            // Assert
            Assert.NotNull(createdId);
            Assert.Equal(1, await context.Books.CountAsync());
            Assert.True(await context.Books.AnyAsync(b => b.Id == createdId));
        }

        [Fact]
        public async Task CreateBookAsync_WithIncorrectModel_ReturnsNull()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();


            var serviceModel = new BookCreateServiceModel
            {
                Title = "TestTitle",
                Author = "TestAuthor",
                Price = -300, // Invalid Price
                Year = 2000,
                Description = "TestDescription",
                ShortDescription = "TestShortDescription",
                Pages = 1,
                ImageUrl = "http://example.com/favicon.ico",
                Isbn = "0000005000000",
                PublisherId = Guid.NewGuid().ToString()
            };

            var booksService = new BooksService(context);

            // Act
            var createdId = await booksService.CreateBookAsync(serviceModel);

            // Assert
            Assert.Null(createdId);
            Assert.False(await context.Books.AnyAsync());
        }

        [Fact]
        public async Task GetAllBooksAsync_WithBooks_WorksCorrectly()
        {
            // Arrange
            var expectedResult = new[] {"Book1", "Book2"};
            var context = this.NewInMemoryDatabase();

            await context.Books.AddRangeAsync(new[]
            {
                new Book
                {
                    Title = "Book2"
                },
                new Book
                {
                    Title = "Book1"
                }
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var actualResult = (await booksService.GetAllBooksAsync<BookListingServiceModel>())
                .Select(b => b.Title)
                .ToArray();

            // Assert
            Assert.Equal(2, actualResult.Length);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAllBooksAsync_WithoutBooks_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);

            // Act
            var actualResult = await booksService.GetAllBooksAsync<BookListingServiceModel>();

            // Assert
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task GetBookByIdAsync_WithCorrectId_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var expectedBook = new Book {Title = "Book1"};

            await context.Books.AddRangeAsync(expectedBook, new Book
            {
                Title = "Book2"
            });

            await context.SaveChangesAsync();

            // Act
            var actualBook = await booksService.GetBookByIdAsync<BookDetailsServiceModel>(expectedBook.Id);

            // Assert
            Assert.NotNull(actualBook);
            Assert.Equal(expectedBook.Title, actualBook.Title);
        }
        
        [Fact]
        public async Task GetBookByIdAsync_WithIncorrectId_ReturnsNull()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var testId = Guid.NewGuid().ToString();

            await context.Books.AddAsync(new Book {Title = "Book1"});

            await context.SaveChangesAsync();

            // Act
            var result = await booksService.GetBookByIdAsync<BookDetailsServiceModel>(testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddTagToBookAsync_WithCorrectIds_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var tag = new Tag();
            var book = new Book();

            await context.Tags.AddAsync(tag);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            // Act
            var result = await booksService.AddTagToBookAsync(book.Id, tag.Id);
            
            // Assert
            Assert.True(result);
            Assert.True(await context.BookTags.AnyAsync(bt => bt.BookId == book.Id && bt.TagId == tag.Id));
        }
        
        [Fact]
        public async Task AddTagToBookAsync_WithIncorrectTagId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var tag = new Tag();
            var book = new Book();
            var nonexistentTagId = Guid.NewGuid().ToString();

            await context.Tags.AddAsync(tag);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            // Act
            var result = await booksService.AddTagToBookAsync(book.Id, nonexistentTagId);
            
            // Assert
            Assert.False(result);
            Assert.False(await context.BookTags.AnyAsync(bt => bt.BookId == book.Id && bt.TagId == tag.Id));
        }
        
        [Fact]
        public async Task AddTagToBookAsync_WithIncorrectBookId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var tag = new Tag();
            var book = new Book();
            var nonexistentBookId = Guid.NewGuid().ToString();

            await context.Tags.AddAsync(tag);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            // Act
            var result = await booksService.AddTagToBookAsync(nonexistentBookId, tag.Id);
            
            // Assert
            Assert.False(result);
            Assert.False(await context.BookTags.AnyAsync(bt => bt.BookId == book.Id && bt.TagId == tag.Id));
        }
        
        [Fact]
        public async Task AddTagToBookAsync_WithExistingBookTagRelation_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var booksService = new BooksService(context);
            var tag = new Tag();
            var book = new Book();

            await context.Tags.AddAsync(tag);
            await context.Books.AddAsync(book);
            await context.BookTags.AddAsync(new BookTag()
            {
                BookId = book.Id,
                TagId = tag.Id
            });
            
            await context.SaveChangesAsync();

            // Act
            var result = await booksService.AddTagToBookAsync(book.Id, tag.Id);
            
            // Assert
            Assert.False(result);
        }
    }
}