using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Services
{
    public class DbCommentService
    {
        private AppDbContext _dbContext;
        public DbCommentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetCommentCountByCompositionId(long compositionId)
        {
            return _dbContext.Comments.Where(c => c.CompositionId == compositionId).ToList().Count;
        }

        public void RemoveByUserId(string userId)
        {
            var comments = _dbContext.Comments.Where(c => c.UserId.Equals(userId)).ToList();
            _dbContext.Comments.RemoveRange(comments);
            _dbContext.SaveChanges();
        }
    }
}
