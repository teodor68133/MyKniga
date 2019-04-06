namespace MyKniga.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Services.Models;
    using Services;
    using Xunit;

    public class PurchasesServiceTests : BaseTests
    {
        [Fact]
        public async Task CreateAsync_WithCorrectModel_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);
            await context.SaveChangesAsync();

            var serviceModel = new PurchaseCreateServiceModel
            {
                UserName = testUser.UserName,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);

            var purchase = await context.Purchases.SingleOrDefaultAsync();
            Assert.NotNull(purchase);
            Assert.Equal(testUser.Id, purchase.UserId);
            Assert.Equal(testBook.Id, purchase.BookId);
            Assert.Equal(serviceModel.PurchaseDate, purchase.PurchaseDate);
        }

        [Fact]
        public async Task CreateAsync_WithIncorrectModel_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);
            await context.SaveChangesAsync();

            var serviceModel = new PurchaseCreateServiceModel
            {
                UserName = null,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.False(await context.Purchases.AnyAsync());
        }

        [Fact]
        public async Task CreateAsync_WithNonexistentUser_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.SaveChangesAsync();

            var serviceModel = new PurchaseCreateServiceModel
            {
                UserName = "InvalidUser",
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.False(await context.Purchases.AnyAsync());
        }

        [Fact]
        public async Task CreateAsync_WithNonexistentBook_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            await context.Users.AddAsync(testUser);
            await context.SaveChangesAsync();

            var serviceModel = new PurchaseCreateServiceModel
            {
                UserName = testUser.UserName,
                BookId = Guid.NewGuid().ToString(),
                PurchaseDate = DateTime.UtcNow
            };

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.False(await context.Purchases.AnyAsync());
        }

        [Fact]
        public async Task CreateAsync_WithExistingPurchase_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);

            var purchase = new Purchase
            {
                UserId = testUser.Id,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            await context.Purchases.AddAsync(purchase);
            await context.SaveChangesAsync();

            var serviceModel = new PurchaseCreateServiceModel
            {
                UserName = testUser.UserName,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.True(await context.Purchases.AnyAsync(p => p.User.UserName == testUser.UserName));
            Assert.Equal(1, await context.Purchases.CountAsync());
        }


        [Fact]
        public async Task GetPurchasesForUserAsync_WithCorrectUserName_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var expectedResult = new[] {"Book1", "Book0"};

            var testUser = new KnigaUser {UserName = "TestUser"};
            var otherUser = new KnigaUser {UserName = "OtherUser"};

            await context.Users.AddRangeAsync(
                testUser,
                otherUser
            );

            await context.Purchases.AddRangeAsync(
                new Purchase
                {
                    UserId = testUser.Id,
                    PurchaseDate = DateTime.UtcNow,
                    Book = new Book {Title = "Book0"}
                },
                new Purchase
                {
                    UserId = testUser.Id,
                    PurchaseDate = DateTime.UtcNow.AddDays(1),
                    Book = new Book {Title = "Book1"}
                },
                new Purchase
                {
                    UserId = otherUser.Id,
                    PurchaseDate = DateTime.UtcNow,
                    Book = new Book {Title = "Book2"}
                }
            );

            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var actualResult = (await purchasesService.GetPurchasesForUserAsync("TestUser"))
                .Select(p => p.Book.Title)
                .ToArray();

            // Assert
            Assert.Equal(2, actualResult.Length);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetPurchasesForUserAsync_WithNoPurchasesForUser_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            await context.Users.AddAsync(testUser);

            await context.Purchases.AddRangeAsync(new Purchase
                {
                    UserId = testUser.Id,
                    PurchaseDate = DateTime.UtcNow.AddDays(1),
                    Book = new Book {Title = "Book2"}
                },
                new Purchase
                {
                    UserId = testUser.Id,
                    PurchaseDate = DateTime.UtcNow,
                    Book = new Book {Title = "Book1"}
                }
            );

            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var actualResult = (await purchasesService.GetPurchasesForUserAsync("OtherUser"));

            // Assert
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task UserHasPurchasedBookAsync_WithPurchase_ReturnsTrue()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);

            var purchase = new Purchase
            {
                UserId = testUser.Id,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            await context.Purchases.AddAsync(purchase);
            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.UserHasPurchasedBookAsync(testBook.Id, testUser.UserName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserHasPurchasedBookAsync_WithoutPurchase_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var otherUser = new KnigaUser {UserName = "OtherUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddRangeAsync(testUser, otherUser);

            var purchase = new Purchase
            {
                UserId = testUser.Id,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            await context.Purchases.AddAsync(purchase);
            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.UserHasPurchasedBookAsync(testBook.Id, otherUser.UserName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserHasPurchasedBookAsync_WithNullUserId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);

            var purchase = new Purchase
            {
                UserId = testUser.Id,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            await context.Purchases.AddAsync(purchase);
            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.UserHasPurchasedBookAsync(testBook.Id, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserHasPurchasedBookAsync_WithNullBookId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};
            var testBook = new Book();

            await context.Books.AddAsync(testBook);
            await context.Users.AddAsync(testUser);

            var purchase = new Purchase
            {
                UserId = testUser.Id,
                BookId = testBook.Id,
                PurchaseDate = DateTime.UtcNow
            };
            await context.Purchases.AddAsync(purchase);
            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.UserHasPurchasedBookAsync(null, testUser.UserName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetPurchasesForPublisherAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var publisherId = Guid.NewGuid().ToString();

            var expectedPurchases = new[]
            {
                new Purchase
                {
                    Book = new Book
                    {
                        PublisherId = publisherId,
                        Title = "C#"
                    },
                    User = new KnigaUser
                    {
                        Email = "wa@wa.a"
                    },
                    PurchaseDate = DateTime.UtcNow
                },
                new Purchase
                {
                    Book = new Book
                    {
                        PublisherId = publisherId,
                        Title = "Java"
                    },
                    User = new KnigaUser
                    {
                        Email = "wa@wa.a"
                    },
                    PurchaseDate = DateTime.UtcNow.AddDays(1)
                }
            };

            await context.Purchases.AddRangeAsync(expectedPurchases);

            await context.Purchases.AddAsync(
                new Purchase
                {
                    Book = new Book
                    {
                        PublisherId = Guid.NewGuid().ToString(),
                        Title = "C++"
                    },
                    User = new KnigaUser
                    {
                        Email = "wa@wa.a"
                    },
                    PurchaseDate = DateTime.UtcNow.AddDays(1)
                }
            );

            await context.SaveChangesAsync();

            var expectedTitle = expectedPurchases.OrderByDescending(p => p.PurchaseDate).First().Book.Title;

            var purchasesService = new PurchasesService(context);

            // Act
            var actualPurchases = (await purchasesService.GetPurchasesForPublisherAsync(publisherId)).ToArray();

            // Assert
            var actualTitle = actualPurchases.First().Book.Title;

            Assert.NotNull(actualPurchases);
            Assert.Equal(2, actualPurchases.Length);
            Assert.Equal(expectedTitle, actualTitle);
        }

        [Fact]
        public async Task GetPurchasesForPublisherAsync_WithNullPublisherId_ReturnsNull()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            await context.Purchases.AddAsync(new Purchase
            {
                Book = new Book
                {
                    PublisherId = Guid.NewGuid().ToString()
                },
                User = new KnigaUser
                {
                    Email = "wa@wa.a"
                }
            });

            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.GetPurchasesForPublisherAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPurchasesForPublisherAsync_WithNoPurchases_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            await context.Purchases.AddAsync(new Purchase
            {
                Book = new Book
                {
                    PublisherId = Guid.NewGuid().ToString()
                },
                User = new KnigaUser
                {
                    Email = "wa@wa.a"
                }
            });

            await context.SaveChangesAsync();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.GetPurchasesForPublisherAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPurchasesAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var publisherId = Guid.NewGuid().ToString();

            var expectedPurchases = new[]
            {
                new Purchase
                {
                    Book = new Book
                    {
                        PublisherId = publisherId,
                        Title = "C#"
                    },
                    User = new KnigaUser
                    {
                        Email = "wa@wa.a"
                    },
                    PurchaseDate = DateTime.UtcNow
                },
                new Purchase
                {
                    Book = new Book
                    {
                        PublisherId = publisherId,
                        Title = "Java"
                    },
                    User = new KnigaUser
                    {
                        Email = "wa@wa.a"
                    },
                    PurchaseDate = DateTime.UtcNow.AddDays(1)
                }
            };

            await context.Purchases.AddRangeAsync(expectedPurchases);

            await context.SaveChangesAsync();

            var expectedTitle = expectedPurchases.OrderByDescending(p => p.PurchaseDate).First().Book.Title;

            var purchasesService = new PurchasesService(context);

            // Act
            var actualPurchases = (await purchasesService.GetAllPurchasesAsync()).ToArray();

            var actualTitle = actualPurchases.First().Book.Title;

            // Assert
            Assert.NotNull(actualPurchases);
            Assert.Equal(2, actualPurchases.Length);
            Assert.Equal(expectedTitle, actualTitle);
        }

        [Fact]
        public async Task GetAllPurchasesAsync_WithNoPurchases_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var purchasesService = new PurchasesService(context);

            // Act
            var result = await purchasesService.GetAllPurchasesAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}