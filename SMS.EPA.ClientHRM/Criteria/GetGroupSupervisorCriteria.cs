namespace SMS.EPA.ClientHRM.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetGroupSupervisorCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_GetForSync";
        }
    }
}
