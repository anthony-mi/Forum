using Microsoft.AspNetCore.Http;

namespace Forum.ViewModels
{
    public class BaseViewModel
    {
        public string RequestScheme { get; set; }
        public string RequestHost { get; set; }

        public string CurrentUserId { get; set; }

        public BaseViewModel(HttpRequest request)
        {
            RequestScheme = request.Scheme;
            RequestHost = request.Host.Value;
        }
    }
}
