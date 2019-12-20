using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbLikeService
    {
        private AppDbContext _dbContext;

        public DbLikeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetLikeCountByCompositionId(long id)
        {
            var likes = _dbContext.Likes.Where(l => id == l.CompositionId);
            return likes.Count();
        }
    }
}
