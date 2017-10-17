namespace SMS.EPA.ServicesAPI.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.JobTitle;
    using Model.API;
    using Services;

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

        public async Task<IHttpActionResult> Update(JobTitle[] titles)
        {
            var criteria = new UpdateJobTitleCriteria
            {
                JobTitles = titles
            };
            CommandResult result = await _service.Update(criteria).ConfigureAwait(false);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}