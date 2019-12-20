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
        
    }
}
