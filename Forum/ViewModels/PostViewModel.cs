using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;

namespace Forum.ViewModels
{
    public class PostViewModel
    {
        public string Body { get; set; }

        public virtual User Author { get; set; }

        public DateTime Created { get; set; }

        public PostViewModel(Post post)
        {
            Body = post.Body;
            Author = post.Author;
            Created = post.Created;
        }
    }
}
