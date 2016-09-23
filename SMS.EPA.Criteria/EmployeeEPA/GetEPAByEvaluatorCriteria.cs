namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string EmployeeIds { get; set; }

        public bool? IsLocal { get; set; }

        public bool IsMonthly { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByEvaluator";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}