namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Feedback;
    using SMS.EPA.Services;

    public class FeedbackReportController : BaseController
    {
        private readonly IFeedbackReportService feedbackReportService;

        public FeedbackReportController() : this(null)
        {
        }

        public FeedbackReportController(IFeedbackReportService feedbackService)
        {
            this.feedbackReportService = feedbackReportService ?? new FeedbackReportService();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Feedback, Policy = Policy.Read)]
        public IEnumerable<FeedbackReport> GetData(GetFeedbackReportCriteria criteria)
        {
            return feedbackReportService.GetData(criteria);
        }
    }
}