namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services;

    public class DepartmentController : BaseController
    {
        private readonly IUnitService unitService;

        public DepartmentController() : this(null)
        {
        }

        public DepartmentController(IUnitService unitService)
        {
            this.unitService = unitService ?? new UnitService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Department, Policy = Policy.Read)]
        public IList<MultiSelectItem> GetDepartment()
        {
            return unitService.GetOrgChart();
        }
    }
}