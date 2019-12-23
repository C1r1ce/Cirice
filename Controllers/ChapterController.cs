using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        [Route("Chapter/{compositionId?}/{chapterId?}")]
        public async Task<IActionResult> Index(long compositionId,long chapterId)
        {
            IActionResult result = BadRequest();
            if (await _rightService.CheckRights(compositionId, await _userManager.GetUserAsync(HttpContext.User)))
            {
                var chapter = await _chapterService.FindByIdAsync(chapterId);
                if (chapter != null)
                {
                    ChapterViewModel viewModel=new ChapterViewModel()
                    {
                        ChapterId = chapter.Id,
                        CompositionId = chapter.CompositionId,
                        Number = chapter.Number,
                        Text = chapter.Text
                    };
                    result = View(viewModel);
                }
            }
            else
            {
                result = RedirectToAction("AccessDenied", "Notifications");
            }

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Chapter/{compositionId?}/{chapterId?}")]
        public async Task<IActionResult> Index(ChapterViewModel model)
        {
            IActionResult result = RedirectToAction("AccessDenied", "Notifications");
            if (await _rightService.CheckRights(model.CompositionId, await _userManager.GetUserAsync(HttpContext.User)))
            {
                var chapter = await _chapterService.FindByIdAsync(model.ChapterId);
                if (chapter != null)
                {
                    chapter.Text = model.Text;
                    _chapterService.UpdateChapter(chapter);
                    var url = Url.Action("Main", "Chapter", new {id = model.CompositionId});
                    result = Redirect(url);
                }
                else
                {
                    result = BadRequest();
                }
                
            }

            return result;
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
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ChapterMainViewModel model)
        {
            if (await _rightService.CheckRights(model.CompositionId, await _userManager.GetUserAsync(HttpContext.User)))
            {
                var chapters = _chapterService.FindByCompositionId(model.CompositionId);
                int lastNumber = 0;
                foreach (var chapter in chapters)
                {
                    if (chapter.Number > lastNumber)
                    {
                        lastNumber = chapter.Number;
                    }
                }

                lastNumber++;
                Chapter newChapter = new Chapter()
                {
                    CompositionId = model.CompositionId,
                    Number = lastNumber,
                    Text = "Put your text here"
                };
                _chapterService.Add(newChapter);
                _compositionService.UpdateLastPublication(model.CompositionId);
                var dbChapter = _chapterService.FindByCompositionIdAndNumber(model.CompositionId, lastNumber);
                var url = Url.Action("Index", "Chapter",
                    new {compositionId = model.CompositionId, chapterId = dbChapter.Id});
                return Redirect(url);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Notifications");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(ChapterMainViewModel model)
        {
            await _chapterService.RemoveById(model.ChapterIdToDelete);
            var url = Url.Action("Main", "Chapter", new { id=model.CompositionId });
            return Redirect(url);
        }
    }
}
