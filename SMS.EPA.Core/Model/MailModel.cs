// ----------------------------------------------------------------------
// <copyright file="MailModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Core
{
    public class MailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string SenderEmail { get; set; }

        public string SenderName { get; set; }

        public string Title { get; set; }

        public string ReceiverEmail { get; set; }

        public string MailCc { get; set; }
    }
}
