using System;
using System.Collections.Generic;

namespace Forum.ViewModels
{
    public class ErrorsViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public IList<string> Errors { get; set; }
        public string GoBackUrl { get; set; }
    }
}
