using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cirice.Data.Services
{
    public class DbGenreService
    {
        private AppDbContext _dbContext;
        public DbGenreService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Genre FindByGenreString(string GenreString)
        {
            var genres = _dbContext.Genres.Where(g => g.GenreString.Equals(GenreString));
            Genre result = null;
            if (genres.Any())
            {
                result = genres.First();
            }

            return result;
        }
        public IQueryable<Genre> GetGenres()
        {
            return _dbContext.Set<Genre>();
        }

        public async Task AddGenresAsync(ICollection<Genre> genres)
        {
            await _dbContext.Genres.AddRangeAsync(genres);
            await _dbContext.SaveChangesAsync();
        }

        public void AddGenre(Genre genre)
        {
            _dbContext.Genres.Add(genre);
            _dbContext.SaveChanges();
        }
    }
}
