using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbChapterService
    {
        private AppDbContext _dbContext;
        public DbChapterService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Chapter> FindByCompositionId(long compositionId)
        {
            var chapters = _dbContext.Chapters.Where(ch => ch.CompositionId == compositionId).OrderBy(ch=>ch.Number).ToList();
            return chapters;
        }

        public async Task SwapAsync(long firstId, long secondId)
        {
            var firstChapter = await _dbContext.Chapters.FindAsync(firstId);
            var secondChapter = await _dbContext.Chapters.FindAsync(secondId);
            var temp = firstChapter;
            firstChapter.Text = secondChapter.Text;
            secondChapter.Text = temp.Text;
            await _dbContext.SaveChangesAsync();
        }
    }
}
