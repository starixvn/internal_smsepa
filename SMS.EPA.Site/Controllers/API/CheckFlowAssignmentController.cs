// ----------------------------------------------------------------------
// <copyright file="CheckFlowAssignmentController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.CheckFlowAssignment;
    using SMS.EPA.Model.CheckFlowAssignment;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class CheckFlowAssignmentController : BaseController
    {
        private readonly ICheckFlowAssignmentService _checkFlowService;

        public CheckFlowAssignmentController()
            : this(null)
        {
        }

        public CheckFlowAssignmentController(ICheckFlowAssignmentService checkFlowService)
        {
            _checkFlowService = checkFlowService ?? new CheckFlowAssignmentService();
        }

        public IEnumerable<EmployeeFlowTargetSettingModel> GetEmployeeFlowTargetSetting(
            int? groupId = null,
            int? employeeId = null,
            int? targetTypeId = null)
        {
            var criteria = new GetEmployeeFlowSettingCriteria
            {
                GroupId = groupId,
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId
            };

            IEnumerable<EmployeeFlowTargetSettingModel> employeeFlowSettings = _checkFlowService.GetEmployeeFlowTargetSetting(criteria);
            return employeeFlowSettings;
        }

        [HttpPut]
        public CommandResult UpdateEmployeeFlowTargetSetting(IList<RawEmployeeFlowModel> employeeFlowSettings)
        {
            var criteria = new UpdateEmployeeFlowSettingCriteria
            {
                EmployeeAssignments = TransformSettingToXml(employeeFlowSettings),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _checkFlowService.UpdateEmployeeFlowTargetSetting(criteria);
            return commandResult;
        }

        private string TransformSettingToXml(IList<RawEmployeeFlowModel> employeeFlowSettings)
        {
            string employeeAssignment = "<Assignment EmployeeId='{0}' TargetTemplateId='{1}' ProcessId='{2}' EvaluatorId='{3}' StartDate='{4}' EndDate='{5}' HasSendMail='{6}'></Assignment>";
            string beginRootTag = "<ROOT>";
            string endRootTag = "</ROOT>";

            var value = new StringBuilder(beginRootTag);

            foreach (RawEmployeeFlowModel setting in employeeFlowSettings)
            {
                value.AppendFormat(
                    employeeAssignment,
                    setting.EmployeeId,
                    setting.TargetTemplateId,
                    setting.ProcessId,
                    setting.EvaluatorInfo.EmployeeId,
                    setting.HasSendMail);
            }

            value.Append(endRootTag);

            return value.ToString();
        }
    }
}