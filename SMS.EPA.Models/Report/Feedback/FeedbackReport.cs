namespace SMS.EPA.Model.Report.Feedback
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using SMS.EPA.Core.Constants;

    public class FeedbackReport
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }

        public string AverageFinalScore
        {
            get
            {
                float average = 0;
                int count = 0;
                var result = string.Empty;
                var feedbacks = Feedbacks.Where(x => !string.IsNullOrWhiteSpace(x.FinalScore) && x.EvaluatedDate.HasValue);
                foreach (FeedbackReportItem feedback in feedbacks)
                {
                    count++;
                    float.TryParse(feedback.FinalScore, out float score);
                    average += score;
                }

                if (count > 0)
                {
                    result = Math.Round(average / count, Constants.NumberOfDecimalDefault).ToString(CultureInfo.InvariantCulture);
                }

                return result;
            }
        }

        public int TotalComment => Feedbacks.Count(x => !string.IsNullOrWhiteSpace(x.Comment));
        public int TotalEvaluate => Feedbacks.Count(x => !string.IsNullOrWhiteSpace(x.FinalScore) && x.EvaluatedDate.HasValue);

        public IEnumerable<FeedbackReportItem> Feedbacks { get; set; } = new List<FeedbackReportItem>();
    }
}