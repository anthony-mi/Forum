using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;

namespace Forum.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public virtual User Author { get; set; }

        public DateTime Created { get; set; }

        public virtual int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public PostViewModel()
        {

        }

        public PostViewModel(Post post)
        {
            Id = post.Id;
            Body = post.Body;
            Author = post.Author;
            Created = post.Created;
            TopicId = post.TopicId;
            Topic = post.Topic;
        }
    }
}
