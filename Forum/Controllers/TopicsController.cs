using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if(!CanUserViewTopic(User, topic))
            {
                return NotFound();
            }

            var viewModel = new TopicViewModel(topic, Request);

            viewModel.CurrentUserId = _usersController.GetUserId(User).Result;

            SetAccessibilityParams(
                viewModel, 
                User,
                IsOwner(User, topic),
                topic.Section.Moderators,
                topic.Accessibility
                );

            return View(viewModel);
        }

        private static bool CanUserViewTopic(ClaimsPrincipal user, Topic topic)
        {
            return  user.IsInRole("Administrator") ||
                    user.IsInRole("Moderator") ||
                    user.IsInRole("User") ||
                    topic.Accessibility == Accessibility.FullAccess;
        }

        private void SetAccessibilityParams(
            TopicViewModel viewModel, 
            ClaimsPrincipal claimsPrincipal, 
            bool isOwner,
            ICollection<Moderator> sectionModerators,
            Accessibility topicAccessibility)
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

            viewModel.CanCreateAnswer = claimsPrincipal.IsInRole("Administrator") ||
                    isModeratorOfCurrentSection ||
                    claimsPrincipal.IsInRole("User") && topicAccessibility == Accessibility.OnlyForUsers ||
                    claimsPrincipal.IsInRole("User") && topicAccessibility == Accessibility.FullAccess;
        }

        private bool IsModeratorOfSection(ClaimsPrincipal claimsPrincipal, Section section)
        {
            bool isModerator = false;

            do
            {
                if (claimsPrincipal == null)
                {
                    isModerator = false;
                    break;
                }

                var user = _userManager.GetUserAsync(claimsPrincipal).Result;

                if (user == null)
                {
                    isModerator = false;
                    break;
                }

                isModerator = section.Moderators.Contains(user);
            } while (false);

            return isModerator;
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
        [Authorize(Roles = "User")]
        public ActionResult Create(int sectionId)
        {
            var vm = new EditTopicViewModel();
            vm.SectionId = sectionId.ToString();

            return View(vm);
        }

        // POST: TopicsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([Bind("Title, Body, SectionId, Accessibility")] EditTopicViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUserAsync(User).Result;

                int sectionId = default;

                try
                {
                    sectionId = Convert.ToInt32(viewModel.SectionId);

                }
                catch(Exception)
                {
                    return BadRequest();
                }

                var section = _dbContext.Sections.FirstOrDefault(s => s.Id == sectionId);

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
                    Section = section,
                    Accessibility = (Accessibility) Enum.Parse(typeof(Accessibility), viewModel.Accessibility)
                };

                newTopic = _dbContext.Topics.Add(newTopic).Entity;
                await _dbContext.SaveChangesAsync();

                user.Topics.Add(newTopic);

                section.Topics.ToList().Add(newTopic);

                await _dbContext.SaveChangesAsync();

                ViewData["Message"] = $"Topic `{viewModel.Title}` successfully created.";
                ViewData["SectionId"] = viewModel.SectionId;

                return View("Success");
            }

            return View(viewModel);
        }

        // GET: TopicsController/Edit/5
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id)
        {
            var topic = _dbContext.Topics.FirstOrDefault(topic => topic.Id == id);
            topic.Section = _dbContext.Sections.FirstOrDefault(s => s.Id == topic.SectionId);

            if (topic == null)
            {
                return NotFound();
            }

            var sections = _dbContext.Sections.ToList();

            return View(new EditTopicViewModel(topic, sections, IsModeratorOfSection(User, topic.Section)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id, EditTopicViewModel viewModel)
        {
            List<string> errorMessages;

            if(!IsValid(viewModel, out errorMessages))
            {
                return View("Error", errorMessages);
            }

            var editedTopic = CreateEditedTopic(viewModel);

            if(editedTopic == null)
            {
                return NotFound();
            }

            try
            {
                _dbContext.Update(editedTopic);
                _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return View("Error", new List<string> { "Unable to save changes." });
            }

            ViewData["Message"] = "The topic is successfully edited.";
            ViewData["SectionId"] = viewModel.SectionId;

            return View("Success");
        }

        private Topic CreateEditedTopic(EditTopicViewModel viewModel)
        {
            var topic = _dbContext.Topics.FirstOrDefault(t => t.Id.Equals(viewModel.Id));

            if(topic == null)
            {
                return null;
            }

            topic.Title = viewModel.Title;
            topic.Body = viewModel.Body;
            topic.Editor = _userManager.GetUserAsync(User).Result;
            topic.Edited = DateTime.Now;
            topic.Accessibility = (Accessibility)Enum.Parse(typeof(Accessibility), viewModel.Accessibility);

            return topic;
        }

        private bool IsValid(EditTopicViewModel viewModel, out List<string> errorMessages)
        {
            bool isValid = true;

            errorMessages = new List<string>();

            var topic = _dbContext.Topics/*.AsNoTracking()*/.FirstOrDefault(t => t.Id == viewModel.Id);

            if (topic == null)
            {
                errorMessages.Add("Topic not found.");
                isValid = false;
            }

            var vm = new TopicViewModel(topic, Request);
            topic.Section = _dbContext.Sections.FirstOrDefault(s => s.Id == topic.SectionId);
            SetAccessibilityParams(vm, User, IsOwner(User, topic), topic.Section.Moderators, topic.Accessibility);

            if (!vm.CanEditTopic)
            {
                errorMessages.Add("No editing rights.");
                return false; // There is no point in validating the next data.
            }

            if (string.IsNullOrEmpty(viewModel.Title))
            {
                errorMessages.Add("Title required.");
                isValid = false;
            }

            try
            {
                int sectionId = Convert.ToInt32(viewModel.SectionId);

                if (_dbContext.Sections.FirstOrDefault(s => s.Id == sectionId) == null)
                {
                    errorMessages.Add("Invalid section id.");
                    isValid = false;
                }
            }
            catch
            {
                errorMessages.Add("Invalid section id.");
                isValid = false;
            }

            topic.Section = _dbContext.Sections.Where(s => s.Id == topic.SectionId).FirstOrDefault();

            return isValid;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Delete(int id)
        {
            var topic = _dbContext.Topics.Find(id);

            if (topic == null)
            {
                return NotFound();
            }

            try
            {
               if(!HaveUserRemovingPermissions(User, topic))
               {
                    return View("Error", "You don't have permissions to delete the topic.");
               }

               foreach(var post in topic.Posts)
               {
                    _dbContext.Posts.Remove(post);
                }

                _dbContext.Topics.Remove(topic);
                await _dbContext.SaveChangesAsync();

                ViewData["Message"] = $"Topic `{topic.Title}` successfully removed.";
                ViewData["SectionId"] = topic.SectionId;

                return View("Success");
            }
            catch (DbUpdateException /* ex */)
            {
                // TODO: Log the error (uncomment ex variable name and write a log).
                return View("Error", "There was a technical error while trying to delete a topic.");
            }
        }

        private bool HaveUserRemovingPermissions(ClaimsPrincipal claimsPrincipal, Topic topic)
        {
            bool haveRemovingPermissions = false;

            do
            {
                if (claimsPrincipal == null)
                {
                    haveRemovingPermissions = false;
                    break;
                }

                var user = _userManager.GetUserAsync(claimsPrincipal).Result;

                if (user == null)
                {
                    haveRemovingPermissions = false;
                    break;
                }

                bool isOwner = topic.AuthorId.Equals(user.Id);
                bool isSectionModerator = topic.Section.Moderators.Contains(user);
                bool isAdministrator = claimsPrincipal.IsInRole("Admin");

                haveRemovingPermissions = isOwner || isSectionModerator || isAdministrator;
            } while (false);

            return haveRemovingPermissions;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
