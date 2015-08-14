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
    using System.Web.Http;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _memberEPAService;
        private readonly UtilitiesController _utilitiesController;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService memberEPAService,
            UtilitiesController utilitiesController)
        {
            _memberEPAService = memberEPAService ?? new EmployeeEPAService();
            _utilitiesController = utilitiesController ?? new UtilitiesController();
        }

        [HttpGet]
        public IEnumerable<EmployeeEPAModel> GetEPAOfCurrentMember(int? status = null)
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                Status = status
            };

            return _memberEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        public EmployeeEPAInfoModel GetEPAInformation(int employeeId)
        {
            EmployeeEPAInfoModel epaInfo = new EmployeeEPAInfoModel();

            var getEPAInfoCriteria = new GetEPAInfoCriteria()
            {
                EmployeeId = employeeId
            };

            epaInfo = _memberEPAService.GetEPAInfo(getEPAInfoCriteria);
            epaInfo.Employee = _utilitiesController.GetEmployeeById(employeeId);

            return epaInfo;
        }
    }
}