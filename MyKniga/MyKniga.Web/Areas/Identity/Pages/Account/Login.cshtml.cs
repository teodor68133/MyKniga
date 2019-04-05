namespace MyKniga.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using MyKniga.Models;

    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> logger;
        private readonly SignInManager<KnigaUser> signInManager;

        public LoginModel(SignInManager<KnigaUser> signInManager, ILogger<LoginModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var result = await this.signInManager.PasswordSignInAsync(this.Input.Email, this.Input.Password,
                false, true);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid username or password");
                return this.Page();
            }

            this.logger.LogInformation("User logged in.");
            return this.LocalRedirect(returnUrl);
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}