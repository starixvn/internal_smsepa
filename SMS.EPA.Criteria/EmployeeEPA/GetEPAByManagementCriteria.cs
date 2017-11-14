namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAByManagementCriteria : CriteriaBase
    {
        public int ManagementId { get; set; }

        public string EmployeeIds { get; set; }

        public int? PeriodId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? StatusProcess { get; set; }

        public override string GetSettingKey() => "EPA_GetByManagement";

        public override bool IsValid() => ManagementId > 0;
    }
}