namespace MyKniga.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MyKniga.Models;
    using Services.Interfaces;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class UserAdministrationController : BaseController
    {
        private readonly UserManager<KnigaUser> userManager;
        private readonly IPublishersService publishersService;

        public UserAdministrationController(UserManager<KnigaUser> userManager, IPublishersService publishersService)
        {
            this.userManager = userManager;
            this.publishersService = publishersService;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await this.userManager.Users
                .OrderBy(u => u.PublisherId == null)
                .ThenBy(u => u.UserName)
                .ProjectTo<UserListingViewModel>()
                .ToArrayAsync();

            var allPublishers = (await this.publishersService.GetAllPublishersAsync())
                .Select(Mapper.Map<PublisherListingViewModel>)
                .ToArray();

            var model = new UserAdministrationIndexViewModel
            {
                Users = allUsers,
                Publishers = allPublishers
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPublisher(string userId, string publisherId)
        {
            if (userId == null || publisherId == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherAssignErrorMessage);
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherAssignErrorMessage);
                return this.RedirectToAction("Index");
            }

            var success = await this.publishersService.AssignUserToPublisherAsync(userId, publisherId);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherAssignErrorMessage);
                return this.RedirectToAction("Index");
            }

            if (!await this.userManager.IsInRoleAsync(user, GlobalConstants.PublisherRoleName))
            {
                await this.userManager.AddToRoleAsync(user, GlobalConstants.PublisherRoleName);
            }

            this.ShowSuccessMessage(NotificationMessages.PublisherAssignSuccessMessage);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> RemovePublisher(string userId)
        {
            if (userId == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherRemoveFromUserErrorMessage);
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherRemoveFromUserErrorMessage);
                return this.RedirectToAction("Index");
            }

            var success = await this.publishersService.RemoveUserFromPublisherAsync(userId);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.PublisherRemoveFromUserErrorMessage);
                return this.RedirectToAction("Index");
            }

            if (await this.userManager.IsInRoleAsync(user, GlobalConstants.PublisherRoleName))
            {
                await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.PublisherRoleName);
            }

            this.ShowSuccessMessage(NotificationMessages.PublisherRemoveFromUserSuccessMessage);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (userId == null)
            {
                this.ShowErrorMessage(NotificationMessages.UserDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                this.ShowErrorMessage(NotificationMessages.UserDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            var result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                this.ShowErrorMessage(NotificationMessages.UserDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            this.ShowSuccessMessage(NotificationMessages.UserDeleteSuccessMessage);
            return this.RedirectToAction("Index");
        }
    }
}