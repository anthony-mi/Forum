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

        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Edited { get; set; }

        public int? EditorId { get; set; }

        public virtual User Editor { get; set; }
    }
}
