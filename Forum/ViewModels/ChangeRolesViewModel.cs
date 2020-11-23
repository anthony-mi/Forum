using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Forum.ViewModels
{
    public class ChangeRolesViewModel
    {
        public string UserId { get; set; }
        public IList<SelectListItem> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<SelectListItem> AvailableSections { get; set; }
        public IList<int> ModeratedByUserSections { get; set; }

        public ChangeRolesViewModel()
        {
            UserId = string.Empty;
            AllRoles = new List<SelectListItem>();
            UserRoles = new List<string>();
            AvailableSections = new List<SelectListItem>();
            ModeratedByUserSections = new List<int>();
        }

        public ChangeRolesViewModel(
            string userId, 
            IList<string> userRoles, 
            IList<IdentityRole> allRoles,
            IList<Section> moderatedByUserSections,
            IList<Section> availableSections)
        {
            UserId = userId;
            UserRoles = userRoles;

            ModeratedByUserSections = new List<int>();
            foreach(var section in moderatedByUserSections)
            {
                ModeratedByUserSections.Add(section.Id);
            }

            AllRoles = new List<SelectListItem>();
            foreach(var role in allRoles)
            {
                var item = new SelectListItem(role.Name, role.Name/*role.Id*/);
                //item.Selected = userRoles.Contains(item.Text);

                AllRoles.Add(item);
            }

            AvailableSections = new List<SelectListItem>();
            foreach (var section in availableSections)
            {
                var item = new SelectListItem(section.Name, section.Id.ToString());
                AvailableSections.Add(item);
            }
        }
    }
}
