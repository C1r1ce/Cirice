using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CompositionId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
