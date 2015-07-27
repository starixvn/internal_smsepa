// ----------------------------------------------------------------------
// <copyright file="CheckFlowAssignmentService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.CheckFlowAssignment;
    using SMS.EPA.Model.CheckFlowAssignment;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class CheckFlowAssignmentService : BaseService, ICheckFlowAssignmentService
    {
        private readonly ICheckFlowAssignmentRepository _checkFlowRepository;

        public CheckFlowAssignmentService()
            : this(null)
        {
        }

        public CheckFlowAssignmentService(ICheckFlowAssignmentRepository checkFlowRepository)
        {
            _checkFlowRepository = checkFlowRepository ?? new CheckFlowAssignmentRepository();
        }

        public IEnumerable<EmployeeFlowTargetSettingModel> GetEmployeeFlowTargetSetting(GetEmployeeFlowSettingCriteria criteria)
        {
            IEnumerable<EmployeeFlowTargetSettingModel> employeeFlowSettings = new List<EmployeeFlowTargetSettingModel>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                RawEmployeeFlowTargetSettingModel rawEmployeeFlowSetting = _checkFlowRepository.GetEmployeeFlowTargetSetting(criteria);
                employeeFlowSettings = GroupAndGetEmployeeTargetSetting(rawEmployeeFlowSetting);
            }

            return employeeFlowSettings;
        }

        public CommandResult UpdateEmployeeFlowTargetSetting(UpdateEmployeeFlowSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private static IEnumerable<EmployeeFlowTargetSettingModel> GroupAndGetEmployeeTargetSetting(
            RawEmployeeFlowTargetSettingModel rawEmployeeFlowSetting)
        {
            IEnumerable<EmployeeFlowTargetSettingModel> employeeFlowSettings = new List<EmployeeFlowTargetSettingModel>();

            if (rawEmployeeFlowSetting.Employees != null)
            {
                employeeFlowSettings = rawEmployeeFlowSetting.Employees;

                foreach (EmployeeFlowTargetSettingModel employeeSetting in employeeFlowSettings)
                {
                    GetTargetTemplate(rawEmployeeFlowSetting.TargetEmployees, employeeSetting);
                    GetFlowSetting(rawEmployeeFlowSetting.EmployeeFlows, employeeSetting);
                }
            }

            return employeeFlowSettings;
        }

        private static void GetTargetTemplate(
            IEnumerable<TargetEmployeeModel> targetEmployees,
            EmployeeFlowTargetSettingModel employeeSetting)
        {
            employeeSetting.TargetTemplate = new TargetTemplateModel();

            if (targetEmployees != null)
            {
                IEnumerable<TargetEmployeeModel> rawTargetEmployee = targetEmployees.Where(x => x.EmployeeId == employeeSetting.EmployeeId);

                if (rawTargetEmployee != null && rawTargetEmployee.Count() > 0)
                {
                    employeeSetting.TargetTemplate = rawTargetEmployee.First().TargetTemplate;
                }
            }
        }

        private static void GetFlowSetting(
            IEnumerable<RawEmployeeFlowModel> rawFlowSettings,
            EmployeeFlowTargetSettingModel employeeSetting)
        {
            IEnumerable<RawEmployeeFlowModel> rawEmployeeFlows = rawFlowSettings.Where(x => x.EmployeeId == employeeSetting.EmployeeId);

            foreach (RawEmployeeFlowModel rawEmployeeFlow in rawEmployeeFlows)
            {
                employeeSetting = GetEvaluators(rawEmployeeFlow, employeeSetting);
            }
        }

        private static EmployeeFlowTargetSettingModel GetEvaluators(
            RawEmployeeFlowModel rawEmployeeFlow,
            EmployeeFlowTargetSettingModel employeeSetting)
        {
            switch (rawEmployeeFlow.ProcessId)
            {
                case (int)ProcessType.Evaluator:
                    FlowModel evaluator = CreateEvaluator(rawEmployeeFlow);
                    employeeSetting.Evaluators.Add(evaluator);
                    break;
                case (int)ProcessType.Reviewer:
                    FlowModel reviewer = CreateEvaluator(rawEmployeeFlow);
                    employeeSetting.Reviewers.Add(reviewer);
                    break;
                case (int)ProcessType.Management:
                    FlowModel management = CreateEvaluator(rawEmployeeFlow);
                    employeeSetting.Managements.Add(management);
                    break;
                case (int)ProcessType.HumanResource:
                    FlowModel humanResource = CreateEvaluator(rawEmployeeFlow);
                    employeeSetting.HumanResources.Add(humanResource);
                    break;
            }

            return employeeSetting;
        }

        private static FlowModel CreateEvaluator(RawEmployeeFlowModel rawEmployeeFlow)
        {
            var evaluator = new FlowModel
            {
                ProcessId = rawEmployeeFlow.ProcessId,
                EvaluatorInfo = rawEmployeeFlow.EvaluatorInfo,
                StartDate = rawEmployeeFlow.StartDate,
                EndDate = rawEmployeeFlow.EndDate,
                HasSendMail = rawEmployeeFlow.HasSendMail
            };

            return evaluator;
        }
    }
}
