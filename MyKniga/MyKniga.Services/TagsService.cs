namespace MyKniga.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Models;

    public class TagsService : BaseService, ITagsService
    {
        public TagsService(MyKnigaDbContext context) : base(context)
        {
        }

        public async Task<bool> CreateAsync(TagCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var nameToCheck = model.Name.Trim().ToLower();

            // Verify that a tag with the provided name does not already exist
            if (await this.Context.Tags.AnyAsync(t => t.Name.ToLower() == nameToCheck))
            {
                return false;
            }

            var tagDbModel = new Tag
            {
                Name = model.Name.Trim().ToLower()
            };

            await this.Context.Tags.AddAsync(tagDbModel);

            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TagDisplayServiceModel>> GetAllTagsAsync(string searchQuery = null)
        {
            var allTags = this.Context.Tags.AsQueryable();

            if (searchQuery != null)
            {
                allTags = allTags.Where(t => t.Name.Contains(searchQuery));
            }

            var selectedTags = await allTags.OrderBy(t => t.Name)
                .ProjectTo<TagDisplayServiceModel>()
                .ToArrayAsync();

            return selectedTags;
        }

        public async Task<bool> DeleteTagAsync(string tagId)
        {
            if (tagId == null)
            {
                return false;
            }

            var tag = await this.Context.Tags.SingleOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
            {
                return false;
            }

            this.Context.Tags.Remove(tag);

            await this.Context.SaveChangesAsync();

            return true;
        }
    }
}