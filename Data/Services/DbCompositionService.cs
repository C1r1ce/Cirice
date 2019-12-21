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

      
    }
}
