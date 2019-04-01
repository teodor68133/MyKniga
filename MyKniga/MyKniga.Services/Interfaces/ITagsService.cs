namespace MyKniga.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ITagsService
    {
        Task<bool> CreateAsync(TagCreateServiceModel model);
        Task<IEnumerable<TagDisplayServiceModel>> GetAllTagsAsync(string searchQuery = null);
        Task<bool> DeleteTagAsync(string tagId);
    }
}