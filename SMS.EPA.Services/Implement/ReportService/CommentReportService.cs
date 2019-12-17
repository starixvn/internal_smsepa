namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class CommentReportService : ICommentReportService
    {
        private readonly IBaseService baseService;

        public CommentReportService() : this(null)
        {
        }

        public CommentReportService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public IEnumerable<EPASummaryCommentModel> GetSummaryComment(int periodId)
        {
            var criteria = new GetEPACommentCriteria
            {
                PeriodId = periodId
            };

            return baseService.GetData<EPASummaryCommentModel>(criteria);
        }
    }
}