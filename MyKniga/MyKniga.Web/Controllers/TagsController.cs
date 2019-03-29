namespace MyKniga.Web.Controllers
{
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

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TagCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceModel = Mapper.Map<TagCreateServiceModel>(model);

            var isSuccess = await this.tagsService.CreateAsync(serviceModel);

            if (!isSuccess)
            {
                this.ShowErrorMessage(NotificationMessages.TagCreateErrorMessage);
                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.TagCreateSuccessMessage);
            return this.RedirectToAction("Index", "Home");
        }
    }
}