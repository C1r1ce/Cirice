﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbCompositionTagService
    {
        private AppDbContext _dbContext;

        public DbCompositionTagService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
    }
}
