namespace SMS.EPA.Site.Controllers.API
{
    using SMS.EPA.Core.Constants;

    public class JobDescriptionController : BaseController
    {
        public byte GetUserPermission()
        {
            return SessionService.GetPermission(EPAFunctionKey.JobDescription);
        }
    }
}