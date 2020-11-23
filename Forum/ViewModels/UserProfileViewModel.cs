using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Forum.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public uint CountOfMessages { get; set; }

        public string PathToProfilePicture { get; set; }

        public string About { get; set; }

        public DateTime LastActivity { get; set; }
        public DateTime Registration { get; set; }

        public List<Topic> LastTopics { get; set; }
        public List<Post> LastPosts { get; set; }

        public IList<string> Roles { get; set; }

        public bool CanEditRoles { get; set; }

        public UserProfileViewModel(
            User user,
            HttpRequest request,
            List<Topic> lastTopics,
            List<Post> lastPosts,
            IList<string> roles,
            bool canEditRoles) : base(request)
        {
            Id = user.Id;
            Username = user.UserName;
            CountOfMessages = user.CountOfMessages;
            PathToProfilePicture = $"Resources/Images/{user.ProfilePicture.Filename}";
            About = user.About;
            LastActivity = user.LastActivity;
            Registration = user.Registration;
            LastTopics = lastTopics;
            LastPosts = lastPosts;
            Roles = roles;
            CanEditRoles = canEditRoles;
        }
    }
}
