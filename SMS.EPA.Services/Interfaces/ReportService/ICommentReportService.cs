namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public interface ICommentReportService
    {
        IEnumerable<EPASummaryCommentModel> GetSummaryComment(int periodId);
    }
}
