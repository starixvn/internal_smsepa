namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.JobTitle;
    using EPA.Model.API;
    using Services;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class JobTitleController : ApiController
    {
        private readonly IServiceAPI _service;

        public JobTitleController() : this(null) 
        {
        }

        public JobTitleController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult Update(JobTitle[] titles)
        {
            var criteria = new UpdateJobTitleCriteria
            {
                JobTitles = titles
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
