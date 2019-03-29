namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IPublishersService
    {
        Task<bool> CreatePublisherAsync(PublisherCreateServiceModel model);
        Task<IEnumerable<PublisherListingServiceModel>> GetAllPublishersAsync();
        Task<bool> AssignUserToPublisherAsync(string userId, string publisherId);
    }
}