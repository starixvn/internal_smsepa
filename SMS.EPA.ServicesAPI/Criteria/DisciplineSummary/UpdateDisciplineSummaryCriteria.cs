namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class UpdateDisciplineSummaryCriteria : CriteriaBase
    {
        private const string DisciplineTpl =
            "<Discipline EmpId='{0}' Period='{1}' ChangeShift='{2}' MissingCheckInOut='{3}' LateIn='{4}' EarlyOut='{5}'"
            + " Absent='{6}' UnpaidLeave='{7}' WorkingUndertime='{8}' UnpaidLeaveAbsent='{9}'"
            + " OtherDisciplines='{10}' SystemSubtractScore='{11}' ModifiedDate='{12}'></Discipline>";

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