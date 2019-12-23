using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.ViewModels
{
    public class CompositionViewModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string GenreString { get; set; }
        public DateTime LastPublication { get; set; }
        public string Annotation { get; set; }
        public string ImgSource { get; set; }
        public IQueryable<Tag> Tags { get; set; }
        public double Rating { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public long CompositionId { get; set; }
        public string UserId { get; set; }
    }
}
