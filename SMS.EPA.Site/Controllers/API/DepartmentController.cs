// -----------------------------------------------------------------------
// <copyright file="DepartmentController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.28">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? new DepartmentService();
        }

        [HttpPut]
        public CommandResult UpdateDepartment(UpdateDepartmentCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _departmentService.UpdateDepartment(criteria);

            return commandResult;
        }
    }
}