using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Controllers
{
    public class SectionsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly UsersController _usersController;

        private const int topicsPerPage = 20;

        public SectionsController(
            ILogger<HomeController> logger, 
            ApplicationDbContext dbContext, 
            UserManager<User> userManager,
            UsersController usersController)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _usersController = usersController;
        }

        public IActionResult Index(int id)
        {
            var section = _dbContext.Sections.Include(s => s.Topics).FirstOrDefault(s => s.Id == id);
            //section.Topics = _dbContext.Entry(section).Collection(s => s.Topics).CurrentValue;

            if (section == null)
            {
                return Error();
            }

            // Explicit loading of dependent data. The EF did not perform either lazy or explicit data loading.
            // Therefore, I had to implement this little crutch.
            // Discussion of this problem: stackoverflow.com/questions/64094376
            section.Topics = _dbContext.Topics.Where(t => t.SectionId == section.Id).Include(t => t.Author).ToList();

            foreach(var topic in section.Topics)
            {
                topic.Author = _dbContext.Users.FirstOrDefault(u => u.Id == topic.AuthorId);
                topic.Posts = _dbContext.Posts.Where(t => t.TopicId == topic.Id).ToList();
            }

            //var userRoles = ControllerContext.HttpContext.Request;
            var viewModel = CreateSectionViewModelAsync(section, null, topicsPerPage).Result;
            return View(viewModel);
        }

        public async Task<IList<SectionViewModel>> GetAllSections(int maxCountOfLastTopics)
        {
            var result = new List<SectionViewModel>();
            var sections = _dbContext.Sections.ToArray();
            var userRoles = _usersController.GetUserRoles(User).Result;

            foreach (var section in sections)
            {
                bool displaySection =
                    AccessibilityChecker.HasAccess(userRoles, section.Accessibility);

                if (!displaySection)
                {
                    continue;
                }

                var sectionViewModel = CreateSectionViewModelAsync(section, userRoles, maxCountOfLastTopics).Result;
                section.Topics = _dbContext.Topics.Where(t => t.SectionId == section.Id).Include(t => t.Author).ToList();
                result.Add(sectionViewModel);
            }

            return result;
        }

        private async Task<SectionViewModel> CreateSectionViewModelAsync(Section section, IList<string> userRoles, int maxCountOfLastTopics)
        {
            var sectionVm = new SectionViewModel(section, ControllerContext.HttpContext.Request);

            if (section.Topics == null)
            {
                section.Topics = _dbContext.Topics.Where(t => t.SectionId == section.Id).ToList();
            }

            sectionVm.LastTopics = new List<Topic>();

            var topics = section.Topics.OrderBy(t => t.Created);

            foreach (var topic in topics)
            {
                if (sectionVm.LastTopics.Count() == maxCountOfLastTopics)
                {
                    break;
                }

                bool displayTopic =
                     AccessibilityChecker.HasAccess(userRoles, section.Accessibility);

                if (!displayTopic)
                {
                    continue;
                }

                topic.Author = _dbContext.Users.FirstOrDefault(u => u.Id == topic.AuthorId);
                topic.Posts = _dbContext.Posts.Where(t => t.TopicId == topic.Id).ToList();

                sectionVm.LastTopics.Add(topic);
            }

            return sectionVm;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
