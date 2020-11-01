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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // POST: PostsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(string body, int topicId)
        {
            if(string.IsNullOrEmpty(body))
            {
                return RedirectToAction("Details", "Topics", new { id = topicId });
            };

            var topic = _dbContext.Topics.FirstOrDefault(t => t.Id == topicId);

            if(topic == null) // Topic doesn't exists.
            {
                return RedirectToAction("Details", "Topics", new { id = topicId });
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
            user.CountOfMessages++;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Details", "Topics", new { id = topicId });
        }

        // GET: PostsController/Edit/5
        [Authorize(Roles = "User")]
        public ActionResult Edit(int id)
        {
            var post = _dbContext.Posts.Find(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(new PostViewModel(post));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Edit(int id, PostViewModel viewModel)
        {
            List<string> errorMessages;

            if (!IsValid(viewModel, out errorMessages))
            {
                return View("Error", errorMessages);
            }

            var editedPost = CreateEditedPostAsync(viewModel).Result;

            if (editedPost == null)
            {
                return NotFound();
            }

            try
            {
                _dbContext.Update(editedPost);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return View("Error", new List<string> { "Unable to save changes." });
            }

            ViewData["Message"] = "The post is successfully edited.";
            ViewData["TopicId"] = viewModel.TopicId;

            return View("Success");
        }

        private bool IsValid(PostViewModel viewModel, out List<string> errorMessages)
        {
            bool isValid = true;

            errorMessages = new List<string>();

            var post = _dbContext.Posts/*.AsNoTracking()*/.Find(viewModel.Id);

            if (post == null)
            {
                errorMessages.Add("Post not found.");
                isValid = false;
            }

            var topic = _dbContext.Topics.Find(viewModel.TopicId);

            if (topic == null)
            {
                errorMessages.Add("Topic not found.");
                isValid = false;
            }

            var vm = new PostViewModel(post);

            if (!HaveUserEditingPermissions(User, post))
            {
                errorMessages.Add("No editing permissions.");
                isValid = false; // There is no point in validating the next data.
            }

            if (string.IsNullOrEmpty(viewModel.Body))
            {
                errorMessages.Add("Body required.");
                isValid = false;
            }

            return isValid;
        }

        private bool HaveUserEditingPermissions(ClaimsPrincipal claimsPrincipal, Post post)
        {
            bool haveEditingPermissions = false;

            do
            {
                if (claimsPrincipal == null)
                {
                    haveEditingPermissions = false;
                    break;
                }

                var user = _userManager.GetUserAsync(claimsPrincipal).Result;

                if (user == null)
                {
                    haveEditingPermissions = false;
                    break;
                }

                bool isOwner = post.AuthorId.Equals(user.Id);
                bool isSectionModerator = post.Topic.Section.Moderators.Contains(user);
                bool isAdministrator = claimsPrincipal.IsInRole("Admin");

                haveEditingPermissions = isOwner || isSectionModerator || isAdministrator;
            } while (false);

            return haveEditingPermissions;
        }

        private async Task<Post> CreateEditedPostAsync(PostViewModel viewModel)
        {
            var post = _dbContext.Posts.Find(viewModel.Id);

            if (post == null)
            {
                return null;
            }

            post.Body = viewModel.Body;
            post.TopicId = viewModel.TopicId;
            post.Topic = _dbContext.Topics.Find(viewModel.Id);
            post.Edited = DateTime.Now;
            post.Editor = _userManager.GetUserAsync(User).Result;

            return post;
        }

        // GET: PostsController/Delete/5
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Delete(int id)
        {
            var post = _dbContext.Posts/*.AsNoTracking()*/.Find(id);

            if(post == null)
            {
                return NotFound();
            }

            if (!HaveUserRemovingPermissions(User, post))
            {
                return View("Error", "You don't have permissions to delete the post.");
            }

            post.Topic.Posts.Remove(post);
            _dbContext.Posts.Remove(post);

            _userManager.GetUserAsync(User).Result.CountOfMessages--;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Details", "Topics", new { id = post.TopicId });
        }

        private bool HaveUserRemovingPermissions(ClaimsPrincipal claimsPrincipal, Post post)
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

                bool isOwner = post.AuthorId.Equals(user.Id);
                bool isSectionModerator = post.Topic.Section.Moderators.Contains(user);
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
