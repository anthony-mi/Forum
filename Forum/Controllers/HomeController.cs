using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Forum.Models;
using Forum.ViewModels;
using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SectionsController _sectionsController;

        private const int _MAX_COUNT_OF_LAST_TOPICS = 5;

        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDbContext dbContext, 
            UserManager<User> userManager,
            SectionsController sectionsController)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _sectionsController = sectionsController;
        }

        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel(Request);
            _sectionsController.ControllerContext = ControllerContext;
            homeViewModel.Sections = _sectionsController.GetAllSections(_MAX_COUNT_OF_LAST_TOPICS).Result;
            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
