using Forum.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private const int countOfTopicsInsideSection = 5;

        public IList<SectionViewModel> Sections { get; set; }

        public HomeViewModel(HttpRequest request) : base(request)
        {
            
        }
    }
}
