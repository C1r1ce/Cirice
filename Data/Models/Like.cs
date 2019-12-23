using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Models
{
    public class Like
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long CompositionId { get; set; }
        public DateTime DateTime { get; set; }
        public long ChapterId { get; set; }
    }
}
