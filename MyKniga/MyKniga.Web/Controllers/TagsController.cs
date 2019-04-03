namespace MyKniga.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models;

    [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
    public class TagsController : BaseController
    {
        private readonly ITagsService tagsService;

        public TagsController(ITagsService tagsService)
        {
            this.tagsService = tagsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (name == null)
            {
                return this.Ok(new {success = false});
            }

            var serviceModel = new TagCreateServiceModel
            {
                Name = name
            };

            var success = await this.tagsService.CreateAsync(serviceModel);

            return this.Ok(new {success});
        }

        public async Task<IActionResult> GetTags(string searchQuery)
        {
            var serviceTags = await this.tagsService.GetAllTagsAsync(searchQuery);

            var viewTags = serviceTags.Select(Mapper.Map<TagDisplayViewModel>);

            var model = new TagListingViewModel
            {
                Tags = viewTags
            };

            return this.Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTag(string tagId)
        {
            if (tagId == null)
            {
                return this.Ok(new {success = false});
            }

            var success = await this.tagsService.DeleteTagAsync(tagId);

            if (!success)
            {
                return this.Ok(new {success = false});
            }

            return this.Ok(new {success = true});
        }

        public IActionResult Administer()
        {
            return this.View();
        }
    }
}