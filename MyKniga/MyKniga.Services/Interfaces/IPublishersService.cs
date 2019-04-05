namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Publisher;

    public interface IPublishersService
    {
        Task<bool> CreatePublisherAsync(PublisherCreateServiceModel model);
        Task<IEnumerable<T>> GetAllPublishersAsync<T>() where T : BasePublisherServiceModel;
        Task<bool> AssignUserToPublisherAsync(string userId, string publisherId);
        Task<bool> RemoveUserFromPublisherAsync(string userId);
        Task<bool> DeletePublisherAsync(string publisherId);
    }
}