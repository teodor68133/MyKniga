namespace MyKniga.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models;

    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly IPurchasesService purchasesService;
        private readonly IBooksService booksService;

        public PurchaseController(IPurchasesService purchasesService, IBooksService booksService)
        {
            this.purchasesService = purchasesService;
            this.booksService = booksService;
        }

        public async Task<IActionResult> ConfirmPurchase(string bookId)
        {
            var serviceBook = await this.booksService.GetBookByIdAsync(bookId);

            // TODO: Redirect To Error Page
            if (serviceBook == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var book = Mapper.Map<BookDetailsViewModel>(serviceBook);

            return this.View(book);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseBook(string bookId)
        {
            var purchase = new PurchaseCreateServiceModel
            {
                BookId = bookId,
                PurchaseDate = DateTime.UtcNow,
                UserName = this.User.Identity.Name
            };

            var isSuccess = await this.purchasesService.CreateAsync(purchase);

            // TODO: Redirect To Error Page
            if (!isSuccess)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}