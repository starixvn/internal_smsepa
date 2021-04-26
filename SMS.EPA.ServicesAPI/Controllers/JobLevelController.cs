namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.JobLevel;
    using Model.API;
    using Services;

    [Authorize]
    public class JobLevelController : ApiController
    {
        private readonly IServiceAPI _service;

        public JobLevelController()
            : this(null)
        {
        }

        public JobLevelController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(JobLevel[] jobLevel)
        {
            var criteria = new UpdateJobLevelCriteria
            {
                JobLevels = jobLevel
            };

            CommandResult result = await _service.Update(criteria).ConfigureAwait(false);
            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForJobLevelCriteria();

            DateTime modifiedDate = await _service.GetLastModifiedDate(criteria).ConfigureAwait(false);

            return Ok(modifiedDate);
        }
    }
}