using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Forum.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly UsersController _usersController;

        public RolesController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<User> userManager, 
            ApplicationDbContext dbContext,
            UsersController usersController)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _usersController = usersController;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var currentUserRoles = await _usersController.GetUserRolesAsync(User);

            if(!currentUserRoles.Contains("Moderator") && !currentUserRoles.Contains("Admin"))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();

                var moderatedByUserSections = new List<Section>();

                foreach(var sm in _dbContext.SectionModerators)
                {
                    if(sm.Moderator.Equals(user))
                    {
                        moderatedByUserSections.Add(sm.Section);
                    }
                }

                var model = new ChangeRolesViewModel(user.Id, userRoles, allRoles, moderatedByUserSections, GetAvailableSections(currentUserRoles));

                return View(model);
            }

            return NotFound();
        }

        private IList<Section> GetAvailableSections(IList<string> currentUserRoles)
        {
            IList<Section> sections = new List<Section>();

            if(currentUserRoles.Contains("Admin"))
            {
                sections =_dbContext.Sections.ToList();
            }
            else if(currentUserRoles.Contains("Moderator"))
            {
                var currentUser = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;

                foreach (var sm in _dbContext.SectionModerators)
                {
                    if (sm.Moderator.Equals(currentUser))
                    {
                        sections.Add(sm.Section);
                    }
                }
            }

            return sections;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ChangeRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            IList<string> errorMessages = new List<string>();

            if(!IsValid(model, out errorMessages))
            {
                var viewModel = new ErrorsViewModel
                {
                    Title = "Roles validation error",
                    Text = "Validation errors occurred while editing user roles",
                    Errors = errorMessages,
                    GoBackUrl = Url.Action("Edit", "Roles", new { userId = model.UserId })
                };

                return View("Errors", viewModel);
            }

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            bool userWasModerator = userRoles.Contains("Moderator");

            if (userWasModerator && model.UserRoles.Contains("Moderator")) // only sections were changed
            {
                var removedReferences = _dbContext.SectionModerators
                                            .Where(sm => 
                                            !model.ModeratedByUserSections.Contains(sm.SectionId) &&
                                            model.UserId.Equals(sm.ModeratorId)).ToList();
                _dbContext.SectionModerators.RemoveRange(removedReferences);

                var userModeratableSections = _dbContext.SectionModerators.Where(sm => sm.ModeratorId.Equals(model.UserId)).Select(sm => sm.SectionId).ToList();

               foreach(var sectionId in model.ModeratedByUserSections)
               {
                    if(!userModeratableSections.Contains(sectionId))
                    {
                        _dbContext.SectionModerators.Add(new SectionModerator { ModeratorId = model.UserId, SectionId = sectionId });
                    }
               }
            }
            else if (!userWasModerator && model.UserRoles.Contains("Moderator")) // user assigned as moderator
            {
                foreach (var section in _dbContext.Sections)
                {
                    if (model.ModeratedByUserSections.Contains(section.Id))
                    {
                        var sectionModerator = new SectionModerator
                        { 
                            ModeratorId = user.Id,
                            SectionId = section.Id,
                        };

                        _dbContext.SectionModerators.Add(sectionModerator);
                    }
                }
            }
            else if (userWasModerator && !model.UserRoles.Contains("Moderator")) // user removed from moderators
            {
                var moderatableByUserSections = _dbContext.SectionModerators.Where(sm => sm.ModeratorId.Equals(user.Id));
                _dbContext.SectionModerators.RemoveRange(moderatableByUserSections);
            }

            _dbContext.SaveChanges();

            var addedRoles = model.UserRoles.Except(userRoles);
            var removedRoles = userRoles.Except(model.UserRoles);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return View("Success");
        }

        private bool IsValid(ChangeRolesViewModel model, out IList<string> errorMessages)
        {
            bool isValid = true;
            var errors = new List<string>();

            if(_userManager.FindByIdAsync(model.UserId).Result == null)
            {
                errors.Add("User not found.");
                isValid = false;
            }

            if(model.UserRoles.Count == 0)
            {
                errors.Add("Choose at least one role.");
                isValid = false;
            }

            if(model.UserRoles.Contains("Moderator") && model.ModeratedByUserSections.Count == 0)
            {
                errors.Add("Choose at least one moderatable section.");
                isValid = false;
            }

            errorMessages = errors;

            return isValid;
        }
    }
}
