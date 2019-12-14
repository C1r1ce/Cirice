using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data;
using Cirice.Data.Email;
using Cirice.Data.Models;
using Cirice.Data.ViewModels;
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

        private IEmailSender _emailSender;
        

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUri = Url.Action("ConfirmEmail", "Account",new {UserId=user.Id,Code=code},protocol:HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(user.Email,user.UserName, "Cirice - Confirm your email",
                        "<div>Please click the link to confirm your email: <a href='" + callbackUri + "'>click here</a></div>");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
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
        public IActionResult ResetPassword()
        {
            return View();
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
                var result = await _userManager.ConfirmEmailAsync(user, code);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
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
                if (model.Name.Contains('@'))
                {
                    var user = await _userManager.FindByEmailAsync(model.Name);
                    result =
                        await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                }
                else
                {
                    result =
                        await _signInManager.PasswordSignInAsync(model.Name, model.Password, false, false);
                }
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if(result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please confirm your email");
                }
                else
                {
                    ModelState.AddModelError("", "Wrond login and (or) password");
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

        public async Task Delete2Users()
        {
            var user1 = await _userManager.FindByEmailAsync("evgenxr2000@gmail.com");
            await _userManager.DeleteAsync(user1);
            var user2 = await _userManager.FindByEmailAsync("c1r1ce.eugene@gmail.com");
            await _userManager.DeleteAsync(user2);
        }

    }

}