namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.JobTitle;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System.Collections.Generic;
    using System.Web.Http;

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

        public IHttpActionResult Update([FromBody]JobTitle[] titles)
        {
            var criteria = new UpdateJobTitleCriteria()
            {
                JobTitles = titles
            };

            CommandResult result = _jobTitleService.Update(criteria);

            return Ok(result);
        }
    }
}
