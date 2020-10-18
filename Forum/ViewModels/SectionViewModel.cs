using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace Forum.ViewModels
{
    public class SectionViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Topic> LastTopics { get; set; }

        public SectionViewModel(Section section, HttpRequest request) : base(request)
        {
            Id = section.Id;
            Name = section.Name;
        }
    }
}
