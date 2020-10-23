using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Moderator : User
    {
        private ICollection<Section> _moderatableSections;

        public Moderator()
        {

        }

        public Moderator(ILazyLoader lazyLoader) : base(lazyLoader)
        {
            
        }

        public virtual ICollection<Section> ModeratableSections
        {
            get => _lazyLoader.Load(this, ref _moderatableSections);
            set => _moderatableSections = value;
        }
    }
}
