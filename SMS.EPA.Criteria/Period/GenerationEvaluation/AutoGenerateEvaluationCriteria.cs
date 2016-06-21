namespace SMS.EPA.Criteria.Period.GenerationEvaluation
{
    using Fanex.Data.Repository;

    public class AutoGenerateEvaluationCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GenerateEPA";
        }
    }
}