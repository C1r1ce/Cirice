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
            var likes = _dbContext.Likes.Where(l => id == l.CompositionId).ToList();
            return likes.Count();
        }

        public List<Like> GetLikesByChapterId(long id)
        {
            return _dbContext.Likes.Where(l => l.ChapterId == id).ToList();
        }

        public void Add(Like like)
        {
            _dbContext.Likes.Add(like);
            _dbContext.SaveChanges();
        }

        public void Remove(Like like)
        {
            _dbContext.Likes.Remove(like);
            _dbContext.SaveChanges();
        }

        public Like FindByUserIdAndChapterId(string userId, long chapterId)
        {
            var likes = _dbContext.Likes.Where(l => l.ChapterId == chapterId).ToList();
            likes = likes.Where(l => l.UserId.Equals(userId)).ToList();
            Like result = null;
            if (likes.Any())
            {
                result = likes.First();
            }

            return result;
        }

        public void RemoveByUserId(string userId)
        {
            var likes = _dbContext.Likes.Where(l => l.UserId.Equals(userId)).ToList();
            _dbContext.RemoveRange(likes);
            _dbContext.SaveChanges();
        }
    }
}
