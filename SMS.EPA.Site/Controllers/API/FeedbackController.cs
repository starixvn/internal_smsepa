namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Feedback;
    using SMS.EPA.Services;

    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService feedbackService;

        public FeedbackController() : this(null)
        {
        }

        public FeedbackController(IFeedbackService feedbackService)
        {
            this.feedbackService = feedbackService ?? new FeedbackService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Feedback, Policy = Policy.Read)]
        public IEnumerable<Feedback> GetFeedback(int periodId)
        {
            return feedbackService.GetByActor(SessionService.EmployeeId, periodId);
        }

        [HttpGet]
        public Feedback GetById(long feedbackId)
        {
            return feedbackService.GetById(feedbackId);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Feedback, Policy = Policy.Host)]
        public CommandResult Update(Feedback feedback)
        {
            CommandResult commandResult = feedbackService.Update(feedback);

            return commandResult;
        }
    }
}