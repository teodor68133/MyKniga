namespace MyKniga.Web.Controllers
{
    using System.Linq;
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
        private readonly UserManager<KnigaUser> userManager;

        public PublishersController(IPublishersService service, UserManager<KnigaUser> userManager)
        {
            this.publishersService = service;
            this.userManager = userManager;
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

        public async Task<IActionResult> Administer()
        {
            var servicePublishers = await this.publishersService.GetAllPublishersAsync<PublisherDetailsServiceModel>();

            var publishers = servicePublishers.Select(Mapper.Map<PublisherDetailsViewModel>);

            var model = new AllPublishersViewModel
            {
                Publishers = publishers
            };

            return this.View(model);
        }

        public async Task<IActionResult> ConfirmDeletion(string id)
        {
            if (id == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherDeleteErrorMessage);
                return this.RedirectToAction("Administer");
            }

            var publisher = await this.publishersService.GetPublisherByIdAsync<PublisherDetailsServiceModel>(id);

            if (publisher == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherDeleteErrorMessage);
                return this.RedirectToAction("Administer");
            }

            var model = Mapper.Map<PublisherDetailsViewModel>(publisher);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePublisher(string id)
        {
            if (id == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherDeleteErrorMessage);
                return this.RedirectToAction("Administer");
            }

            var userIds = await this.publishersService.GetAllUserIdsInPublisherAsync(id);

            foreach (var userId in userIds)
            {
                var user = await this.userManager.FindByIdAsync(userId);

                await this.publishersService.RemoveUserFromPublisherAsync(userId);

                if (await this.userManager.IsInRoleAsync(user, GlobalConstants.PublisherRoleName))
                {
                    await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.PublisherRoleName);
                }
            }

            var success = await this.publishersService.DeletePublisherAsync(id);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherDeleteErrorMessage);
                return this.RedirectToAction("Administer");
            }

            this.ShowSuccessMessage(NotificationMessages.PublisherDeleteSuccessMessage);
            return this.RedirectToAction("Administer");
        }
    }
}