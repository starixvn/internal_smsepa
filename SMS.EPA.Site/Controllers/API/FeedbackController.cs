namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Feedback, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FeedbackReport, Policy = Policy.Read)]
        public HttpResponseMessage GetById(long feedbackId)
        {
            Feedback feedback = feedbackService.GetById(feedbackId);
            if (feedback.Evaluator.EmployeeId == SessionService.EmployeeId
                || SessionService.HasPermissionOnFunction(EPAFunctionKey.FeedbackReport, Policy.Read))
            {
                if (feedback.Evaluator.EmployeeId == SessionService.EmployeeId)
                {
                    var today = DateTime.Today;
                    feedback.IsHost = DateTime.Compare(feedback.FromDate.Date, today) >= 0
                        && DateTime.Compare(feedback.ToDate.Date, today) <= 0;
                }
                else
                {
                    feedback.IsViewDetail = true;
                }

                return Request.CreateResponse(HttpStatusCode.OK, feedback);
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, new Feedback());
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