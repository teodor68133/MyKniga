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
        Task<T> GetPublisherByIdAsync<T>(string id) where T : BasePublisherServiceModel;
        Task<bool> RemoveUserFromPublisherAsync(string userId);
        Task<bool> UpdatePublisherAsync(PublisherEditServiceModel model);
        Task<bool> DeletePublisherAsync(string publisherId);
        Task<IEnumerable<string>> GetAllUserIdsInPublisherAsync(string publisherId);
    }
}