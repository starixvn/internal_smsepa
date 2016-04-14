namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class UpdateAttendanceViolationCriteria : CriteriaBase
    {
        private const string _VialationTpl =
            "<Violation EmpId='{0}' WorkDate='{1}' ShiftName='{2} 'CheckIn='{3}' 'CheckOut='{4}' TotalHours='{5}'"
            + " LateIn='{6}' EarlyOut='{7}' Type='{8}' DisciplineScore='{9}'"
            + " EmployeeComment='{10}' ApproverComment='{11}' IsIgnored='{12}' ModifiedDate='{13}' IsDeleted='{14}'></Violation>";

        [SpParam(Ignored = true)]
        public IList<EmployeeDiscipline> AttendanceViolations { get; set; }

        public string VialationXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (AttendanceViolations != null)
                {
                    foreach (EmployeeDiscipline violation in AttendanceViolations)
                    {
                        value.AppendFormat(
                            _VialationTpl,
                            violation.EmployeeId,
                            violation.WorkDate,
                            violation.ShiftName,
                            violation.CheckIn,
                            violation.CheckOut,
                            violation.TotalHours,
                            violation.LateIn,
                            violation.EarlyOut,
                            violation.Type,
                            violation.DisciplineScore,
                            HttpUtility.HtmlEncode(violation.EmployeeComment),
                            HttpUtility.HtmlEncode(violation.ApproverComment),
                            violation.IsIgnored,
                            violation.ModifiedDate,
                            violation.IsDeleted);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_EmployeeDiscipline";
        }

        public override bool IsValid()
        {
            return AttendanceViolations != null && AttendanceViolations.Any();
        }
    }
}