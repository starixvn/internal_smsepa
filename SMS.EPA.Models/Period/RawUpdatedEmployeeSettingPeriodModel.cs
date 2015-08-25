// ----------------------------------------------------------------------
// <copyright file="RawUpdatedEmployeeSettingPeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Model.FlowManagement;

    public class RawUpdatedEmployeeSettingPeriodModel
    {
        private const string _AssignmentItem = "<Assignment ProcessId='{0}' EvaluatorId='{1}' Duration='{2}' HasSendMail='{3}' RemindDay='{4}' StartDate='{5}' EndDate='{6}'></Assignment>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public int PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public int? TargetTemplateId { get; set; }

        public IEnumerable<PeriodFlowModel> EmployeeAssignments { get; set; }

        public string ConvertToXml()
        {
            var value = new StringBuilder(_BeginRootTag);

            if (EmployeeAssignments != null)
            {
                foreach (PeriodFlowModel assignment in EmployeeAssignments)
                {
                    value.AppendFormat(_AssignmentItem, assignment.ProcessId, assignment.EvaluatorInfo.EmployeeId, assignment.Duration, assignment.HasSendMail, assignment.RemindDay, assignment.StartDate, assignment.EndDate);
                }
            }

            value.Append(_EndRootTag);
            return value.ToString();
        }
    }
}
