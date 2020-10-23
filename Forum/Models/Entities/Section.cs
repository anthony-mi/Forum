using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Section
    {
        private readonly ILazyLoader _lazyLoader;

        private ICollection<Topic> _topics;
        private ICollection<Moderator> _moderators;

        public Section()
        {

        }

        public Section(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }

        [Index("IX_UniqueKeyInt", IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<Topic> Topics
        {
            get => _lazyLoader.Load(this, ref _topics);
            set => _topics = value;
        }

        public virtual ICollection<Moderator> Moderators
        {
            get => _lazyLoader.Load(this, ref _moderators);
            set => _moderators = value;
        }

        public Accessibility Accessibility { get; set; }
    }
}
