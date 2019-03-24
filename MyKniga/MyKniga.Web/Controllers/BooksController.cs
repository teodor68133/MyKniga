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
    using Services.Models.Book;

    public class BooksController : Controller
    {
        private readonly IBooksService booksService;
        private readonly ITagsService tagsService;

        public BooksController(IBooksService booksService, ITagsService tagsService)
        {
            this.booksService = booksService;
            this.tagsService = tagsService;
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
            var allBooks = await this.booksService.GetAllBooksAsync<BookListingServiceModel>();

            return this.Ok(allBooks);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }
            
            var book = await this.booksService.GetBookByIdAsync<BookDetailsServiceModel>(id);

            if (book == null)
            {
                return this.NotFound();
            }

            var viewBook = Mapper.Map<BookDetailsViewModel>(book);
            var allTags = (await this.tagsService.GetAllTagsAsync())
                .Select(Mapper.Map<TagDisplayViewModel>)
                .ToArray();

            viewBook.AllTags = allTags;

            return this.View(viewBook);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AddTagToBook(string bookId, string tagId)
        {
            if (bookId == null || tagId == null)
            {
                return this.Ok(new {success = false});
            }

            var isSuccess = await this.booksService.AddTagToBookAsync(bookId, tagId);

            return this.Ok(new {success = isSuccess});
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> RemoveTagFromBook(string bookId, string tagId)
        {
            if (bookId == null || tagId == null)
            {
                return this.Ok(new {success = false});
            }

            await this.booksService.RemoveTagFromBookAsync(bookId, tagId);

            return this.Ok(new {success = true});
        }
    }
}