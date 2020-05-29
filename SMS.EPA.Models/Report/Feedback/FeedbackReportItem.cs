namespace SMS.EPA.Model.Report.Feedback
{
    using System;
    using Newtonsoft.Json;
    using SMS.EPA.Model.Feedback;

    public class FeedbackReportItem
    {
        public long FeedbackId { get; set; }
        public string FinalScore { get; set; }
        public string Comment { get; set; }

        [JsonIgnore]
        public DateTime? EvaluatedDate { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }

        public FeedbackEmployee Evaluator { get; set; }
    }
}