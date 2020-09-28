using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public TopicsController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: TopicsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TopicsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TopicsController/Create
        public ActionResult Create(int sectionId)
        {
            return View(new TopicViewModel { SectionId = sectionId });
        }

        // POST: TopicsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,SectionId")] TopicViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUserAsync(HttpContext.User).Result;

                var section = _dbContext.Sections.Where(s => s.Id == viewModel.SectionId).FirstOrDefault();

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
