namespace SMS.EPA.Core.Services.Implement
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using SMS.EPA.Core.Services.Interfaces;

    public class BaseMailService : IBaseMailService
    {
        public void SendEmail(MailModel email)
        {
            if (email == null
                || !IsValidEmail(email.ReceiverEmail)
                || ConfigurationManager.AppSettings["NotSendMail"].Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = email.ReceiverEmail,
                MailCc = email.MailCc,
                Subject = email.Subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = email.Body
            };
            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        public string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}