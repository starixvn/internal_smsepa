namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByEvaluatorCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int EvaluatorId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByEvaluator";
        }
    }
}