using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.Util;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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


        public CompositionController(UserManager<User> userManager,
            DbCompositionService dbCompositionService,
            DbGenreService dbGenreService,
            DbTagService dbTagService,
            DbCompositionTagService dbCompositionTagService)
        {
            _userManager = userManager;
            _dbCompositionService = dbCompositionService;
            _dbGenreService = dbGenreService;
            _dbTagService = dbTagService;
            _dbCompositionTagService = dbCompositionTagService;
        }
        // GET: Composition
        public ActionResult Index()
        {
            return View();
        }


        // GET: Composition/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Composition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompositionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dbCompositionService.GetByName(model.Name) == null)
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
                    long compositionId = _dbCompositionService.GetByName(composition.Name).Id;
                    var listTagStrings = _tagFormatter.FormatTags(model.Tags);
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

        [HttpGet]
        public IActionResult Image(long id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            Composition composition = _dbCompositionService.FindById(id);
            if(composition!=null && composition.UserId == user.Id)
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
        
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public void Upload(IFormFile file,long id)
        {


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