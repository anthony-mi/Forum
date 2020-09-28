using Forum.Data;
using Forum.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Forum.ViewModels
{
    public class SectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Topic> LastTopics { get; set; }

        public SectionViewModel(Section section, int countOfTopics, ApplicationDbContext dbContext)
        {
            Id = section.Id;
            Name = section.Name;

            if(section.Topics == null)
            {
                LastTopics = new List<Topic>();
            }
            else
            {
                LastTopics = section.Topics.OrderBy(t => t.Created).Take(countOfTopics);
            }
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
