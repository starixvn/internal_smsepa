namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class GetGroupSettingJobDescriptionCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobDescription_GetGroupSetting";
    }
}