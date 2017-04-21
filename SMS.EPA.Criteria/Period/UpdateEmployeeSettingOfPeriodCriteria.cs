namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Text;

    public class UpdateEmployeeSettingOfPeriodCriteria : CriteriaBase
    {
        private const string AssignmentXmlTemplate = @"<Assignment EmployeeId='{0}' ProcessId='{1}' EvaluatorId='{2}'
                StartDate='{3}' EndDate='{4}' Priority='{5}'></Assignment>";

        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeIds { get; set; }

        public int? TargetTemplateId { get; set; }

        [SpParam(SqlParamName = "EmployeeAssignments")]
        public string EmployeeAssignmentXml
        {
            get
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

        [SpParam(Ignored = true)]
        public IEnumerable<PeriodFlowModel> EmployeeAssignments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_EditEmployeeSetting";
        }
    }
}