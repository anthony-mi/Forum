using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Forum.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        private const int _countOfTopics = 5;
        private const int _countOfPosts = 5;

        public uint CountOfMessages { get; set; }

        public string PathToProfilePicture { get; set; }

        public string About { get; set; }

        public DateTime LastActivity { get; set; }
        public DateTime Registration { get; set; }

        public List<Topic> LastTopics { get; set; }
        public List<Post> LastPosts { get; set; }

        public UserProfileViewModel(User user, ApplicationDbContext dbContext, HttpRequest request) : base(request)
        {
            CountOfMessages = user.CountOfMessages;
            PathToProfilePicture = $"Resources/Images/{user.ProfilePicture.Filename}";
            About = user.About;
            LastActivity = user.LastActivity;
            Registration = user.Registration;
            LastTopics = GetLastTopics(user, dbContext, _countOfTopics);
            LastPosts = GetLastPosts(user, dbContext, _countOfPosts);
        }

        private List<Post> GetLastPosts(User user, ApplicationDbContext dbContext, int countOfPosts)
        {
            var posts = dbContext.Posts.Where(t => t.AuthorId == user.Id).ToList();
            return (List<Post>) posts.OrderBy(t => t.Created).Take(countOfPosts);
        }

        private List<Topic> GetLastTopics(User user, ApplicationDbContext dbContext, int countOfTopics)
        {
            var topics = dbContext.Topics.Where(t => t.AuthorId == user.Id).ToList();
            return (List<Topic>) topics.OrderBy(t => t.Created).Take(countOfTopics);
        }
    }
}
