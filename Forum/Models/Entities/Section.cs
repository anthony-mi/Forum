using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Section
    {
        public int Id { get; set; }
        [Index("IX_UniqueKeyInt", IsUnique = true)]
        public string Name { get; set; }
        public virtual IEnumerable<Topic> Topics { get; set; }
        public Accessibility Accessibility { get; set; }
    }
}
