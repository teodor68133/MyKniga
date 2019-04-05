namespace MyKniga.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using MyKniga.Models;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> logger;
        private readonly SignInManager<KnigaUser> signInManager;
        private readonly UserManager<KnigaUser> userManager;

        public RegisterModel(
            UserManager<KnigaUser> userManager,
            SignInManager<KnigaUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

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

            var user = new KnigaUser
            {
                UserName = this.Input.Email,
                Email = this.Input.Email
            };

            var result = await this.userManager.CreateAsync(user, this.Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.Page();
            }

            this.logger.LogInformation("User created a new account with password.");

            await this.signInManager.SignInAsync(user, false);
            return this.LocalRedirect(returnUrl);
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}