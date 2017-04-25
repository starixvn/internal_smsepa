namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Web.Http;

    public class DepartmentController : BaseController
    {
        private readonly ICommonService _commonService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(ICommonService commonService)
        {
            _commonService = commonService ?? new CommonService();
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

            return _commonService.GetDepartment(criteria);
        }
    }
}