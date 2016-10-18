namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Core.Constants;

    public class UpdatedEmployeeSettingPeriodModel
    {
        private const string AssignmentXmlTemplate = @"<Assignment EmployeeId='{0}' ProcessId='{1}' EvaluatorId='{2}'
                StartDate='{3}' EndDate='{4}' Priority='{5}'></Assignment>";

        public int PeriodId { get; set; }

        public int? TargetTemplateId { get; set; }

        public string EmployeeIds { get; set; }

        public IEnumerable<PeriodFlowModel> EmployeeAssignments { get; set; }

        public string ConvertToXml()
        {
            var assignmentXml = new StringBuilder(Constants.BeginRootTag);

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

            return assignmentXml.Append(Constants.EndRootTag).ToString();
        }
    }
}