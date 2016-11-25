namespace SMS.EPA.ClientHrm.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetTeamCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}