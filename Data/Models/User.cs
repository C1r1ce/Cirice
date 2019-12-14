using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Cirice.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public string ImgSource { get; set; }
        public string About { get; set; }

    }
}
