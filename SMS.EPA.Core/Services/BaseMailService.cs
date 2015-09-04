// ----------------------------------------------------------------------
// <copyright file="BaseMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/27/2015">Created.</change>
// </history>

namespace SMS.EPA.Core.Services.Implement
{
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;
    using Fanex.MailDeliver.Model;
    using SMS.EPA.Core.Services.Interfaces;

    public class BaseMailService : IBaseMailService
    {
        public void SendEmail(MailModel email)
        {
            if (!IsValidEmail(email.ReceiverEmail))
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
            using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
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