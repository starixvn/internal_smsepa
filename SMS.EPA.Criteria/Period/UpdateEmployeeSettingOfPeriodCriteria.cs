namespace SMS.EPA.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model;

    public class UpdateEmployeeSettingOfPeriodCriteria : CriteriaBase
    {
        private const string AssignmentXmlTemplate = @"<Assignment EmployeeId='{0}' ProcessId='{1}' EvaluatorId='{2}'
                StartDate='{3}' EndDate='{4}' Priority='{5}'></Assignment>";

        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeIds { get; set; }

        public int? TargetTemplateId { get; set; }
        public DateTime? DisciplineStartDate { get; set; }
        public DateTime? DisciplineEndDate { get; set; }

        [SpParam(SqlParamName = "EmployeeAssignments")]
        public string EmployeeAssignmentXml
        {
            get
            {
                var assignmentXml = new StringBuilder();
                if (EmployeeAssignments != null && EmployeeAssignments.Any())
                {
                    assignmentXml.Append(Constants.BeginRootTag);
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

                    assignmentXml.Append(Constants.EndRootTag);
                }

                return assignmentXml.ToString();
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<PeriodFlowModel> EmployeeAssignments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0 && EmployeeAssignments != null && EmployeeAssignments.Any();

        public override string GetSettingKey() => "EpaPeriod_EditEmployeeSetting";
    }
}