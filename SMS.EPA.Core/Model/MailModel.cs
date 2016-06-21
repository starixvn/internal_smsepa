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