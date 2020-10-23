using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class User: IdentityUser
    {
        protected readonly ILazyLoader _lazyLoader;

        private ICollection<Post> _posts;
        private ICollection<Topic> _topics;

        public User()
        {
        }

        public User(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        [DefaultValue(0)]
        public uint CountOfMessages { get; set; }

        public int? ProfilePictureId { get; set; }

        public virtual Image ProfilePicture { get; set; }

        [DefaultValue("")]
        public string About { get; set; }

        public DateTime LastActivity { get; set; }

        public DateTime Registration { get; set; }

        public virtual ICollection<Post> Posts
        {
            get => _lazyLoader.Load(this, ref _posts);
            set => _posts = value;
        }

        public virtual ICollection<Topic> Topics
        {
            get => _lazyLoader.Load(this, ref _topics);
            set => _topics = value;
        }
    }
}
