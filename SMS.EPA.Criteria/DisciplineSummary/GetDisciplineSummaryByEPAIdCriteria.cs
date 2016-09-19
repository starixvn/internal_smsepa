namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryByEPAIdCriteria : CriteriaBase
    {
        public string EPAIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPAIds);
        }

        public override string GetSettingKey()
        {
            return "EmployeeDisciplineSummary_GetByEPA";
        }
    }
}