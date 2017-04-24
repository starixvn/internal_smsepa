namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class UpdateDisciplineCriteria : CriteriaBase
    {
        private const string VialationTpl =
            "<Violation EmpId='{0}' WorkDate='{1}' ShiftName='{2}' CheckIn='{3}' CheckOut='{4}' TotalHours='{5}'"
            + " LateIn='{6}' EarlyOut='{7}' Type='{8}' DisciplineScore='{9}'"
            + " EmployeeComment='{10}' ApproverComment='{11}' IsMissingCheckInOut='{12}' IsIgnoredMissingCheckInOut='{13}'"
            + " IsLateIn='{14}' IsIgnoredLateIn='{15}' IsEarlyOut='{16}' IsIgnoredEarlyOut='{17}' IsWorkingUndertime='{18}'"
            + " IsIgnoredWorkingUndertime='{19}' IsChangeShift='{20}' IsIgnoredChangeShift='{21}' IsAbsent='{22}' IsIgnoredAbsent='{23}'"
            + " IsUnpaidLeave='{24}' IsIgnoredUnpaidLeave='{25}' IsUnpaidLeaveAbsent='{26}' IsIgnoredUnpaidLeaveAbsent='{27}'"
            + " ModifiedDate='{28}' IsDeleted='{29}'></Violation>";

        [SpParam(Ignored = true)]
        public IList<Discipline> Disciplines { get; set; }

        public string VialationXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Disciplines != null)
                {
                    foreach (Discipline discipline in Disciplines)
                    {
                        value.AppendFormat(
                            VialationTpl,
                            discipline.EmployeeId,
                            discipline.WorkDate,
                            discipline.ShiftName,
                            discipline.CheckIn,
                            discipline.CheckOut,
                            discipline.TotalHours ?? 0,
                            discipline.LateIn,
                            discipline.EarlyOut,
                            discipline.Type,
                            discipline.DisciplineScore,
                            HttpUtility.HtmlEncode(discipline.EmployeeComment),
                            HttpUtility.HtmlEncode(discipline.ApproverComment),
                            discipline.IsMissingCheckInOut,
                            discipline.IsIgnoredMissingCheckInOut,
                            discipline.IsLateIn,
                            discipline.IsIgnoredLateIn,
                            discipline.IsEarlyOut,
                            discipline.IsIgnoredEarlyOut,
                            discipline.IsWorkingUndertime,
                            discipline.IsIgnoredWorkingUndertime,
                            discipline.IsChangeShift,
                            discipline.IsIgnoredChangeShift,
                            discipline.IsAbsent,
                            discipline.IsIgnoredAbsent,
                            discipline.IsUnpaidLeave,
                            discipline.IsIgnoredUnpaidLeave,
                            discipline.IsUnpaidLeaveAbsent,
                            discipline.IsIgnoredUnpaidLeaveAbsent,
                            discipline.ModifiedDate,
                            discipline.IsDeleted);
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
            return Disciplines != null && Disciplines.Any();
        }
    }
}