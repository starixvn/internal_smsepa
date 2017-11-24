namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateDisciplineSummaryCriteria : CriteriaBase
    {
        private const string DisciplineTpl =
            "<Discipline EmpId='{0}' WorkDate='{1}' ChangeShift='{2}' MissingCheckInOut='{3}' LateIn='{4}' EarlyOut='{5}'"
            + " Absent='{6}' UnpaidLeave='{7}' UnplannedLeave='{8}' WeekendLeave='{9}' WorkingUndertime='{10}' UnpaidLeaveAbsent='{11}'"
            + " SevenDaysLeave='{12}' OtherDisciplines='{13}' SystemSubtractScore='{14}' ModifiedDate='{15}' IsDeleted='{16}'></Discipline>";

        [SpParam(Ignored = true)]
        public IList<DisciplineSummary> Disciplines { get; set; }

        public string DisciplineXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (Disciplines != null)
                {
                    foreach (DisciplineSummary discipline in Disciplines)
                    {
                        value.AppendFormat(
                            DisciplineTpl,
                            discipline.EmployeeId,
                            discipline.WorkDate,
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
                            discipline.SevenDaysLeave,
                            discipline.OtherDisciplines,
                            discipline.SystemSubtractScore,
                            discipline.ModifiedDate,
                            discipline.IsDeleted);
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_DisciplineSummary";

        public override bool IsValid() => Disciplines != null && Disciplines.Any();
    }
}