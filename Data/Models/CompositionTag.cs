using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Models
{
    public class CompositionTag
    {
        public long Id { get; set; }
        public long CompositionId { get; set; }
        public int TagId { get; set; }
    }
}
