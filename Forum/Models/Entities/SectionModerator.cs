using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class SectionModerator
    {
        private readonly ILazyLoader _lazyLoader;

        private Section _section;
        private User _moderator;

        public SectionModerator()
        {

        }

        public SectionModerator(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int SectionId { get; set; }
        public virtual Section Section
        {
            get => _lazyLoader.Load(this, ref _section);
            set => _section = value;
        }

        public string ModeratorId { get; set; }
        public User Moderator
        {
            get => _lazyLoader.Load(this, ref _moderator);
            set => _moderator = value;
        }
    }
}
