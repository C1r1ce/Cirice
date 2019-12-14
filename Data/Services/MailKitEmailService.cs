using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Cirice.Data.Services
{
    public class MailKitEmailService : IEmailSender
    {
        private readonly AuthMessageSenderOptions _authOptions;

        public MailKitEmailService(IOptions<AuthMessageSenderOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }


        public async Task SendEmailAsync(string userEmail, string userName, string emailSubject, string message)
        {
            var mailAddress = _authOptions.Mail;
            var mailPassword = _authOptions.MailPassword;
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Administration of Cirice", mailAddress));
            emailMessage.To.Add(new MailboxAddress(userName, userEmail));
            emailMessage.Subject = emailSubject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.AuthenticateAsync(mailAddress, mailPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
