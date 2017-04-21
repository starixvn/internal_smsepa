namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetGroupDisciplinePermissionCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_GetForSync";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}