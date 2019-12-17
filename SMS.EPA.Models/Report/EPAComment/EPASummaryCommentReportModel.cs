using System.Collections.Generic;

namespace SMS.EPA.Model
{
    public class EPASummaryCommentReportModel
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public IEnumerable<EPASummaryCommentModel> CommentSummary { get; set; }
    }
}