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
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
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
        public IEnumerable<Employee> GetMember(
            int processId,
            int? groupId = null)
        {
            var criteria = new GetMemberCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = processId,
                GroupId = groupId
            };

            return _employeeEPAService.GetMember(criteria);
        }

        [HttpGet]
        public IEnumerable<EmployeeEPAModel> GetEPAOfCurrentMember(
            int? targetTypeId = null,
            int? status = null)
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
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

        [HttpGet]
        public IEnumerable<MemberEPAModel> GetEPAByEvaluator(
            DateTime fromDate,
            DateTime toDate,
            int? targetTypeId = null,
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                FromDate = fromDate,
                ToDate = toDate,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpPut]
        public CommandResult UpdateEmployeeEPA(EmployeeEPAInfoModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                PeriodId = model.PeriodId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.Comment,
                Score = _employeeEPAService.Encode(model.FinalScore),
                KPIDetails = _employeeEPAService.ConvertKPIInfoToXml(model.KPIGroups),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEmployeeEPA(criteria);
        }

        [HttpPut]
        public CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _employeeEPAService.UpdateStatusEPA(criteria);
        }
    }
}