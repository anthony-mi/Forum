using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class TopicViewModel
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Body { get; set; }

        public int SectionId { get; set; }
    }
}
