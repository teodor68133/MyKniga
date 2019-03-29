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
    }
}