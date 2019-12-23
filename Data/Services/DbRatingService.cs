using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbRatingService
    {
        private AppDbContext _dbContext;

        public DbRatingService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public double GetAverageRatingByCompositionId(long id)
        {
            double result = 0;
            var ratings = _dbContext.Ratings.Where(r => r.CompositionId == id);
            if (ratings.Any())
            {
                double averageMark = 0;
                foreach (Rating rating in ratings)
                {
                    averageMark += rating.Mark;
                }
                averageMark /= ratings.Count();
                result = Math.Round(averageMark, 1);
            }
            return result;
        }

        public void Add(Rating rating)
        {
            _dbContext.Ratings.Add(rating);
            _dbContext.SaveChanges();
        }

        public Rating FindByUserIdAndCompositionId(string userId, long compositionId)
        {
            var ratings = _dbContext.Ratings.Where(r => compositionId == r.CompositionId).ToList();
            ratings = ratings.Where(r => r.UserId.Equals(userId)).ToList();
            Rating result = null;
            if (ratings.Any())
            {
                result = ratings.First();
            }

            return result;
        }

        public void UpdateMark(string userId, long compositionId, byte mark)
        {
            var rating = FindByUserIdAndCompositionId(userId, compositionId);
            rating.Mark = mark;
            _dbContext.Ratings.Update(rating);
            _dbContext.SaveChanges();
        }

        public void RemoveByUserId(string userId)
        {
            var ratings = _dbContext.Ratings.Where(r => r.UserId.Equals(userId)).ToList();
            _dbContext.Ratings.RemoveRange(ratings);
            _dbContext.SaveChanges();
        }
        
    }
}
