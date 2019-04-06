namespace MyKniga.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models;
    using Services.Models.Book;

    [Authorize]
    public class PurchaseController : BaseController
    {
        private readonly IPurchasesService purchasesService;
        private readonly IBooksService booksService;
        private readonly IUsersService usersService;

        public PurchaseController(IPurchasesService purchasesService, IBooksService booksService,
            IUsersService usersService)
        {
            this.purchasesService = purchasesService;
            this.booksService = booksService;
            this.usersService = usersService;
        }

        public async Task<IActionResult> ConfirmPurchase(string bookId)
        {
            if (bookId == null)
            {
                this.ShowErrorMessage(NotificationMessages.PurchaseErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            var serviceBook = await this.booksService.GetBookByIdAsync<BookConfirmPurchaseServiceModel>(bookId);


            if (serviceBook == null)
            {
                this.ShowErrorMessage(NotificationMessages.PurchaseErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            var isPurchased = await this.purchasesService.UserHasPurchasedBookAsync(bookId, this.User.Identity.Name);

            if (isPurchased)
            {
                return this.RedirectToAction("Details", "Books", new {id = bookId});
            }

            var book = Mapper.Map<BookConfirmPurchaseViewModel>(serviceBook);

            return this.View(book);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseBook(string bookId)
        {
            if (bookId == null)
            {
                this.ShowErrorMessage(NotificationMessages.PurchaseErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            var isPurchased = await this.purchasesService.UserHasPurchasedBookAsync(bookId, this.User.Identity.Name);

            if (isPurchased)
            {
                return this.RedirectToAction("Details", "Books", new {id = bookId});
            }

            var purchase = new PurchaseCreateServiceModel
            {
                BookId = bookId,
                PurchaseDate = DateTime.UtcNow,
                UserName = this.User.Identity.Name
            };

            var isSuccess = await this.purchasesService.CreateAsync(purchase);

            if (!isSuccess)
            {
                this.ShowErrorMessage(NotificationMessages.PurchaseErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            this.ShowSuccessMessage(NotificationMessages.PurchaseSuccessMessage);

            return this.RedirectToAction("My");
        }

        public async Task<IActionResult> My()
        {
            var servicedBooks = await this.purchasesService.GetPurchasesForUserAsync(this.User.Identity.Name);

            var viewModel = servicedBooks.Select(Mapper.Map<PurchaseListingViewModel>);

            return this.View(viewModel);
        }

        [Authorize(Roles = GlobalConstants.PublisherRoleName)]
        public async Task<IActionResult> Publisher()
        {
            var publisherId = await this.usersService.GetPublisherIdByUserNameAsync(this.User.Identity.Name);

            if (publisherId == null)
            {
                return this.Forbid();
            }

            var serviceBooks = await this.purchasesService.GetPurchasesForPublisherAsync(publisherId);

            var viewModel = serviceBooks.Select(Mapper.Map<PurchaseAdminListingViewModel>);

            return this.View(viewModel);
        }
    }
}