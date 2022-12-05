namespace SMS.EPA.AutoJob.Criteria
{
    using Fanex.Data.Repository;

    internal class GetLastSpecialUCDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetLastSpecialUCDate";

        public override bool IsValid() => true;
    }
}
