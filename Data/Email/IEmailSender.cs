using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string userEmail, string userName, string emailSubject, string message);
    }
}
