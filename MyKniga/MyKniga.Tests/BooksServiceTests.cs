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
    }
}