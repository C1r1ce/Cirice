using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.ViewModels
{
    public class ChapterViewModel
    {
        public long CompositionId { get; set; }
        public long ChapterId { get; set; }
        public int Number { get; set; }

        [Required]
        [MinLength(10,ErrorMessage = "Not enough characters")]
        public string Text { get; set; }
    }
}
