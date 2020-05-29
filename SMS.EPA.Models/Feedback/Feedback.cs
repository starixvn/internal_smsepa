namespace SMS.EPA.Model.Feedback
{
    using System;
    using System.Collections.Generic;

    public class Feedback
    {
        public long FeedbackId { get; set; }
        public string Period { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsHost { get; set; }
        public bool IsViewDetail { get; set; }
        public DateTime? EvaluatedDate { get; set; }
        public string FinalScore { get; set; }
        public string Comment { get; set; }

        public FeedbackEmployee Employee { get; set; }
        public FeedbackEmployee Evaluator { get; set; }

        public IEnumerable<FeedbackKPIGroup> KPIGroups { get; set; } = new List<FeedbackKPIGroup>();
    }
}