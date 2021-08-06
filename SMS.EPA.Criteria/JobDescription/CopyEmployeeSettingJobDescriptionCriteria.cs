namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class CopyEmployeeSettingJobDescriptionCriteria : CriteriaBase
    {
        public int FromEmployeeId { get; set; }
        public int ToEmployeeId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "JobDescription_CopyEmployeeSetting";

        public override bool IsValid() => FromEmployeeId > 0 && ToEmployeeId > 0 && ActorId > 0;
    }
}