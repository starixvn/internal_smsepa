// -----------------------------------------------------------------------
// <copyright file="EmployeeEPAController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.10">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _memberEPAService;

        public EmployeeEPAController()
            : this(null)
        {
        }

        public EmployeeEPAController(IEmployeeEPAService memberEPAService)
        {
            _memberEPAService = memberEPAService ?? new EmployeeEPAService();
        }

        public IEnumerable<EmployeeEPAModel> GetEPAOfCurrentMember(int? status = null)
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                Status = status
            };

            return _memberEPAService.GetEPAByEmployee(criteria);
        }
    }
}