using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Forum.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly UsersController _usersController;

        public TopicsController(
            ApplicationDbContext dbContext, 
            UserManager<User> userManager,
            UsersController usersController)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _usersController = usersController;
        }

        // GET: TopicsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TopicsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var topic = _dbContext
                .Topics
                .Include(t => t.Author)
                .Include(t => t.Posts)
                .Include(t => t.Author)
                .Include(t => t.Section)
                .FirstOrDefault(t => t.Id == id);

            if (topic == null)
            {
                return Error();
            }

            // Explicit loading of dependent data.
            // The EF Core did not perform lazy data loading.
            // Therefore, I had to implement this little crutch.
            // Discussion of this problem: stackoverflow.com/questions/64094376
            topic.Section = _dbContext.Sections.Where(s => s.Id == topic.SectionId).FirstOrDefault();

            foreach (var post in topic.Posts)
            {
                post.Author = _dbContext.Users.FirstOrDefault(u => u.Id == post.AuthorId);
            }

            var viewModel = new TopicViewModel(topic, Request);

            viewModel.CurrentUserId = _usersController.GetUserId(User).Result;

            SetAccessibilityParams(
                viewModel, 
                User,
                IsOwner(User, topic),
                topic.Section.Moderators
                );

            return View(viewModel);
        }

        private void SetAccessibilityParams(
            TopicViewModel viewModel, 
            ClaimsPrincipal claimsPrincipal, 
            bool isOwner,
            ICollection<Moderator> sectionModerators)
        {
            if (claimsPrincipal == null)
            {
                viewModel.CanEditTopic = false;
                viewModel.CanRemoveTopic = false;
                viewModel.CanEditAllAnswers = false;
                viewModel.CanRemoveAllAnswers = false;
                viewModel.CanCreateAnswer = false;
                return;
            }

            var user = _userManager.GetUserAsync(claimsPrincipal).Result;

            if (user == null)
            {
                viewModel.CanEditTopic = false;
                viewModel.CanRemoveTopic = false;
                viewModel.CanEditAllAnswers = false;
                viewModel.CanRemoveAllAnswers = false;
                viewModel.CanCreateAnswer = false;
                return;
            }

            bool isModeratorOfCurrentSection = sectionModerators.Contains(user);

            viewModel.CanEditTopic = 
                isOwner || 
                isModeratorOfCurrentSection ||
                claimsPrincipal.IsInRole("Admin");

            viewModel.CanRemoveTopic =
                isOwner ||
                isModeratorOfCurrentSection ||
                claimsPrincipal.IsInRole("Admin");

            viewModel.CanEditAllAnswers =
                isModeratorOfCurrentSection ||
                claimsPrincipal.IsInRole("Admin");

            viewModel.CanRemoveAllAnswers =
                isModeratorOfCurrentSection ||
                claimsPrincipal.IsInRole("Admin");

            viewModel.CanCreateAnswer = true;
        }

        private static bool IsOwner(ClaimsPrincipal user, Topic topic)
        {
            bool isOwner = false;

            if(user != null)
            {
                var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                isOwner = topic.AuthorId.Equals(currentUserId);
            }

            return isOwner;
        }

        // GET: TopicsController/Create
        public ActionResult Create(int sectionId)
        {
            var vm = new TopicViewModel(Request);
            vm.SectionId = sectionId;

            return View(vm);
        }

        // POST: TopicsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,SectionId")] TopicViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUserAsync(HttpContext.User).Result;

                var section = _dbContext.Sections.FirstOrDefault(s => s.Id == viewModel.SectionId);

                if (section == null)
                {
                    return Error();
                }

                if (section.Topics == null)
                {
                    section.Topics = new List<Topic>();
                }

                var newTopic = new Topic
                {
                    Title = viewModel.Title,
                    Body = viewModel.Body,
                    Created = DateTime.Now,
                    AuthorId = user.Id,
                    Author = user,
                    SectionId = section.Id,
                    Section = section
                };

                newTopic = _dbContext.Topics.Add(newTopic).Entity;
                await _dbContext.SaveChangesAsync();

                section.Topics.ToList().Add(newTopic);

                await _dbContext.SaveChangesAsync();

                ViewData["Message"] = $"Topic `{viewModel.Title}` successfully created.";
                ViewData["SectionId"] = viewModel.SectionId;

                return View("Success");
            }

            return View(viewModel);
        }

        // GET: TopicsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TopicsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TopicsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TopicsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
