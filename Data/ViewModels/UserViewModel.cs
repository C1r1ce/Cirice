using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.ViewModels
{
    public class UserViewModel
    {
        public List<CompositionViewModel> CompositionViewModels { get; set; }
        public string UserName { get; set; }
        public string UserAbout { get; set; }

        public string UserId { get; set; }
    }
}
