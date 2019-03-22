namespace MyKniga.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models;

    public class BooksController : Controller
    {
        private readonly IBooksService booksService;

        public BooksController(IBooksService booksService)
        {
            this.booksService = booksService;
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Create(BookCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceBook = Mapper.Map<BookCreateServiceModel>(model);

            this.booksService.CreateBookAsync(serviceBook);

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> GetBooks()
        {
            var allBooks = await this.booksService.GetAllBooksAsync();

            return this.Ok(allBooks);
        }
    }
}