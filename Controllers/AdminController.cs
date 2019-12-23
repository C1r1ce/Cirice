using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cirice.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private UserManager<User> _userManager;
        private DbCompositionService _dbCompostionService;
        private DbLikeService _dbLikeService;
        private DbCommentService _dbCommentService;
        private DbRatingService _dbRatingService;

        public AdminController(UserManager<User> userManager,
            DbCompositionService dbCompositionService,
            DbLikeService dbLikeService,
            DbCommentService dbCommentService,
            DbRatingService dbRatingService

        )
        {
            _userManager = userManager;
            _dbCompostionService = dbCompositionService;
            _dbLikeService = dbLikeService;
            _dbCommentService = dbCommentService;
            _dbRatingService = dbRatingService;

        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            var model = new AdminViewModel()
            {
                Users = users
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(List<string> userCheckBox,string action)
        {
            if (userCheckBox.Any()) { 
                switch (action)
                {
                    case "block": await Block(userCheckBox);
                        break;
                    case "unlock": await Unlock(userCheckBox);
                        break;
                    case "delete": await Delete(userCheckBox);
                        break;
                    case "giveAdmin": await GiveAdmin(userCheckBox);
                        break;
                    case "removeAdmin": await RemoveAdmin(userCheckBox);
                        break;

                }
            }
            return RedirectToAction("Index");
        }

        private async Task Block(List<string> userCheckBox)
        {
            foreach (var id in userCheckBox)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await _userManager.SetLockoutEnabledAsync(user, true);
                    if (result.Succeeded)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddYears(10)));
                        
                    }
                }
            }
            
            
        }
        private async Task Unlock(List<string> userCheckBox)
        {
            foreach (var id in userCheckBox)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.SetLockoutEnabledAsync(user, false);

                }
            }
        }
        private async Task Delete(List<string> userCheckBox)
        {
            foreach (var id in userCheckBox)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    _dbCompostionService.DeleteByUserId(user.Id);
                    _dbRatingService.RemoveByUserId(user.Id);
                    _dbCommentService.RemoveByUserId(user.Id);
                    _dbLikeService.RemoveByUserId(user.Id);
                }
            }
        }
        private async Task GiveAdmin(List<string> userCheckBox)
        {
            foreach (var id in userCheckBox)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }
        }
        private async Task RemoveAdmin(List<string> userCheckBox)
        {
            foreach (var id in userCheckBox)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "admin");
                }
            }
        }

    }
}
