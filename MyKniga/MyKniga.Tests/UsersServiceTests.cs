namespace MyKniga.Tests
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using Services;
    using Xunit;

    public class UsersServiceTests : BaseTests
    {
        [Fact]
        public async Task GetPublisherIdByUserNameAsync_WithCorrectData_WorksCorrectly()
        {
            // Arrange
            const string testUserName = "User1";
            var expectedId = Guid.NewGuid().ToString();

            var context = this.NewInMemoryDatabase();

            await context.Users.AddRangeAsync(
                new KnigaUser
                {
                    UserName = testUserName,
                    PublisherId = expectedId
                },
                new KnigaUser
                {
                    UserName = "User2",
                    PublisherId = Guid.NewGuid().ToString()
                }
            );

            await context.SaveChangesAsync();

            var usersService = new UsersService(context);

            // Act
            var actualId = await usersService.GetPublisherIdByUserNameAsync(testUserName);

            // Assert
            Assert.NotNull(actualId);
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task GetPublisherIdByUserNameAsync_WithNullPublisherId_ReturnsNull()
        {
            // Arrange
            const string testUserName = "User1";
            var context = this.NewInMemoryDatabase();

            await context.Users.AddRangeAsync(
                new KnigaUser
                {
                    UserName = testUserName,
                    PublisherId = null
                },
                new KnigaUser
                {
                    UserName = "User2",
                    PublisherId = Guid.NewGuid().ToString()
                }
            );

            await context.SaveChangesAsync();

            var usersService = new UsersService(context);

            // Act
            var actualResult = await usersService.GetPublisherIdByUserNameAsync(testUserName);

            // Assert
            Assert.Null(actualResult);
        }

        [Fact]
        public async Task GetPublisherIdByUserNameAsync_WithNameNull_ReturnsNull()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            await context.Users.AddAsync(new KnigaUser
            {
                UserName = "User2",
                PublisherId = Guid.NewGuid().ToString()
            });

            await context.SaveChangesAsync();
            
            var usersService = new UsersService(context);

            // Act
            var actualResult = await usersService.GetPublisherIdByUserNameAsync(null);

            // Assert
            Assert.Null(actualResult);
        }
        
        [Fact]
        public async Task GetPublisherIdByUserNameAsync_WithNonexistentUser_ReturnsNull()
        {
            // Arrange
            const string testUserName = "User1";
            var context = this.NewInMemoryDatabase();

            await context.Users.AddAsync(new KnigaUser
            {
                UserName = "User2",
                PublisherId = Guid.NewGuid().ToString()
            });

            await context.SaveChangesAsync();
            
            var usersService = new UsersService(context);

            // Act
            var actualResult = await usersService.GetPublisherIdByUserNameAsync(testUserName);

            // Assert
            Assert.Null(actualResult);
        }
    }
}