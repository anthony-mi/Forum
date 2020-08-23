using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class User: IdentityUser
    {
        [DefaultValue(0)]
        public uint CountOfMessages { get; set; }
        [DefaultValue(0)]
        public int Reputation { get; set; }
        public Image ProfilePicture { get; set; }
        public string About { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime Registration { get; set; }
    }
}
