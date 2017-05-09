namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using EPA.Model.API;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdateDisciplineSummaryCriteria : CriteriaBase
    {
        private const string DisciplineTpl =
            "<Discipline EmpId='{0}' Period='{1}' ChangeShift='{2}' MissingCheckInOut='{3}' LateIn='{4}' EarlyOut='{5}'"
            + " Absent='{6}' UnpaidLeave='{7}' UnplannedLeave='{8}' WeekendLeave='{9}' WorkingUndertime='{10}' UnpaidLeaveAbsent='{11}'"
            + " OtherDisciplines='{12}' SystemSubtractScore='{13}' ModifiedDate='{14}'></Discipline>";

        [SpParam(Ignored = true)]
        public IList<DisciplineSummary> Disciplines { get; set; }

        public string DisciplineXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Disciplines != null)
                {
                    foreach (DisciplineSummary discipline in Disciplines)
                    {
                        value.AppendFormat(
                            DisciplineTpl,
                            discipline.EmployeeId,
                            discipline.Period,
                            discipline.ChangeShift,
                            discipline.MissingCheckInOut,
                            discipline.LateIn,
                            discipline.EarlyOut,
                            discipline.Absent,
                            discipline.UnpaidLeave,
                            discipline.UnplannedLeave,
                            discipline.WeekendLeave,
                            discipline.WorkingUndertime,
                            discipline.UnpaidLeaveAbsent,
                            discipline.OtherDisciplines,
                            discipline.SystemSubtractScore,
                            discipline.ModifiedDate);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_DisciplineSummary";
        }

        public override bool IsValid()
        {
            return Disciplines != null && Disciplines.Any();
        }
    }
}