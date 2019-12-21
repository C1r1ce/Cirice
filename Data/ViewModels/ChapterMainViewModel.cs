using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.ViewModels
{
    public class ChapterMainViewModel
    {
        public string CompositionName { get; set; }
        public long CompositionId { get; set; }
        public List<ChapterNumberAndId> ChapterNumbersAndIds { get; set; }

        [Required] 
        public long ChapterIdFirst { get; set; }

        [Required]
        public long ChapterIdSecond { get; set; }
    }

    public class ChapterNumberAndId
    {
        public int Number { get; set; }
        public long ChapterId { get; set; }
    }
}
