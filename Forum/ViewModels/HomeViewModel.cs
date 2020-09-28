using Forum.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class HomeViewModel
    {
        private const int countOfTopicsInsideSection = 5;

        public IEnumerable<SectionViewModel> Sections { get; set; }

        public HomeViewModel(ApplicationDbContext dbContext)
        {
            Sections = new List<SectionViewModel>();

            var sections = dbContext.Sections.ToArray();

            foreach(var section in sections)
            {
                var vm = new SectionViewModel(section, countOfTopicsInsideSection, dbContext);
                (Sections as List<SectionViewModel>).Add(vm);
            }
        }
    }
}
