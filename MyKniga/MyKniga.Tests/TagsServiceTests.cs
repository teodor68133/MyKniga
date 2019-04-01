namespace MyKniga.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Services.Models;
    using Services;
    using Xunit;

    public class TagsServiceTests : BaseTests
    {
        [Fact]
        public async Task CreateAsync_WithValidModel_WorksCorrectly()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var serviceModel = new TagCreateServiceModel
            {
                Name = "TestTag"
            };

            var tagsService = new TagsService(context);

            // Act
            var result = await tagsService.CreateAsync(serviceModel);

            // Assert
            Assert.True(result);
            Assert.True(await context.Tags.AnyAsync(t => t.Name == serviceModel.Name.ToLower()));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ReturnsFalse()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();

            var serviceModel = new TagCreateServiceModel
            {
                Name = null
            };

            var tagsService = new TagsService(context);

            // Act
            var result = await tagsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.Equal(0, context.Tags.Count());
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateName_ReturnsFalse()
        {
            // Arrange
            const string testTagName = "testtag";
            var context = this.NewInMemoryDatabase();

            await context.Tags.AddAsync(new Tag
            {
                Name = testTagName
            });

            await context.SaveChangesAsync();

            var serviceModel = new TagCreateServiceModel
            {
                Name = testTagName
            };

            var tagsService = new TagsService(context);

            // Act
            var result = await tagsService.CreateAsync(serviceModel);

            // Assert
            Assert.False(result);
            Assert.True(await context.Tags.AnyAsync(t => t.Name == testTagName));
            Assert.Equal(1, context.Tags.Count());
        }

        [Fact]
        public async Task GetAllTagsAsync_WithTags_WorksCorrectly()
        {
            // Arrange
            var expectedResult = new[] {"Tag1", "Tag2"};
            var context = this.NewInMemoryDatabase();

            await context.Tags.AddRangeAsync(new[]
            {
                new Tag
                {
                    Name = "Tag2"
                },
                new Tag
                {
                    Name = "Tag1"
                }
            });

            await context.SaveChangesAsync();

            var tagsService = new TagsService(context);

            // Act
            var actualResult = (await tagsService.GetAllTagsAsync())
                .Select(t => t.Name)
                .ToArray();

            // Assert
            Assert.Equal(2, actualResult.Length);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task GetAllTagsAsync_WithoutTags_ReturnsEmptyCollection()
        {
            // Arrange
            var context = this.NewInMemoryDatabase();
            var tagsService = new TagsService(context);

            // Act
            var actualResult = await tagsService.GetAllTagsAsync();

            // Assert
            Assert.Empty(actualResult);
        }
    }
}