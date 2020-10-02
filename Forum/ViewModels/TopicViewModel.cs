using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels
{
    public class TopicViewModel : BaseViewModel
    {
        public TopicViewModel(HttpRequest request) : base(request)
        {
        }

        public TopicViewModel(Topic topic, HttpRequest request) : base(request)
        {
            Id = topic.Id;
            Title = topic.Title;
            Body = topic.Body;
            SectionId = topic.SectionId;
            Author = topic.Author;
            Posts = ConvertToPostViewModel(topic.Posts);
        }

        private IEnumerable<PostViewModel> ConvertToPostViewModel(ICollection<Post> posts)
        {
            var list = new List<PostViewModel>();

            foreach(var post in posts)
            {
                list.Add(new PostViewModel(post));
            }

            return list;
        }

        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Body { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public int SectionId { get; set; }

        public virtual User Author { get; set; }
    }
}
