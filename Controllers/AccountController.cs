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

        private readonly IEmailSender _emailSender;
        

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