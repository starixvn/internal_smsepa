// -----------------------------------------------------------------------
// <copyright file="DepartmentController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.01">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    public class DepartmentController : BaseController
    {
        private readonly UtilitiesController _utilitiesController;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(UtilitiesController utilitiesController)
        {
            _utilitiesController = utilitiesController ?? new UtilitiesController();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Department, Policy = Policy.Read)]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            return _utilitiesController.GetDepartment(parentId, isActive);
        }
    }
}