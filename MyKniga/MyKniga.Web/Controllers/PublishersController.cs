namespace MyKniga.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using MyKniga.Models;
    using Services.Interfaces;
    using Services.Models.Publisher;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class PublishersController : BaseController
    {
        private readonly IPublishersService publishersService;

        public PublishersController(IPublishersService service)
        {
            this.publishersService = service;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PublisherCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherCreateErrorMessage);
                return this.View(model);
            }

            var servicePublisher = Mapper.Map<PublisherCreateServiceModel>(model);

            var success = await this.publishersService.CreatePublisherAsync(servicePublisher);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherCreateErrorMessage);
                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.PublisherCreateSuccessMessage);
            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var servicePublisher = await this.publishersService.GetPublisherByIdAsync<PublisherEditServiceModel>(id);

            if (servicePublisher == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherEditErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            var model = Mapper.Map<PublisherEditBindingModel>(servicePublisher);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PublisherEditBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceModel = Mapper.Map<PublisherEditServiceModel>(model);

            var success = await this.publishersService.UpdatePublisherAsync(serviceModel);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherEditErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            this.ShowSuccessMessage(NotificationMessages.PublisherEditSuccessMessage);
            return this.RedirectToAction("Index", "Home");
        }
    }
}