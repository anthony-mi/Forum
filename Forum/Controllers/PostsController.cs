using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public PostsController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: PostsController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: PostsController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // POST: PostsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(string body, int topicId)
        {
            if(string.IsNullOrEmpty(body))
            {
                return Error();
            };

            var topic = _dbContext.Topics.FirstOrDefault(t => t.Id == topicId);

            if(topic == null) // Topic doesn't exists.
            {
                return Error();
            }

            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var post = new Post
            {
                Body = body,
                Author = user,
                Created = DateTime.Now,
                Topic = topic
            };

            post = _dbContext.Posts.Add(post).Entity;
            await _dbContext.SaveChangesAsync();

            topic.Posts.ToList().Add(post);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Details", "Topics", new { id = topicId });
        }

        //// POST: PostsController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: PostsController/Edit/5
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PostsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
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

        // GET: PostsController/Delete/5
        [Authorize(Roles = "User")]
        public ActionResult Delete(int id)
        {
            var post = _dbContext.Posts.Include(p => p.Topic)/*.AsNoTracking()*/.FirstOrDefault(p => p.Id == id);

            if(post == null)
            {
                return NotFound();
            }

            _dbContext.Posts.Remove(post);

            _dbContext.SaveChangesAsync();

            return RedirectToAction("Details", "Topics", new { id = post.Topic.Id });
        }

        // POST: PostsController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
