using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            _dbContext.Chapters.Update(firstChapter);
            _dbContext.Chapters.Update(secondChapter);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Chapter> FindByIdAsync(long id)
        {
            var chapter = await _dbContext.Chapters.FindAsync(id);
            return chapter;
        }

        public void UpdateChapter(Chapter chapter)
        {
            _dbContext.Chapters.Update(chapter);
            _dbContext.SaveChanges();
        }

        public void Add(Chapter chapter)
        {
            _dbContext.Chapters.Add(chapter);
            _dbContext.SaveChanges();
        }

        public Chapter FindByCompositionIdAndNumber(long compositionId, int number)
        {
            Chapter chapter = null;
            var chapters = _dbContext.Chapters.Where(ch => ch.CompositionId == compositionId).ToList();
            chapters = chapters.Where(ch => ch.Number == number).ToList();
            if (chapters.Any())
            {
                chapter = chapters.First();
            }

            return chapter;
        }

        public async Task RemoveById(long id)
        {
            var chapter = await FindByIdAsync(id);
            if (chapter != null)
            {
                var chaptersAfter = _dbContext.Chapters.Where(ch => ch.Number > chapter.Number);
                foreach (var ch in chaptersAfter)
                {
                    ch.Number -= 1;
                }
                _dbContext.Chapters.UpdateRange(chaptersAfter);
                _dbContext.Chapters.Remove(chapter);
                _dbContext.SaveChanges();
            }
        }

        public bool LastChapter(Chapter chapter)
        {
            var chapters = _dbContext.Chapters.Where(c => c.Number > chapter.Number);
            if (!chapters.Any())
            {
                return true;
            }

            return false;
        }

    }
}
