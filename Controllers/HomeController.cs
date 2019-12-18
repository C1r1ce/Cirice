using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cirice.Models;

namespace Cirice.Controllers
{
    public class HomeController : Controller
    {
        private DbGenreService _dbGenreService;
        private DbTagService _dbTagService;
        public HomeController(DbGenreService dbGenreService,DbTagService dbTagService)
        {
            _dbGenreService = dbGenreService;
            _dbTagService = dbTagService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
