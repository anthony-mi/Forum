using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models.Entities
{
    public class Image
    {
        //[Key]
        //[ForeignKey("ProfilePictureId ")]
        public int Id { get; set; }
        public string Filename { get; set; }
    }
}
