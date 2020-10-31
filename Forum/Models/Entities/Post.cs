using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Post
    {
        private readonly ILazyLoader _lazyLoader;

        private User _author;
        private User _editor;
        private Topic _topic;

        public Post()
        {
        }

        public Post(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public string Body { get; set; }

        public string AuthorId { get; set; }

        public virtual User Author
        {
            get => _lazyLoader.Load(this, ref _author);
            set => _author = value;
        }

        public DateTime Created { get; set; }

        public DateTime? Edited { get; set; }

        public string EditorId { get; set; }

        public virtual User Editor
        {
            get => _lazyLoader.Load(this, ref _editor);
            set => _editor = value;
        }

        public int TopicId { get; set; }

        public virtual Topic Topic
        {
            get => _lazyLoader.Load(this, ref _topic);
            set => _topic = value;
        }
    }
}
