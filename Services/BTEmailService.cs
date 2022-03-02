using BugTracker.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTEmailService : IEmailSender
    {
        #region Variables
        private readonly MailSettings _mailSettings;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        #endregion

        #region SendEmailAsync
        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            MimeMessage email = new(); // Instantiates email of type MimeMessage
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail); // Parses _mailSettings.Mail into a mail box address
            email.To.Add(MailboxAddress.Parse(emailTo)); // Adds an address to the emailTo string that is parsed into a mail box address
            email.Subject = subject; // Email subject = subject

            var builder = new BodyBuilder // Creates new instance of BodyBuilder class
            {
                HtmlBody = htmlMessage // Assigns string htmlMessage to HtmlBody
            };

            email.Body = builder.ToMessageBody(); // Turns message into a message body

            try
            {
                using var smtp = new SmtpClient(); // Initializes new instance of SmtpClient class
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls); // Connects to smtp using values in _mailSettings
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password); // Authenticates smtp connection
                await smtp.SendAsync(email); // Sends email through smtp connection
                smtp.Disconnect(true); // Disconnects smtp connection

            }

            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
