// ----------------------------------------------------------------------
// <copyright file="MailModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Enitites
{
    public class MailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string SendTo { get; set; }

        public string SendCc { get; set; }

        public string Sender { get; set; }

        public string SendToNickName { get; set; }

        public bool IsAllowSendRequester { get; set; }
    }
}
