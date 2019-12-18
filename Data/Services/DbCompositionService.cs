using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

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

        public Composition GetByName(string name)
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
            var composition = _dbContext.Compositions.Where(c=>c.Id==id);
            Composition result = null;
            if (composition.Any())
            {
                result = composition.First();
            }

            return result;
        }
    }
}
