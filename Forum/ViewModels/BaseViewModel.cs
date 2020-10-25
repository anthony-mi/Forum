using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
