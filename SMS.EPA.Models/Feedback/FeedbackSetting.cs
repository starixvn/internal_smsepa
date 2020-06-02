namespace SMS.EPA.Model.Feedback
{
    using System;
    using System.Collections.Generic;

    public class FeedbackSetting
    {
        public int PeriodId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public FeedbackEmployee Employee { get; set; }
        public IEnumerable<FeedbackEmployee> Evaluators { get; set; } = new List<FeedbackEmployee>();
    }
}