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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly UtilitiesController _utilitiesController;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService employeeEPAService,
            UtilitiesController utilitiesController)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
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

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        public EmployeeEPAInfoModel GetEPAInformation(
            int epaId,
            int periodId,
            int employeeId)
        {
            EmployeeEPAInfoModel epaInfo = new EmployeeEPAInfoModel();

            var getEPAInfoCriteria = new GetEPAInfoCriteria()
            {
                EPAId = epaId,
                PeriodId = periodId,
                EmployeeId = employeeId
            };

            epaInfo = _employeeEPAService.GetEPAInfo(getEPAInfoCriteria);
            epaInfo.Employee = _utilitiesController.GetEmployeeById(employeeId);

            return epaInfo;
        }

        [HttpPut]
        public CommandResult UpdateEmployeeEPA(EmployeeEPAInfoModel model)
        {
            CommandResult commondResult = new CommandResult();

            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                PeriodId = model.PeriodId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = model.Employee.EmployeeId == CurrentUser.EmployeeInfo.EmployeeId ? model.Employee.EmployeeId : model.Evaluator.EmployeeId,
                Comment = model.Comment,
                Score = _employeeEPAService.Encode(model.FinalScore),
                KPIDetails = _employeeEPAService.ConvertKPIInfoToXml(model.KPIGroups),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            commondResult = _employeeEPAService.UpdateEmployeeEPA(criteria);

            return commondResult;
        }
    }
}