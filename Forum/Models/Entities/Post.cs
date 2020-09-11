using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public int AuthorId { get; set; }

        public virtual User Author { get; set; }

        public DateTime Creation { get; set; }

        public DateTime? Editing { get; set; }

        public int? EditorId { get; set; }

        public virtual User Editor { get; set; }
    }
}
