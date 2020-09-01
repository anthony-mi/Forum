using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Forum.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly UserSettings _userSettings;
        private readonly ApplicationDbContext _dbContext;

        public ConfirmEmailModel(UserManager<User> userManager,
            IOptions<UserSettings> userSettings,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _userSettings = userSettings.Value;
            _dbContext = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if(result.Succeeded)
            {
                SetUserDefaultParams(user);
            }

            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            ViewData["StatusMessage"] = StatusMessage;
            return Page();
        }

        private void SetUserDefaultParams(User user)
        {
            _userManager.AddToRoleAsync(user, "User");

            user.CountOfMessages = 0;
            user.LastActivity = DateTime.Now;
            user.Reputation = 0;
            user.Registration = DateTime.Now;
            if (_userSettings != null)
            {
                var image = _dbContext.Images
                    .Where(
                    i => i.Filename == _userSettings.DefaultProfilePicture)
                    .FirstOrDefault();
                user.ProfilePicture = image;
            }
            _userManager.UpdateAsync(user);
        }
    }
}
