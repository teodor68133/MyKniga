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
    using Models.Publisher;
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

        public async Task<IEnumerable<T>> GetAllPublishersAsync<T>()
            where T : BasePublisherServiceModel
        {
            var publishers = await this.Context.Publishers
                .OrderBy(p => p.Name)
                .ProjectTo<T>()
                .ToArrayAsync();

            return publishers;
        }

        public async Task<bool> UpdatePublisherAsync(PublisherEditServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var publisher = await this.Context.Publishers.SingleOrDefaultAsync(p => p.Id == model.Id);

            if (publisher == null)
            {
                return false;
            }

            var nameToCheck = model.Name.Trim().ToLower();

            // Check if publisher name has already been taken
            if (await this.Context.Publishers.AnyAsync(p => p.Name.ToLower() == nameToCheck))
            {
                return false;
            }

            publisher.Name = model.Name.Trim();
            publisher.Description = model.Description;
            publisher.ImageUrl = model.ImageUrl;

            this.Context.Publishers.Update(publisher);
            await this.Context.SaveChangesAsync();

            return true;
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

        public async Task<IEnumerable<string>> GetAllUserIdsInPublisherAsync(string publisherId)
        {
            if (publisherId == null)
            {
                return null;
            }

            var userIds = await this.Context.Users.Where(u => u.PublisherId == publisherId)
                .Select(u => u.Id)
                .ToArrayAsync();

            return userIds;
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

        public async Task<T> GetPublisherByIdAsync<T>(string id)
            where T : BasePublisherServiceModel
        {
            var servicePublisher = await this.Context.Publishers
                .ProjectTo<T>()
                .SingleOrDefaultAsync(p => p.Id == id);

            return servicePublisher;
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