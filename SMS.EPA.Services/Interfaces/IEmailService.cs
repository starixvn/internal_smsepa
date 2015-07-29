// ----------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/29/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body);

        void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCC, string subject, string body);
    }
}
