using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Cloud;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.Util;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;

namespace Cirice.Controllers
{
    public class CompositionController : Controller
    {
        private TagFormatter _tagFormatter = new TagFormatter();

        private UserManager<User> _userManager;
        private DbCompositionService _dbCompositionService;
        private DbGenreService _dbGenreService;
        private DbTagService _dbTagService;
        private DbCompositionTagService _dbCompositionTagService;
        private ICloudUploader _cloudUploader;
        private DbLikeService _dbLikeService;
        private DbRatingService _dbRatingService;


        public CompositionController(UserManager<User> userManager,
            DbCompositionService dbCompositionService,
            DbGenreService dbGenreService,
            DbTagService dbTagService,
            DbCompositionTagService dbCompositionTagService,
            ICloudUploader cloudUploader,
            DbLikeService dbLikeService,
            DbRatingService dbRatingService)
        {
            _userManager = userManager;
            _dbCompositionService = dbCompositionService;
            _dbGenreService = dbGenreService;
            _dbTagService = dbTagService;
            _dbCompositionTagService = dbCompositionTagService;
            _cloudUploader = cloudUploader;
            _dbLikeService = dbLikeService;
            _dbRatingService = dbRatingService;
        }
        // GET: Composition
        [Route("Composition/{id?}")]
        public ActionResult Index(long id)
        {
            Composition composition = _dbCompositionService.FindById(id);
            if (composition != null)
            {
                Genre genre = _dbGenreService.FindById(composition.GenreId);
                var tags = _dbTagService.FindByCompositionId(composition.Id);
                var likeCount = _dbLikeService.GetLikeCountByCompositionId(composition.Id);
                var rating = _dbRatingService.GetAverageRatingByCompositionId(composition.Id);
                var user = _userManager.FindByIdAsync(composition.UserId).Result;
                return View(new CompositionViewModel()
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
            return View();

        }


        // GET: Composition/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }


        // POST: Composition/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompositionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dbCompositionService.FindByName(model.Name) == null)
                {
                    Composition composition=new Composition()
                    {
                        Annotation = model.Annotation,
                        FirstPublication = DateTime.Now,
                        GenreId = _dbGenreService.FindByGenreString(model.GenreString).Id,
                        LastPublication = DateTime.Now,
                        Name=model.Name,
                        UserId = _userManager.GetUserAsync(HttpContext.User).Result.Id
                    };
                    _dbCompositionService.AddComposition(composition);
                    long compositionId = _dbCompositionService.FindByName(composition.Name).Id;
                    var listTagStrings = _tagFormatter.FormatTagsToList(model.Tags);
                    _dbTagService.AddTagsIfNotExist(listTagStrings);
                    var tags = _dbTagService.FindAllByTagStrings(listTagStrings);
                    _dbCompositionTagService.AddCompositionTags(compositionId,tags);
                    var callbackUri = Url.Action("Image", "Composition", new { id = compositionId }, protocol: HttpContext.Request.Scheme);
                    return Redirect(callbackUri);
                }
                else
                {
                    ModelState.AddModelError("", "A composition with this name already exists.");
                }
            }
            return View();
        }
        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var composition = _dbCompositionService.FindById(id);
            var genreString = _dbGenreService.FindById(composition.GenreId).GenreString;
            var tags = _dbTagService.FindByCompositionId(composition.Id);
            var tagsString = _tagFormatter.FormatTagsToString(tags);
            return View(new CompositionEditViewModel()
            {
                Annotation = composition.Annotation,
                GenreString = genreString,
                Name = composition.Name,
                Tags = tagsString,
                Id = id
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CompositionEditViewModel model)
        {
            IActionResult result = RedirectToAction("Index","Home");
            if (ModelState.IsValid)
            {
                
                if ( _dbCompositionService.FindById(model.Id)!=null)
                {
                    var nameBefore = _dbCompositionService.FindById(model.Id).Name;
                    if (nameBefore == model.Name || _dbCompositionService.FindByName(model.Name) == null)
                    {
                        Composition composition = new Composition()
                        {
                            Id = model.Id,
                            Annotation = model.Annotation,
                            FirstPublication = DateTime.Now,
                            GenreId = _dbGenreService.FindByGenreString(model.GenreString).Id,
                            LastPublication = DateTime.Now,
                            Name = model.Name,
                            UserId = _userManager.GetUserAsync(HttpContext.User).Result.Id
                        };
                        _dbCompositionService.Update(composition);
                        var listTagStrings = _tagFormatter.FormatTagsToList(model.Tags);
                        _dbTagService.AddTagsIfNotExist(listTagStrings);
                        var tags = _dbTagService.FindAllByTagStrings(listTagStrings);
                        _dbTagService.UpdateCompositionTags(model.Id, tags);
                        var callbackUri = Url.Action("Index", "Composition", new { id = model.Id }, protocol: HttpContext.Request.Scheme);
                        result = Redirect(callbackUri);
                    }
                    else
                    {
                        ModelState.AddModelError("", "A composition with this name already exists.");
                        result = View();
                    }
                }
                else
                {
                    result = BadRequest();
                }
            }
            return result;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Image(long id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            Composition composition = _dbCompositionService.FindById(id);
            if(composition!=null && user!=null && composition.UserId == user.Id)
            {
                return View(new ImageViewModel(){Id=composition.Id});
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Composition/Create
        public ActionResult Reader()
        {
            return View();
        }
        
       

        [HttpPost]
        public void Upload(IFormFile file,long id)
        {
            var composition = _dbCompositionService.FindById(id);
            if (composition != null)
            {
                var url = _cloudUploader.UploadImg(file);
                composition.ImgSource = url;
                _dbCompositionService.Update(composition);
            }

        }

        // POST: Composition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}