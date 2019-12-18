using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Models
{
    public class Composition
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int GenreId { get; set; }
        public DateTime FirstPublication { get; set; }
        public DateTime LastPublication { get; set; }
        public string Annotation { get; set; }
        public string ImgSource { get; set; }
    }
}
