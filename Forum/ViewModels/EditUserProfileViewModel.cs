using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Forum.ViewModels
{
    public class EditUserProfileViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string About { get; set; }

        public IFormFile NewProfilePicture { get; set; }

        public EditUserProfileViewModel()
        {

        }

        public EditUserProfileViewModel(User user)
        {
            Id = user.Id;
            Username = user.UserName;
            About = user.About;
        }
    }
}
