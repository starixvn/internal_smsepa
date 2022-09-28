namespace SMS.EPA.Criteria.Dashboard
{
    using Fanex.Data.Repository;

    public class GetAnnouncementCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Dashboard_GetAnnouncement";

        public override bool IsValid() => EmployeeId > 0;
    }
}