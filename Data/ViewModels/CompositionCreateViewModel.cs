using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.ViewModels
{
    public class CompositionCreateViewModel
    {
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string GenreString { get; set; }

        [Required]
        public string Tags { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Annotation { get; set; }
    }
}
