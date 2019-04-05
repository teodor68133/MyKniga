namespace MyKniga.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;
    using Services.Models.Publisher;
    using Xunit;

    public class PublishersServiceTests : BaseTests
    {
        [Fact]
        public async Task CreatePublisherAsync_WithCorrectModel_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var publisherModel = new PublisherCreateServiceModel
            {
                Description = "TestDescription",
                ImageUrl = "http://www.test.com",
                Name = "TestName "
            };

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.CreatePublisherAsync(publisherModel);

            // Assert
            Assert.True(result);

            var publisher = await context.Publishers.SingleOrDefaultAsync();
            Assert.NotNull(publisher);
            Assert.Equal(publisherModel.Name.Trim(), publisher.Name);
        }

        [Fact]
        public async Task CreatePublisherAsync_WithIncorrectModel_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var publisherModel = new PublisherCreateServiceModel
            {
                Description = "TestDescription",
                ImageUrl = "http://www.test.com",
                Name = ""
            };

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.CreatePublisherAsync(publisherModel);

            // Assert
            Assert.False(result);
            Assert.False(await context.Publishers.AnyAsync());
        }

        [Fact]
        public async Task CreatePublisherAsync_WithRepeatedName_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var publisherModel = new PublisherCreateServiceModel
            {
                Description = "TestDescription",
                ImageUrl = "http://www.test.com",
                Name = "TestName "
            };

            await context.Publishers.AddAsync(
                new Publisher
                {
                    Name = "TestName"
                }
            );
            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.CreatePublisherAsync(publisherModel);

            // Assert
            Assert.False(result);
            Assert.Equal(1, await context.Publishers.CountAsync());
        }

        [Fact]
        public async Task GetPublishersAsync_WithPublishers_WorksCorrectly()
        {
            // Arrange
            var expectedResult = new[] {"Publisher1", "Publisher2"};
            var context = this.NewInMemoryDatabase();

            await context.Publishers.AddRangeAsync(
                new Publisher
                {
                    Name = "Publisher2"
                },
                new Publisher
                {
                    Name = "Publisher1"
                }
            );

            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var actualResult = (await publishersService.GetAllPublishersAsync<PublisherDetailsServiceModel>())
                .Select(p => p.Name)
                .ToArray();

            // Assert
            Assert.Equal(2, actualResult.Length);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetPublishersAsync_WithoutPublishers_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var publishersService = new PublishersService(context);

            // Act
            var actualResult = await publishersService.GetAllPublishersAsync<PublisherDetailsServiceModel>();

            // Assert
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task AssignUserToPublisherAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Users.AddAsync(testUser);
            await context.Publishers.AddAsync(testPublisher);
            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.AssignUserToPublisherAsync(testUser.Id, testPublisher.Id);

            // Assert
            Assert.True(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Equal(testPublisher.Id, dbUser.PublisherId);
        }

        [Fact]
        public async Task AssignUserToPublisherAsync_WithNullPublisherId_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Users.AddAsync(testUser);
            await context.Publishers.AddAsync(testPublisher);
            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.AssignUserToPublisherAsync(testPublisher.Id, null);

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Null(dbUser.PublisherId);
        }

        [Fact]
        public async Task AssignUserToPublisherAsync_WithNullUserId_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Users.AddAsync(testUser);
            await context.Publishers.AddAsync(testPublisher);
            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.AssignUserToPublisherAsync(null, testPublisher.Id);

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Null(dbUser.PublisherId);
        }

        [Fact]
        public async Task AssignUserToPublisherAsync_WithIncorrectUserId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            var testPublisher = new Publisher {Name = "TestPublisher"};

            var publishersService = new PublishersService(context);

            await context.Users.AddAsync(testUser);
            await context.Publishers.AddAsync(testPublisher);
            await context.SaveChangesAsync();

            // Act
            var result =
                await publishersService.AssignUserToPublisherAsync(Guid.NewGuid().ToString(), testPublisher.Id);

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Null(dbUser.PublisherId);
        }

        [Fact]
        public async Task AssignUserToPublisherAsync_WithIncorrectPublisherId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testUser = new KnigaUser {UserName = "TestUser"};

            var testPublisher = new Publisher {Name = "TestPublisher"};

            var publishersService = new PublishersService(context);

            await context.Users.AddAsync(testUser);
            await context.Publishers.AddAsync(testPublisher);
            await context.SaveChangesAsync();

            // Act
            var result = await publishersService.AssignUserToPublisherAsync(testUser.Id, Guid.NewGuid().ToString());

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Null(dbUser.PublisherId);
        }

        [Fact]
        public async Task RemoveUserFromPublisherAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Publishers.AddAsync(testPublisher);

            var testUser = new KnigaUser {UserName = "TestUser", PublisherId = testPublisher.Id};

            await context.Users.AddAsync(testUser);

            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.RemoveUserFromPublisherAsync(testUser.Id);

            // Assert
            Assert.True(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Null(dbUser.PublisherId);
        }

        [Fact]
        public async Task RemoveUserFromPublisherAsync_WithNullId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Publishers.AddAsync(testPublisher);

            var testUser = new KnigaUser {UserName = "TestUser", PublisherId = testPublisher.Id};

            await context.Users.AddAsync(testUser);

            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.RemoveUserFromPublisherAsync(null);

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Equal(testPublisher.Id, dbUser.PublisherId);
        }

        [Fact]
        public async Task RemoveUserFromPublisherAsync_WithIncorrectId_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var testPublisher = new Publisher {Name = "TestPublisher"};

            await context.Publishers.AddAsync(testPublisher);

            var testUser = new KnigaUser {UserName = "TestUser", PublisherId = testPublisher.Id};

            await context.Users.AddAsync(testUser);

            await context.SaveChangesAsync();

            var publishersService = new PublishersService(context);

            // Act
            var result = await publishersService.RemoveUserFromPublisherAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.False(result);

            var dbUser = await context.Users.SingleOrDefaultAsync();
            Assert.Equal(testPublisher.Id, dbUser.PublisherId);
        }
    }
}