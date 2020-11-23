using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models.Entities;
using Forum.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        private const int CountOfLastPosts = 5;
        private const int CountOfLastTopics = 5;

        public UsersController(UserManager<User> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
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

            var viewModel = new UserProfileViewModel(
                user,
                Request,
                lastTopics,
                lastPosts,
                _userManager.GetRolesAsync(user).Result,
                _userManager.IsInRoleAsync(user, "Moderator").Result || _userManager.IsInRoleAsync(user, "Admin").Result);

            return View(viewModel);
        }

        // GET: UserController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
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

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
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

        public async Task<IList<string>> GetUserRoles(ClaimsPrincipal userClaims)
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
