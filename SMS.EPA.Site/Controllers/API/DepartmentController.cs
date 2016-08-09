namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DepartmentController : BaseController
    {
        private readonly ICommonService commonService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(ICommonService commonService)
        {
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Department, Policy = Policy.Read)]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            var criteria = new GetDepartmentCriteria()
            {
                ParentId = parentId,
                IsActive = isActive
            };

            return commonService.GetDepartment(criteria);
        }
    }
}