using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cirice.Controllers
{
    public class ReaderController : Controller
    {
        private DbChapterService _dbChapterService;
        private DbCompositionService _dbCompositionService;
        private DbLikeService _dbLikeService;
        private UserManager<User> _userManager;
        private DbRatingService _dbRatingService;

        public ReaderController(
            DbChapterService dbChapterService,
            DbCompositionService dbCompositionService,
            DbLikeService dbLikeService,
            UserManager<User> userManager,
            DbRatingService dbRatingService)
        {
            _dbChapterService = dbChapterService;
            _dbCompositionService = dbCompositionService;
            _dbLikeService = dbLikeService;
            _userManager = userManager;
            _dbRatingService = dbRatingService;
        }

        [Route("Reader/{compositionId?}/{chapterId?}")]
        public async Task<IActionResult> Index(long compositionId,long ?chapterId)
        {
            IActionResult result = BadRequest();
            
            var composition = _dbCompositionService.FindById(compositionId);
            if (composition != null)
            {
                long chapterIdCurrent=chapterId ?? 0;
                Chapter chapter=null;
                if (chapterIdCurrent > 0)
                {
                    chapter = await _dbChapterService.FindByIdAsync(chapterIdCurrent);
                }
                else
                {
                    var firstChapter = _dbChapterService.FindByCompositionIdAndNumber(compositionId, 1);
                    if (firstChapter != null)
                    {
                        chapter = firstChapter;
                    }
                }
                if (chapter != null)
                {
                    List<ChapterNumberAndId> chapterNumberAndIds = new List<ChapterNumberAndId>();
                    var chapters = _dbChapterService.FindByCompositionId(compositionId);
                    foreach (var ch in chapters)
                    {
                        chapterNumberAndIds.Add(new ChapterNumberAndId()
                        {
                            ChapterId = ch.Id,
                            Number = ch.Number
                        });
                    }

                    byte rating = 1;
                    if (_dbChapterService.LastChapter(chapter))
                    {
                        var user = await _userManager.GetUserAsync(HttpContext.User);
                        if (user != null)
                        {
                            var rate = _dbRatingService.FindByUserIdAndCompositionId(user.Id, composition.Id);
                            if (rate != null)
                            {
                                rating = rate.Mark;
                            }
                        }
                        
                    }
                    var likes = _dbLikeService.GetLikesByChapterId(chapter.Id);
                    var likesUserIds = new List<string>();
                    foreach (var like in likes)
                    {
                        likesUserIds.Add(like.UserId);
                    }
                    result = View(new ReaderChapterViewModel()
                    {
                        ChapterId = chapterIdCurrent,
                        ChapterNumbersAndIds = chapterNumberAndIds,
                        CompositionId = compositionId,
                        CompositionName = composition.Name,
                        Text = chapter.Text,
                        ChapterNumber = chapter.Number,
                        LikeUserIds = likesUserIds,
                        Rating = rating
                    });
                }
                else
                {
                    result = RedirectToAction("EmptyComposition", "Notifications");
                }
            }

            return result;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Like(ReaderChapterViewModel model)
        {
            IActionResult result;
            var chapter = await _dbChapterService.FindByIdAsync(model.ChapterId);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (chapter != null && user != null)
            {
                var url = Url.Action("Index", "Reader",
                    new {compositionId = model.CompositionId, chapterId = model.ChapterId});
                result = Redirect(url);
                var dbLike = _dbLikeService.FindByUserIdAndChapterId(user.Id, model.ChapterId);
                if (dbLike == null)
                {
                    _dbLikeService.Add(new Like()
                    {
                        ChapterId = model.ChapterId,
                        CompositionId = model.CompositionId,
                        DateTime = DateTime.Now,
                        UserId = user.Id
                    });

                }
                else
                {
                    _dbLikeService.Remove(dbLike);
                }
            }
            else
            {
                 result = BadRequest();
            }

            return result;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(ReaderChapterViewModel model)
        {
            var composition = _dbCompositionService.FindById(model.CompositionId);
            if (composition != null && model.Rating>0)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (_dbRatingService.FindByUserIdAndCompositionId(user.Id, composition.Id)==null)
                {
                    var rating = new Rating()
                    {
                        UserId = user.Id,
                        CompositionId = model.CompositionId,
                        Mark = model.Rating
                    };
                    _dbRatingService.Add(rating);
                }
                else
                {
                    _dbRatingService.UpdateMark(user.Id, composition.Id, model.Rating);
                }
                var url = Url.Action("Index", "Reader",
                    new { compositionId = model.CompositionId, chapterId = model.ChapterId });
                return Redirect(url);
            }

            return BadRequest();
        }
    }
}
