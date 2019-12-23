using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.ViewModels
{
    public class ReaderChapterViewModel
    {
        public string CompositionName { get; set; }
        public long CompositionId { get; set; }
        public long ChapterId { get; set; }

        public int ChapterNumber { get; set; }
        public List<ChapterNumberAndId> ChapterNumbersAndIds { get; set; }
        public string Text { get; set; }

        public List<string> LikeUserIds { get; set; }

        public byte Rating { get; set; }
    }
}
