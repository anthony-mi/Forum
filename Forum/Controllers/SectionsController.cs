using Forum.Data;
using Forum.Models;
using Forum.ViewModels;
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

        private const int topicsPerPage = 20;

        public SectionsController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(int id)
        {
            var section = _dbContext.Sections.Include(s => s.Topics).FirstOrDefault(s => s.Id == id);
            //section.Topics = _dbContext.Entry(section).Collection(s => s.Topics).CurrentValue;

            if (section == null)
            {
                return Error();
            }

            // Explicit loading of dependent data. The system did not perform either lazy or explicit data loading.
            // Therefore, I had to implement this little crutch.
            // Discussion of this problem: stackoverflow.com/questions/64094376
            section.Topics = _dbContext.Topics.Where(t => t.SectionId == section.Id).Include(t => t.Author).ToList();

            foreach(var topic in section.Topics)
            {
                topic.Author = _dbContext.Users.FirstOrDefault(u => u.Id == topic.AuthorId);
                topic.Posts = _dbContext.Posts.Where(t => t.TopicId == topic.Id).ToList();
            }

            var viewModel = new SectionViewModel(section, topicsPerPage, _dbContext);
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
