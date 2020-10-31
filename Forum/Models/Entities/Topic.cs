using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Forum.Models.Entities
{
    public class Topic
    {
        private readonly ILazyLoader _lazyLoader;

        private ICollection<Post> _posts;
        private User _author;
        private User _editor;
        private Section _section;

        public Topic()
        {
        }

        public Topic(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        public string Title { get; set; }

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

        public virtual ICollection<Post> Posts
        {
            get => _lazyLoader.Load(this, ref _posts);
            set => _posts = value;
        }

        public int SectionId { get; set; }

        public virtual Section Section
        {
            get => _lazyLoader.Load(this, ref _section);
            set => _section = value;
        }

        public Accessibility Accessibility { get; set; }
    }
}
