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

    public class PublishersService : BaseService, IPublishersService
    {
        public PublishersService(MyKnigaDbContext context) : base(context)
        {
        }

        public async Task<bool> CreatePublisherAsync(PublisherCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var nameToCheck = model.Name.Trim().ToLower();

            // Check if publisher name has already been taken
            if (await this.Context.Publishers.AnyAsync(p => p.Name.ToLower() == nameToCheck))
            {
                return false;
            }

            model.Name = model.Name.Trim();

            var dbPublisher = Mapper.Map<Publisher>(model);

            await this.Context.Publishers.AddAsync(dbPublisher);

            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PublisherListingServiceModel>> GetAllPublishersAsync()
        {
            var publishers = await this.Context.Publishers
                .OrderBy(p => p.Name)
                .ProjectTo<PublisherListingServiceModel>()
                .ToArrayAsync();

            return publishers;
        }

        public async Task<bool> AssignUserToPublisherAsync(string userId, string publisherId)
        {
            if (userId == null || publisherId == null)
            {
                return false;
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null || !await this.Context.Publishers.AnyAsync(p => p.Id == publisherId))
            {
                return false;
            }

            user.PublisherId = publisherId;

            this.Context.Users.Update(user);

            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveUserFromPublisherAsync(string userId)
        {
            if (userId == null)
            {
                return false;
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.PublisherId = null;

            this.Context.Users.Update(user);

            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePublisherAsync(string publisherId)
        {
            if (publisherId == null)
            {
                return false;
            }

            var publisher = await this.Context.Publishers.SingleOrDefaultAsync(p => p.Id == publisherId);

            if (publisher == null)
            {
                return false;
            }

            this.Context.Publishers.Remove(publisher);

            await this.Context.SaveChangesAsync();

            return true;
        }
    }
}