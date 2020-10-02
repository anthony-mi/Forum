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
        public IEnumerable<Topic> LastTopics { get; set; }
        public Accessibility Accessibility { get; set; }

        public SectionViewModel(
            Section section, 
            int countOfTopics, 
            ApplicationDbContext dbContext,
            HttpRequest request) : base(request)
        {
            Id = section.Id;
            Name = section.Name;
            Accessibility = section.Accessibility;

            if(section.Topics == null)
            {
                section.Topics = dbContext.Topics.Where(t => t.SectionId == section.Id).ToList();
            }

            LastTopics = section.Topics.OrderBy(t => t.Created).Take(countOfTopics);
        }

        //private IEnumerable<Topic> GetLastTopics(string sectionName, ApplicationDbContext dbContext, int countOfTopics)
        //{
        //    var section = dbContext.Sections.Where(s => s.Name.Equals(sectionName)).FirstOrDefault();

        //    if(section == null)
        //    {
        //        return new List<Topic>();
        //    }

        //    if(section.Topics == null)
        //    {
        //        return new List<Topic>();
        //    }
            
        //    return (List<Topic>) section.Topics.OrderBy(t => t.Created).Take(countOfTopics);
        //}
    }
}
