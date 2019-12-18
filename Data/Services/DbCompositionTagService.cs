using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbCompositionTagService
    {
        private AppDbContext _dbContext;

        public DbCompositionTagService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddCompositionTags(long compositionId, IQueryable<Tag> tags)
        {
            List<CompositionTag> list = new List<CompositionTag>();
            foreach (Tag tag in tags)
            {
                list.Add(new CompositionTag()
                {
                    CompositionId = compositionId,
                    TagId = tag.Id
                });
            }
            _dbContext.CompositionTags.AddRange(list);
            _dbContext.SaveChanges();
        }
    }
}
