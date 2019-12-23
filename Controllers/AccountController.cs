using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cirice.Data;
using Cirice.Data.Email;
using Cirice.Data.Models;
using Cirice.Data.Services;
using Cirice.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Cirice.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly IEmailSender _emailSender;
        private DbCompositionService _dbCompositionService;
        private DbTagService _dbTagService;
        private DbGenreService _dbGenreService;
        private DbRatingService _dbRatingService;
        private DbLikeService _dbLikeService;
        private DbCommentService _dbCommentService;


        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            DbCompositionService dbCompositionService,
            DbTagService dbTagService,
            DbGenreService dbGenreService,
            DbRatingService dbRatingService,
            DbLikeService dbLikeService,
            DbCommentService dbCommentService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dbCompositionService = dbCompositionService;
            _dbTagService = dbTagService;
            _dbGenreService = dbGenreService;
            _dbRatingService = dbRatingService;
            _dbLikeService = dbLikeService;
            _dbCommentService = dbCommentService;
        }

       
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User nUser = new User { Email = model.Email, UserName = model.Name,
                    FirstLogin = DateTime.Now, LastLogin = DateTime.Now};
                var result = await _userManager.CreateAsync(nUser, model.Password);
                if (result.Succeeded)
                {
                    User user = await _userManager.FindByEmailAsync(model.Email);
                    await _userManager.SetLockoutEnabledAsync(user, false);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUri = Url.Action("ConfirmEmail", "Account",new {UserId=user.Id,Code=code},protocol:HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(user.Email,user.UserName, "Confirm your email",
                        "<div>Hi dear "+ user.UserName+". Please click the link to confirm your email: <a href='" + callbackUri + "'>click here</a></div>");
                    return RedirectToAction("ConfirmEmailSend", "Notifications");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "The user with this mail does not exist");
                }
                else
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUri = Url.Action("ResetPassword", "Account", new { UserId = user.Id, Code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(user.Email, user.UserName, "Reset password",
                        "<div>Hi dear " + user.UserName + ". Please click the link to reset your password: <a href='" + callbackUri + "'>click here</a></div>");
                    return RedirectToAction("ResetPasswordSend","Notifications");
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string userId, string code)
        {
            IActionResult result;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                result = RedirectToAction("Login", "Account");
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    result = BadRequest();
                }
                else
                {
                    var reset = await _userManager.ResetPasswordAsync(user, code, model.Password);
                    if (reset.Succeeded)
                    {
                        result = RedirectToAction("ResetPasswordSuccess", "Notifications");
                    }
                    else
                    {
                        ModelState.AddModelError("",
                            "Something go wrong, we don`t change your password. Try again later.");
                        result = View();
                    }
                }
            }

            return result;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("Login","Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            if (!user.EmailConfirmed)
            {
                await _userManager.ConfirmEmailAsync(user, code);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }

            return RedirectToAction("ConfirmEmailSuccess","Notifications");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result;
                User user;

                if (model.Name.Contains('@'))
                {
                    user = await _userManager.FindByEmailAsync(model.Name);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(model.Name);
                }

                if (user != null)
                {
                    result =
                        await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        user.LastLogin = DateTime.Now;
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError("", "Please confirm your email");
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Your account is LockedOut");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wrond login and (or) password");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User does not exist");
                }
                
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Route("Account/{user?}")]
        public async Task<IActionResult> Index(string user)
        {
            var dbUser = await _userManager.FindByIdAsync(user);
            if (user!=null)
            {
                var compositions = _dbCompositionService.FindByUserId(user);
                List<CompositionViewModel> list = new List<CompositionViewModel>();
                foreach (Composition composition in compositions)
                {
                    Genre genre = _dbGenreService.FindById(composition.GenreId);
                    var tags = _dbTagService.FindByCompositionId(composition.Id);
                    var likeCount = _dbLikeService.GetLikeCountByCompositionId(composition.Id);
                    var rating = _dbRatingService.GetAverageRatingByCompositionId(composition.Id);
                    var commentCount = _dbCommentService.GetCommentCountByCompositionId(composition.Id);
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
                        UserName = dbUser.UserName,
                        CompositionId = composition.Id,
                        UserId = dbUser.Id,
                        Comments = commentCount
                    });
                }
                return View(new UserViewModel()
                {
                    CompositionViewModels = list,
                    UserAbout = dbUser.About,
                    UserId = dbUser.Id,
                    UserName = dbUser.UserName
                });
            }
            return BadRequest();
        }

    }

}