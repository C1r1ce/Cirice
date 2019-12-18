using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cirice.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult ResetPasswordSend()
        {
            return View();
        }

        public IActionResult ResetPasswordSuccess()
        {
            return View();
        }

        public IActionResult ConfirmEmailSend()
        {
            return View();
        }

        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }
    }
}
