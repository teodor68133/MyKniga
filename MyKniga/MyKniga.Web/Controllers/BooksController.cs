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
    using Services.Models.Book;
    using Services.Models.Publisher;

    public class BooksController : BaseController
    {
        private readonly IBooksService booksService;
        private readonly ITagsService tagsService;
        private readonly IUsersService usersService;
        private readonly IPurchasesService purchasesService;
        private readonly IPublishersService publishersService;

        public BooksController(IBooksService booksService, ITagsService tagsService, IUsersService usersService,
            IPurchasesService purchasesService, IPublishersService publishersService)
        {
            this.booksService = booksService;
            this.tagsService = tagsService;
            this.usersService = usersService;
            this.purchasesService = purchasesService;
            this.publishersService = publishersService;
        }

        [Authorize(Roles = GlobalConstants.PublisherRoleName)]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.PublisherRoleName)]
        public async Task<IActionResult> Create(BookCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.ShowErrorMessage(NotificationMessages.BookCreateErrorMessage);
                return this.View(model);
            }

            var publisherId = await this.usersService.GetPublisherIdByUserNameAsync(this.User.Identity.Name);

            if (publisherId == null)
            {
                this.ShowErrorMessage(NotificationMessages.BookCreateErrorMessage);
                return this.RedirectToAction("Index", "Home");
            }

            var serviceBook = Mapper.Map<BookCreateServiceModel>(model);

            serviceBook.PublisherId = publisherId;

            var bookId = await this.booksService.CreateBookAsync(serviceBook);

            if (bookId == null)
            {
                this.ShowErrorMessage(NotificationMessages.BookCreateErrorMessage);
                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.BookCreateSuccessMessage);
            return this.RedirectToAction("Details", "Books", new {id = bookId});
        }

        public async Task<IActionResult> Index()
        {
            var servicePublishers = await this.publishersService.GetAllPublishersAsync<PublisherListingServiceModel>();
            var serviceTags = await this.tagsService.GetAllTagsAsync();

            var publishers = servicePublishers.Select(Mapper.Map<PublisherListingViewModel>).ToArray();
            var tags = serviceTags.Select(Mapper.Map<TagDisplayViewModel>).ToArray();

            var model = new AllBooksViewModel
            {
                Publishers = publishers,
                Tags = tags
            };

            return this.View(model);
        }

        [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
        public async Task<IActionResult> Edit(string id)
        {
            var serviceBook = await this.booksService.GetBookByIdAsync<BookEditServiceModel>(id);

            if (serviceBook == null || !await this.UserCanEditBookAsync(serviceBook))
            {
                this.ShowErrorMessage(NotificationMessages.BookEditErrorMessage);
                return this.RedirectToAction("Index");
            }

            var model = Mapper.Map<BookEditBindingModel>(serviceBook);

            return this.View(model);
        }

        [HttpPost]
        [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
        public async Task<IActionResult> Edit(BookEditBindingModel model)
        {
            var serviceBook = await this.booksService.GetBookByIdAsync<BookWithPublisherServiceModel>(model.Id);

            if (serviceBook == null || !await this.UserCanEditBookAsync(serviceBook))
            {
                this.ShowErrorMessage(NotificationMessages.BookEditErrorMessage);
                return this.RedirectToAction("Index");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceModel = Mapper.Map<BookEditServiceModel>(model);

            var success = await this.booksService.UpdateBookAsync(serviceModel);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.BookEditErrorMessage);
                return this.RedirectToAction("Index");
            }

            this.ShowSuccessMessage(NotificationMessages.BookEditSuccessMessage);
            return this.RedirectToAction("Details", new {id = model.Id});
        }

        public async Task<IActionResult> GetBooks(BookFilteringBindingModel model)
        {
            var serviceModel = Mapper.Map<BookFilteringServiceModel>(model);

            var allBooks = await this.booksService.GetBooksByFilteringAsync<BookListingServiceModel>(serviceModel);

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

            viewBook.CanEdit = await this.UserCanEditBookAsync(book);

            viewBook.HasPurchased = await this.UserHasPurchasedBookAsync(book);

            if (viewBook.CanEdit)
            {
                var allTags = (await this.tagsService.GetAllTagsAsync())
                    .Select(Mapper.Map<TagDisplayViewModel>)
                    .ToArray();

                viewBook.AllTags = allTags;
            }

            return this.View(viewBook);
        }

        [HttpPost]
        [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
        public async Task<IActionResult> AddTagToBook(string bookId, string tagId)
        {
            if (bookId == null || tagId == null)
            {
                return this.Ok(new {success = false});
            }

            var book = await this.booksService.GetBookByIdAsync<BookWithPublisherServiceModel>(bookId);

            if (book == null || !await this.UserCanEditBookAsync(book))
            {
                return this.Ok(new {success = false});
            }

            var isSuccess = await this.booksService.AddTagToBookAsync(bookId, tagId);

            return this.Ok(new {success = isSuccess});
        }

        [HttpPost]
        [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
        public async Task<IActionResult> RemoveTagFromBook(string bookId, string tagId)
        {
            if (bookId == null || tagId == null)
            {
                return this.Ok(new {success = false});
            }

            var book = await this.booksService.GetBookByIdAsync<BookWithPublisherServiceModel>(bookId);

            if (book == null || !await this.UserCanEditBookAsync(book))
            {
                return this.Ok(new {success = false});
            }

            var success = await this.booksService.RemoveTagFromBookAsync(bookId, tagId);

            return this.Ok(new {success});
        }

        [HttpPost]
        [Authorize(Policy = GlobalConstants.AdministratorOrPublisherPolicyName)]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            if (bookId == null)
            {
                this.ShowErrorMessage(NotificationMessages.BookDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            var book = await this.booksService.GetBookByIdAsync<BookWithPublisherServiceModel>(bookId);

            if (book == null || !await this.UserCanEditBookAsync(book))
            {
                this.ShowErrorMessage(NotificationMessages.BookDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            var success = await this.booksService.DeleteBookAsync(bookId);

            if (!success)
            {
                this.ShowErrorMessage(NotificationMessages.BookDeleteErrorMessage);
                return this.RedirectToAction("Index");
            }

            this.ShowSuccessMessage(NotificationMessages.BookDeleteSuccessMessage);
            return this.RedirectToAction("Index");
        }

        private async Task<bool> UserCanEditBookAsync(BookWithPublisherServiceModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                return true;
            }

            var publisherId = await this.usersService.GetPublisherIdByUserNameAsync(this.User.Identity.Name);

            return publisherId == model.PublisherId;
        }

        private async Task<bool> UserHasPurchasedBookAsync(BookDetailsServiceModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return await this.purchasesService.UserHasPurchasedBookAsync(model.Id, this.User.Identity.Name);
        }
    }
}