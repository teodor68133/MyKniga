namespace MyKniga.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Services.Models;
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
                PublisherId = Guid.NewGuid().ToString(),
                DownloadUrl = "http://example.com/download.ico"
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

            await context.Books.AddRangeAsync(
                new Book
                {
                    Title = "Book2"
                },
                new Book
                {
                    Title = "Book1"
                }
            );

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
            await context.BookTags.AddAsync(new BookTag
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

        [Fact]
        public async Task RemoveTagFromBookAsync_WithCorrectIds_WorksCorrectly()
        {
            // Arrange
            var bookId = Guid.NewGuid().ToString();
            var tagId = Guid.NewGuid().ToString();
            var context = this.NewInMemoryDatabase();

            await context.BookTags.AddAsync(new BookTag
            {
                BookId = bookId,
                TagId = tagId
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.RemoveTagFromBookAsync(bookId, tagId);

            // Assert
            Assert.True(result);
            Assert.Equal(0, await context.BookTags.CountAsync());
        }

        [Fact]
        public async Task RemoveTagFromBookAsync_WithIncorrectTagId_ReturnsFalse()
        {
            // Arrange
            var bookId = Guid.NewGuid().ToString();
            var fakeId = Guid.NewGuid().ToString();

            var context = this.NewInMemoryDatabase();

            await context.BookTags.AddAsync(new BookTag
            {
                BookId = bookId,
                TagId = Guid.NewGuid().ToString()
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.RemoveTagFromBookAsync(bookId, fakeId);

            // Assert
            Assert.False(result);
            Assert.Equal(1, await context.BookTags.CountAsync());
        }

        [Fact]
        public async Task RemoveTagFromBookAsync_WithIncorrectBookId_ReturnsFalse()
        {
            // Arrange
            var tagId = Guid.NewGuid().ToString();
            var fakeId = Guid.NewGuid().ToString();

            var context = this.NewInMemoryDatabase();

            await context.BookTags.AddAsync(new BookTag
            {
                BookId = Guid.NewGuid().ToString(),
                TagId = tagId
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.RemoveTagFromBookAsync(fakeId, tagId);

            // Assert
            Assert.False(result);
            Assert.Equal(1, await context.BookTags.CountAsync());
        }

        [Fact]
        public async Task RemoveTagFromBookAsync_WithNullBookId_ReturnsFalse()
        {
            // Arrange
            var tagId = Guid.NewGuid().ToString();

            var context = this.NewInMemoryDatabase();

            await context.BookTags.AddAsync(new BookTag
            {
                BookId = Guid.NewGuid().ToString(),
                TagId = tagId
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.RemoveTagFromBookAsync(null, tagId);

            // Assert
            Assert.False(result);
            Assert.Equal(1, await context.BookTags.CountAsync());
        }

        [Fact]
        public async Task RemoveTagFromBookAsync_WithNullTagId_ReturnsFalse()
        {
            // Arrange
            var bookId = Guid.NewGuid().ToString();

            var context = this.NewInMemoryDatabase();

            await context.BookTags.AddAsync(new BookTag
            {
                BookId = bookId,
                TagId = Guid.NewGuid().ToString()
            });

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.RemoveTagFromBookAsync(bookId, null);

            // Assert
            Assert.False(result);
            Assert.Equal(1, await context.BookTags.CountAsync());
        }

        [Fact]
        public async Task UpdateBooksAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testBook = new Book
            {
                Title = "TestBook1",
                Author = "TestAuthor1",
                Price = 11,
                Year = 2011,
                Description = "TestDescription1",
                ShortDescription = "TestShortDescription1",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico1",
                Isbn = "0000005000001",
                DownloadUrl = "http://downloadtest.com/1"
            };

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var updateModel = new BookEditServiceModel
            {
                Id = testBook.Id,
                Title = "TestBook",
                Author = "TestAuthor",
                Price = 1,
                Year = 2001,
                Description = "TestDescription",
                ShortDescription = "TestShortDescription",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico",
                Isbn = "0000005000000",
                DownloadUrl = "http://downloadtest.com"
            };

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.UpdateBookAsync(updateModel);

            // Assert
            Assert.True(result);

            var updatedBook = await context.Books.SingleOrDefaultAsync(b => b.Id == testBook.Id);
            Assert.NotNull(updatedBook);
            Assert.Equal(updateModel.Title, updatedBook.Title);
            Assert.Equal(updateModel.Author, updatedBook.Author);
            Assert.Equal(updateModel.Price, updatedBook.Price);
            Assert.Equal(updateModel.Year, updatedBook.Year);
            Assert.Equal(updateModel.Description, updatedBook.Description);
            Assert.Equal(updateModel.ShortDescription, updatedBook.ShortDescription);
            Assert.Equal(updateModel.Pages, updatedBook.Pages);
            Assert.Equal(updateModel.ImageUrl, updatedBook.ImageUrl);
            Assert.Equal(updateModel.Isbn, updatedBook.Isbn);
            Assert.Equal(updateModel.DownloadUrl, updatedBook.DownloadUrl);
        }

        [Fact]
        public async Task UpdateBooksAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            const string testTitle = "TestBook1";

            var testBook = new Book
            {
                Title = testTitle,
                Author = "TestAuthor1",
                Price = 11,
                Year = 2011,
                Description = "TestDescription1",
                ShortDescription = "TestShortDescription1",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico1",
                Isbn = "0000005000001",
                DownloadUrl = "http://downloadtest.com/1"
            };

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var updateModel = new BookEditServiceModel
            {
                Id = testBook.Id,
                Title = "",
                Author = "TestAuthor",
                Price = 1,
                Year = 2001,
                Description = "TestDescription",
                ShortDescription = "TestShortDescription",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico",
                Isbn = "0000005000000",
                DownloadUrl = "http://downloadtest.com"
            };

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.UpdateBookAsync(updateModel);

            // Assert
            Assert.False(result);

            var updatedBook = await context.Books.SingleOrDefaultAsync(b => b.Id == testBook.Id);
            Assert.NotNull(updatedBook);
            Assert.Equal(testTitle, updatedBook.Title);
        }

        [Fact]
        public async Task UpdateBooksAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testBook = new Book
            {
                Title = "TestBook1",
                Author = "TestAuthor1",
                Price = 11,
                Year = 2011,
                Description = "TestDescription1",
                ShortDescription = "TestShortDescription1",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico1",
                Isbn = "0000005000001",
                DownloadUrl = "http://downloadtest.com/1"
            };

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var updateModel = new BookEditServiceModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = "TestBook",
                Author = "TestAuthor",
                Price = 1,
                Year = 2001,
                Description = "TestDescription",
                ShortDescription = "TestShortDescription",
                Pages = 2,
                ImageUrl = "http://example.com/favicon.ico",
                Isbn = "0000005000000",
                DownloadUrl = "http://downloadtest.com"
            };

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.UpdateBookAsync(updateModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBookAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.DeleteBookAsync(testBook.Id);

            // Assert
            Assert.True(result);
            Assert.False(await context.Books.AnyAsync());
        }

        [Fact]
        public async Task DeleteBookAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.DeleteBookAsync(null);

            // Assert
            Assert.False(result);
            Assert.True(await context.Books.AnyAsync());
        }

        [Fact]
        public async Task DeleteBookAsync_WithIncorrectId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var incorrectId = Guid.NewGuid().ToString();
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            // Act
            var result = await booksService.DeleteBookAsync(incorrectId);

            // Assert
            Assert.False(result);
            Assert.True(await context.Books.AnyAsync());
        }

        [Fact]
        public async Task GetBooksByFilteringAsync_WithAllFilters_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            
            var expectedTitles = new[] {"Book5", "Book6"};

            var testTagId = Guid.NewGuid().ToString();
            var testPublisherId = Guid.NewGuid().ToString();

            await context.Books.AddRangeAsync(
                new Book
                {
                    Title = "Book6",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2015,
                    Price = 30
                },
                new Book
                {
                    Title = "Book5",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2010,
                    Price = 50
                },
                new Book
                {
                    Title = "Book4",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2019,
                    Price = 30
                },
                new Book
                {
                    Title = "Book3",
                    BookTags = new[]
                    {
                        new BookTag {TagId = Guid.NewGuid().ToString()}
                    },
                    PublisherId = testPublisherId,
                    Year = 2015,
                    Price = 30
                },
                new Book
                {
                    Title = "Book2",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = Guid.NewGuid().ToString(),
                    Year = 2015,
                    Price = 30
                },
                new Book
                {
                    Title = "Book1",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2000,
                    Price = 30
                },
                new Book
                {
                    Title = "Book0",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2015,
                    Price = 300
                },
                new Book
                {
                    Title = "Book10",
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2015,
                    Price = 3
                }
            );

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            var serviceModel = new BookFilteringServiceModel
            {
                TagId = testTagId,
                PublisherId = testPublisherId,
                PriceFrom = 30,
                PriceTo = 78.90m,
                YearTo = 2015,
                YearFrom = 2010
            };

            // Act
            var actualTitles = (await booksService.GetBooksByFilteringAsync<BookListingServiceModel>(serviceModel))
                .Select(b => b.Title)
                .ToArray();

            // Assert
            Assert.NotNull(actualTitles);
            Assert.Equal(2, actualTitles.Length);
            Assert.Equal(expectedTitles, actualTitles);
        }

        [Fact]
        public async Task GetBooksByFilteringAsync_WithNoMatching_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testTagId = Guid.NewGuid().ToString();
            var testPublisherId = Guid.NewGuid().ToString();

            await context.Books.AddRangeAsync(
                new Book
                {
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = testPublisherId,
                    Year = 2019,
                    Price = 30
                },
                new Book
                {
                    BookTags = new[]
                    {
                        new BookTag {TagId = Guid.NewGuid().ToString()}
                    },
                    PublisherId = testPublisherId,
                    Year = 2015,
                    Price = 30
                },
                new Book
                {
                    BookTags = new[]
                    {
                        new BookTag {TagId = testTagId}
                    },
                    PublisherId = Guid.NewGuid().ToString(),
                    Year = 2015,
                    Price = 30
                }
            );

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            var serviceModel = new BookFilteringServiceModel
            {
                TagId = testTagId,
                PublisherId = testPublisherId,
                PriceFrom = 30,
                PriceTo = 78.90m,
                YearTo = 2015,
                YearFrom = 2010
            };

            // Act
            var result = await booksService.GetBooksByFilteringAsync<BookListingServiceModel>(serviceModel);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBooksByFilteringAsync_WithOneFilter_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var expectedTitles = new[] {"Book2", "Book3"};

            await context.Books.AddRangeAsync(
                new Book
                {
                    Title = "Book3",
                    Year = 2013
                },
                new Book
                {
                    Title = "Book2",
                    Year = 2005,
                },
                new Book
                {
                    Title = "Book1",
                    Year = 2000
                }
            );

            await context.SaveChangesAsync();

            var booksService = new BooksService(context);

            var serviceModel = new BookFilteringServiceModel
            {
                YearFrom = 2005
            };

            // Act
            var actualTitles = (await booksService.GetBooksByFilteringAsync<BookListingServiceModel>(serviceModel))
                .Select(b => b.Title)
                .ToArray();

            // Assert
            Assert.NotNull(actualTitles);
            Assert.Equal(2, actualTitles.Length);
            Assert.Equal(expectedTitles, actualTitles);
        }
    }
}