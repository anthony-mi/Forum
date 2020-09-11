using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class User: IdentityUser
    {
        [DefaultValue(0)]
        public uint CountOfMessages { get; set; }
        public int? ProfilePictureId { get; set; }
        public virtual Image ProfilePicture { get; set; }
        [DefaultValue("")]
        public string About { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime Registration { get; set; }
    }
}
