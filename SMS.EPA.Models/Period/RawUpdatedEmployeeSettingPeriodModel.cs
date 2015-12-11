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

    public class RawUpdatedEmployeeSettingPeriodModel
    {
        private const string AssignmentXmlTemplate = "<Assignment EmployeeId='{0}' ProcessId='{1}' EvaluatorId='{2}'" + 
                                        " StartDate='{3}' EndDate='{4}' Priority='{5}'></Assignment>";

        private const string BeginRootTag = "<ROOT>";

        private const string EndRootTag = "</ROOT>";

        public int PeriodId { get; set; }

        public int? TargetTemplateId { get; set; }

        public string EmployeeIds { get; set; }

        public IEnumerable<PeriodFlowModel> EmployeeAssignments { get; set; }

        public string ConvertToXml()
        {
            var assignmentXml = new StringBuilder(BeginRootTag);

            if (EmployeeAssignments != null)
            {
                foreach (PeriodFlowModel assignment in EmployeeAssignments)
                {
                    assignmentXml.AppendFormat(
                        AssignmentXmlTemplate,
                        assignment.EmployeeId,
                        assignment.ProcessId,
                        assignment.EvaluatorInfo.EmployeeId,
                        assignment.StartDate,
                        assignment.EndDate,
                        assignment.Priority);
                }
            }

            assignmentXml.Append(EndRootTag);
            return assignmentXml.ToString();
        }
    }
}
