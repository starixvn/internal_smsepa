namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.JobTitle;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class JobTitleController : ApiController
    {
        private readonly IServiceAPI _jobTitleService;

        public JobTitleController() : this(null) 
        {
        }

        public JobTitleController(IServiceAPI jobTitleService)
        {
            _jobTitleService = jobTitleService ?? new ServiceAPI();
        }

        public IHttpActionResult Update(JobTitle[] titles)
        {
            var criteria = new UpdateJobTitleCriteria()
            {
                JobTitles = titles
            };

            CommandResult result = _jobTitleService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
