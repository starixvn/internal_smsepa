// ----------------------------------------------------------------------
// <copyright file="IMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/27/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    public interface IMailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCc, string subject, string body);

        void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCc, string subject, string body);
    }
}
