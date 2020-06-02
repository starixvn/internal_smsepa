namespace SMS.EPA.Model.Feedback
{
    using System;
    using Newtonsoft.Json;
    using SMS.EPA.Core.Services;

    public class FeedbackEmployee
    {
        [JsonIgnore]
        public long FeedbackId { get; set; }

        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }
        public string JobTitle { get; set; }
        public DateTime JoinedDate { get; set; }

        public string Avatar
        {
            get
            {
                IUtilityService utilityService = new UtilityService();
                return utilityService.GetAvatar(Code);
            }
        }
    }
}