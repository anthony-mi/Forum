using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models.Entities;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Forum.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        private const int CountOfLastPosts = 5;
        private const int CountOfLastTopics = 5;

        public UsersController(
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        // GET: UserController
        public async Task<ActionResult> Index()
        {
            return View("Profile");
        }

        public async Task<ActionResult> Index(string username)
        {
            var task = _userManager.FindByNameAsync(username);
            task.Wait();

            var user = task.Result;

            if(user == null)
            {
                return Redirect("~/Home/Index");
            }



            return View("Profile");
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = _dbContext.Users.Find(id);

            if(user == null)
            {
                return NotFound();
            }

            var lastPosts = _dbContext.Posts.Where(t => t.AuthorId == user.Id).ToList();
            lastPosts = new List<Post>(lastPosts.OrderBy(t => t.Created).Take(CountOfLastPosts));

            var lastTopics = _dbContext.Topics.Where(t => t.AuthorId == user.Id).ToList();
            lastTopics = new List<Topic>(lastTopics.OrderBy(t => t.Created).Take(CountOfLastTopics));

            var currentUser = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;

            var viewModel = new UserProfileViewModel(
                user,
                Request,
                lastTopics,
                lastPosts,
                _userManager.GetRolesAsync(user).Result,
                
                currentUser != null && _userManager.IsInRoleAsync(currentUser, "Moderator").Result || _userManager.IsInRoleAsync(currentUser, "Admin").Result);

            return View(viewModel);
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = _dbContext.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new EditUserProfileViewModel(user));
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            IList<string> errorMessages = new List<string>();

            if (!IsValid(model, out errorMessages, user))
            {
                var viewModel = new ErrorsViewModel
                {
                    Title = "User profile validation error",
                    Text = "Validation errors occurred while editing user profile",
                    Errors = errorMessages,
                    GoBackUrl = Url.Action("Edit", "Users", new { userId = model.Id })
                };

                return View("Errors", viewModel);
            }

            user.UserName = model.Username;
            user.About = model.About;

            if(model.NewProfilePicture != null)
            {
                var userSettings = _serviceProvider.GetRequiredService<IOptions<UserSettings>>();
                var defaultProfilePicture = userSettings.Value.DefaultProfilePicture;
                var imagesManager = _serviceProvider.GetRequiredService<ImagesManager>();
                var newImage = imagesManager.CreateAsync(model.NewProfilePicture).Result;

                newImage = _dbContext.Images.Add(newImage).Entity;

                _dbContext.SaveChanges();

                user.ProfilePicture = newImage;

                if (!user.ProfilePicture.Filename.Equals(defaultProfilePicture))
                {
                    if (_dbContext.Images.FirstOrDefault(i => i.Id.Equals(user.ProfilePicture.Id)) == null)
                    {
                        _dbContext.Images.Remove(user.ProfilePicture);
                        _dbContext.SaveChanges();
                    }

                    imagesManager.RemoveAsync(user.ProfilePicture);
                }
            }

            _dbContext.SaveChangesAsync();

            ViewData["Message"] = "Changes saved successfully.";

            return View("Success");
        }

        private bool IsValid(EditUserProfileViewModel model, out IList<string> errorMessages, User user)
        {
            bool isValid = true;
            var errors = new List<string>();

            if (_userManager.FindByIdAsync(model.Id).Result == null)
            {
                errors.Add("User not found.");
                isValid = false;
            }

            if(string.IsNullOrEmpty(model.Username))
            {
                errors.Add("Username required.");
                isValid = false;
            }


            if (!model.Username.Equals(user.UserName)) // username was changed
            {
                if (_dbContext.Users.FirstOrDefault(u => u.UserName.Equals(model.Username)) != null)
                {
                    errors.Add("User with the same username already exists.");
                    isValid = false;
                }
            }

            errorMessages = errors;

            return isValid;
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(ClaimsPrincipal userClaims)
        {
            var userRoles = new List<string> { "Guest" };

            if(userClaims == null)
            {
                return userRoles;
            }

            var user = _userManager.GetUserAsync(userClaims).Result;

            if (user != null)
            {
                userRoles = _userManager.GetRolesAsync(user).Result as List<string>;
            }

            return userRoles;
        }

        public async Task<string> GetUserId(ClaimsPrincipal userClaims)
        {
            string id = string.Empty;

            if (userClaims != null)
            {
                var user = _userManager.GetUserAsync(userClaims).Result;

                if (user != null)
                {
                    id = user.Id;
                }
            }

            return id;
        }
    }
}
