using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cirice.Models;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Security;

namespace Cirice.Controllers
{
    public class HomeController : Controller
    {
        private DbGenreService _dbGenreService;
        private DbTagService _dbTagService;
        private DbCompositionService _dbCompositionService;
        private DbLikeService _dbLikeService;
        private DbRatingService _dbRatingService;
        private UserManager<User> _userManager;

        private const int NumberOfLoadedCompositions = 6;

        public HomeController(
            DbCompositionService dbCompositionService,
            DbGenreService dbGenreService,
            DbTagService dbTagService,
            DbLikeService dbLikeService,
            DbRatingService dbRatingService,
            UserManager<User> userManager)
        {
            _dbCompositionService = dbCompositionService;
            _dbGenreService = dbGenreService;
            _dbTagService = dbTagService;
            _dbLikeService = dbLikeService;
            _dbRatingService = dbRatingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            int page = id ?? 0;
            var list = GetItems(page);
            if (page == 0)
            {
                return View(new ListCompositionViewModel()
                {
                    CompositionViewModels = list
                });
            }
            else
            {
                return PartialView("_Item", new ListCompositionViewModel()
                {
                    CompositionViewModels = list
                });
            }
        }

        private List<CompositionViewModel> GetItems(int page)
        {
            var compositions = _dbCompositionService.GetCompositionsOrderedByDate(NumberOfLoadedCompositions, page);
            List<CompositionViewModel> list = new List<CompositionViewModel>();
            foreach (Composition composition in compositions)
            {
                Genre genre = _dbGenreService.FindById(composition.GenreId);
                var tags = _dbTagService.FindByCompositionId(composition.Id);
                var likeCount = _dbLikeService.GetLikeCountByCompositionId(composition.Id);
                var rating = _dbRatingService.GetAverageRatingByCompositionId(composition.Id);
                var user = _userManager.FindByIdAsync(composition.UserId).Result;
                list.Add(new CompositionViewModel()
                {
                    Annotation = composition.Annotation,
                    GenreString = genre.GenreString,
                    ImgSource = composition.ImgSource,
                    LastPublication = composition.LastPublication,
                    Likes = likeCount,
                    Name = composition.Name,
                    Rating = rating,
                    Tags = tags,
                    UserName = user.UserName,
                    CompositionId = composition.Id,
                    UserId = user.Id
                });
            }

            return list;
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
