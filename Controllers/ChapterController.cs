using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cirice.Controllers
{
    public class ChapterController : Controller
    {
        private RightService _rightService;
        private DbCompositionService _compositionService;
        private UserManager<User> _userManager;
        private DbChapterService _chapterService;

        public ChapterController(
            RightService rightService,
            DbCompositionService compositionService,
            UserManager<User> userManager,
            DbChapterService chapterService)
        {
            _rightService = rightService;
            _compositionService = compositionService;
            _userManager = userManager;
            _chapterService = chapterService;
        }

        [Route("Chapter/{id?}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Chapter/Main/{id?}")]
        public async Task<IActionResult> Main(long id)
        {
            if (await _rightService.CheckRights(id, await _userManager.GetUserAsync(HttpContext.User)))
            {
                var chapters = _chapterService.FindByCompositionId(id);
                var composition = _compositionService.FindById(id);
                List<ChapterNumberAndId> chapterNumberAndIds = new List<ChapterNumberAndId>();
                foreach (var chapter in chapters)
                {
                    chapterNumberAndIds.Add(new ChapterNumberAndId()
                    {
                        ChapterId = chapter.Id,
                        Number = chapter.Number
                    });
                }
               
                var model = new ChapterMainViewModel()
                {
                    CompositionId = id,
                    ChapterNumbersAndIds = chapterNumberAndIds,
                    CompositionName = composition.Name
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Notifications");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Main(ChapterMainViewModel model)
        {
            if (await _rightService.CheckRights(model.CompositionId, await _userManager.GetUserAsync(HttpContext.User)))
            {
                if (model.ChapterIdFirst != model.ChapterIdSecond)
                {
                    await _chapterService.SwapAsync(model.ChapterIdFirst,model.ChapterIdSecond);
                    return RedirectToAction("Main","Chapter");
                }
                else
                {
                    ModelState.AddModelError("","Select different chapters");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Notifications");
            }
        }
    }
}
