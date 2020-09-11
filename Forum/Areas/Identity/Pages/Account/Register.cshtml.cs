using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forum.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly UserSettings _userSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<UserSettings> userSettings,
            ApplicationDbContext dbContext,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userSettings = userSettings.Value;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    Registration = DateTime.Now
                };

                if(EmailAlreadyExists(user.Email))
                {
                    ModelState.AddModelError(string.Empty, $"User with email '{user.Email}' already exists.");
                    return Page();
                }

                if (UsernameAlreadyExists(user.UserName))
                {
                    ModelState.AddModelError(string.Empty, $"User with username '{user.UserName}' already exists.");
                    return Page();
                }

                SetUserDefaultParams(user);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void SetUserDefaultParams(User user)
        {
            user.About = string.Empty;
            user.CountOfMessages = 0;
            user.LastActivity = DateTime.Now;
            user.Registration = DateTime.Now;
            if (_userSettings != null)
            {
                var image = _dbContext.Images
                    .Where(
                    i => i.Filename == _userSettings.DefaultProfilePicture)
                    .FirstOrDefault();
                user.ProfilePicture = image;
            }
            _userManager.UpdateAsync(user).Wait();
        }

        private bool EmailAlreadyExists(string email)
        {
            bool exists = false;

            var userWithSuchEmail = _dbContext.Users.Where(u => u.Email.Equals(email)).FirstOrDefault();
            if(userWithSuchEmail != null && userWithSuchEmail.EmailConfirmed)
            {
                exists = true;
            }

            return exists;
        }

        private bool UsernameAlreadyExists(string username)
        {
            bool exists = false;

            var userWithSuchUsername = _dbContext.Users.Where(u => u.UserName.Equals(username)).FirstOrDefault();
            if (userWithSuchUsername != null && userWithSuchUsername.EmailConfirmed)
            {
                exists = true;
            }

            return exists;
        }
    }
}
