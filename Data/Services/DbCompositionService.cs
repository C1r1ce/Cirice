using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Cirice.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cirice.Data.Services
{
    public class DbCompositionService
    {
        private AppDbContext _dbContext;

        public DbCompositionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

//        public long AddCompositionAndReturnId(Composition composition)
//        {
//            _dbContext.Compositions.Add(composition);
//            _dbContext.SaveChanges();
//            return _dbContext.Compositions.Find(composition).Id;
//        }

        public Composition FindByName(string name)
        {
            var compositions = _dbContext.Compositions.Where(c=>c.Name.Equals(name));
            Composition result=null;
            if (compositions.Any())
            {
                result = compositions.First();
            }
            return result;
        }

        public void AddComposition(Composition composition)
        {
            if (!_dbContext.Compositions.Contains(composition))
            { 
                _dbContext.Compositions.Add(composition);
                _dbContext.SaveChanges();
            }
        }

        public Composition FindById(long id)
        {
            return _dbContext.Compositions.Find(id);
        }

        public void Update(Composition composition)
        {
            var compositionDb = _dbContext.Compositions.Find(composition.Id);
            compositionDb.Annotation = composition.Annotation;
            compositionDb.GenreId = composition.GenreId;
            compositionDb.Name = composition.Name;
            _dbContext.SaveChanges();
        }

        public IEnumerable<Composition> GetCompositionsOrderedByDate(int number,int page)
        {
            
            var compositions = _dbContext.Compositions.OrderByDescending(c=>c.LastPublication)
                .Skip(page*number).Take(number).ToList();

            return compositions;
        }

        public IEnumerable<Composition> FindByUserId(string userId)
        {
            return _dbContext.Compositions.Where(c => c.UserId.Equals(userId)).ToList();
        }

        public void DeleteByUserId(string userId)
        {
            var compositions = FindByUserId(userId);
            foreach (var composition in compositions)
            {
                Delete(composition);
            }
        }

        public void Delete(Composition composition)
        {
            var chapters = _dbContext.Chapters.Where(ch => ch.CompositionId == composition.Id).ToList();
            var compositionTags = _dbContext.CompositionTags.Where(ct => ct.CompositionId == composition.Id).ToList();
            var comments = _dbContext.Comments.Where(c => c.CompositionId == composition.Id).ToList();
            var likes = _dbContext.Likes.Where(l => l.CompositionId == composition.Id).ToList();
            var ratings = _dbContext.Ratings.Where(r => r.CompositionId == composition.Id).ToList();
            _dbContext.Compositions.Remove(composition);
            _dbContext.Chapters.RemoveRange(chapters);
            _dbContext.CompositionTags.RemoveRange(compositionTags);
            _dbContext.Comments.RemoveRange(comments);
            _dbContext.Ratings.RemoveRange(ratings);
            _dbContext.SaveChanges();
        }

        public void UpdateLastPublication(long compositionId)
        {
            var dbComposition = _dbContext.Compositions.Find(compositionId);
            dbComposition.LastPublication = DateTime.Now;
            _dbContext.SaveChanges();
        }


    }
}
